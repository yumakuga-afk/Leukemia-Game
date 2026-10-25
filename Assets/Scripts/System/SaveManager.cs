
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    // ==========================================
    // SINGLETON
    // ==========================================

    public static SaveManager Instance { get; private set; }

    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "savegame.json";
    [SerializeField] private bool loadOnStart = true;
    [SerializeField] private bool autoSave = true;
    [SerializeField] private bool saveOnQuit = true;

    [Header("New Game Settings")]
    [SerializeField] private int startingSceneBuildIndex = 0;

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;

    private GameStateManager gameState;

    private bool isInitialized = false;
    private bool isTransitioning = false;

    private const int CurrentSaveVersion = 2;

    private string SavePath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                saveFileName
            );
        }
    }

    // ==========================================
    // INITIALIZATION
    // ==========================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        // Allow other managers to initialize.
        yield return null;

        gameState = GameStateManager.Instance;

        if (gameState == null)
        {
            Debug.LogError(
                "[SaveManager] GameStateManager not found!"
            );
            yield break;
        }

        gameState.OnGameStateChanged += HandleGameStateChanged;
        SceneManager.sceneLoaded += HandleSceneLoaded;

        if (loadOnStart && SaveExists())
        {
            if (TryReadSave(out SaveData data))
            {
                isTransitioning = true;
                yield return LoadGameRoutine(data);
                isTransitioning = false;
            }
        }

        isInitialized = true;

        LogMessage("SaveManager initialized.");
        LogMessage("Save path: " + SavePath);
    }

    private void OnDestroy()
    {
        if (gameState != null)
        {
            gameState.OnGameStateChanged -= HandleGameStateChanged;
        }

        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ==========================================
    // SAVING
    // ==========================================

    public bool SaveGame()
    {
        if (!isInitialized || isTransitioning ||
            gameState == null)
        {
            Debug.LogWarning(
                "[SaveManager] Saving is currently unavailable."
            );
            return false;
        }

        try
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning(
                    "[SaveManager] Player not found. Save cancelled."
                );
                return false;
            }

            SaveData data = gameState.GetSaveData();

            data.saveVersion = CurrentSaveVersion;
            data.saveDate = DateTime.UtcNow.ToString("o");

            data.sceneName =
                SceneManager.GetActiveScene().name;

            data.playerPosition =
                player.transform.position;

            string json = JsonUtility.ToJson(data, true);

            // Write a temporary file first, then replace
            // the main file after writing succeeds.
            string tempPath = SavePath + ".tmp";

            File.WriteAllText(tempPath, json);

            if (File.Exists(SavePath))
            {
                // Keep a backup of the last complete save.
                string backupPath = SavePath + ".bak";

                File.Copy(
                    SavePath,
                    backupPath,
                    true
                );

                File.Delete(SavePath);
            }

            File.Move(tempPath, SavePath);

            LogMessage(
                "Game saved in scene: " + data.sceneName
            );

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[SaveManager] Save failed: " +
                exception.Message
            );
            return false;
        }
    }

    // ==========================================
    // READING SAVE FILES
    // ==========================================

    private bool TryReadSave(out SaveData data)
    {
        data = null;

        if (!SaveExists())
        {
            LogMessage("No save file found.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            data = JsonUtility.FromJson<SaveData>(json);

            if (data == null ||
                data.saveVersion != CurrentSaveVersion ||
                string.IsNullOrWhiteSpace(data.sceneName))
            {
                Debug.LogError(
                    "[SaveManager] Invalid or unsupported save data."
                );

                data = null;
                return false;
            }

            if (!Application.CanStreamedLevelBeLoaded(
                data.sceneName))
            {
                Debug.LogError(
                    "[SaveManager] Scene is not in Build Settings: " +
                    data.sceneName
                );

                data = null;
                return false;
            }

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[SaveManager] Could not read save: " +
                exception.Message
            );

            return false;
        }
    }

    // ==========================================
    // LOADING
    // ==========================================

    public void LoadGame()
    {
        if (!isInitialized || isTransitioning ||
            gameState == null)
            return;

        if (!TryReadSave(out SaveData data))
            return;

        isTransitioning = true;
        StartCoroutine(LoadGameFromMenu(data));
    }

    private IEnumerator LoadGameFromMenu(SaveData data)
    {
        yield return LoadGameRoutine(data);

        isTransitioning = false;
    }

    private IEnumerator LoadGameRoutine(SaveData data)
    {
        LogMessage("Loading scene: " + data.sceneName);

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(
                data.sceneName,
                LoadSceneMode.Single
            );

        if (operation == null)
        {
            Debug.LogError(
                "[SaveManager] Scene loading could not start."
            );
            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        // Allow scene objects to finish initialization.
        yield return null;

        GameObject player = null;

        try
        {
            player = GameObject.FindGameObjectWithTag(
                "Player"
            );
        }
        catch (UnityException exception)
        {
            Debug.LogError(
                "[SaveManager] Player tag error: " +
                exception.Message
            );
        }

        if (player == null)
        {
            Debug.LogError(
                "[SaveManager] No tagged Player in loaded scene."
            );
        }
        else
        {
            player.transform.position = data.playerPosition;

            Rigidbody2D rb =
                player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.position = new Vector2(
                    data.playerPosition.x,
                    data.playerPosition.y
                );

                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            Physics2D.SyncTransforms();
        }

        // Restore discoveries after the new scene exists.
        // This fires OnGameStateChanged once, allowing
        // notebook entries and world objects to refresh.
        gameState.LoadSaveData(data);

        LogMessage("Game loaded successfully.");
    }

    // ==========================================
    // AUTOSAVE
    // ==========================================

    private void HandleGameStateChanged()
    {
        if (!isInitialized || isTransitioning)
            return;

        if (autoSave)
        {
            SaveGame();
        }
    }

    private void HandleSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        if (!isInitialized || isTransitioning ||
            !autoSave)
            return;

        StartCoroutine(SaveAfterSceneLoad());
    }

    private IEnumerator SaveAfterSceneLoad()
    {
        // Give the new scene time to initialize.
        yield return null;

        if (!isTransitioning)
        {
            SaveGame();
        }
    }

    // ==========================================
    // NEW GAME
    // ==========================================

    public void NewGame()
    {
        if (!isInitialized || isTransitioning ||
            gameState == null)
            return;

        if (!Application.CanStreamedLevelBeLoaded(
            startingSceneBuildIndex))
        {
            Debug.LogError(
                "[SaveManager] Starting scene is not configured."
            );
            return;
        }

        isTransitioning = true;
        StartCoroutine(NewGameRoutine());
    }

    private IEnumerator NewGameRoutine()
    {
        // Clear investigation progress.
        gameState.ResetGameState();

        // Delete existing saves.
        try
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);

            if (File.Exists(SavePath + ".bak"))
                File.Delete(SavePath + ".bak");

            if (File.Exists(SavePath + ".tmp"))
                File.Delete(SavePath + ".tmp");
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[SaveManager] Failed to delete save files: " +
                exception.Message
            );

            isTransitioning = false;
            yield break;
        }

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(
                startingSceneBuildIndex,
                LoadSceneMode.Single
            );

        if (operation == null)
        {
            Debug.LogError(
                "[SaveManager] Starting scene could not load."
            );

            isTransitioning = false;
            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        yield return null;

        isTransitioning = false;

        LogMessage("New game started.");
    }

    // ==========================================
    // FILE UTILITIES
    // ==========================================

    public bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    public string GetSavePath()
    {
        return SavePath;
    }

    // ==========================================
    // APPLICATION EXIT
    // ==========================================

    private void OnApplicationQuit()
    {
        if (saveOnQuit && isInitialized &&
            !isTransitioning)
        {
            SaveGame();
        }
    }

    // ==========================================
    // DEBUGGING
    // ==========================================

    private void LogMessage(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[SaveManager] " + message);
        }
    }
}
