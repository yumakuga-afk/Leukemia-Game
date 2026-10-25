
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // ==========================================
    // SINGLETON
    // ==========================================

    public static GameStateManager Instance { get; private set; }

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;

    // ==========================================
    // GAME STATE
    // ==========================================

    private HashSet<string> collectedItems =
        new HashSet<string>();

    private HashSet<string> interactions =
        new HashSet<string>();

    private HashSet<string> progressionFlags =
        new HashSet<string>();

    // Notifies other systems when game state changes.
    public event Action OnGameStateChanged;

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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ==========================================
    // ITEM TRACKING
    // ==========================================

    public void AddItem(string itemID)
    {
        if (string.IsNullOrWhiteSpace(itemID))
            return;

        if (collectedItems.Add(itemID))
        {
            LogMessage("Item collected: " + itemID);
            NotifyStateChanged();
        }
    }

    public void RemoveItem(string itemID)
    {
        if (string.IsNullOrWhiteSpace(itemID))
            return;

        if (collectedItems.Remove(itemID))
        {
            LogMessage("Item removed: " + itemID);
            NotifyStateChanged();
        }
    }

    public bool HasItem(string itemID)
    {
        return collectedItems.Contains(itemID);
    }

    public List<string> GetCollectedItems()
    {
        return new List<string>(collectedItems);
    }

    // ==========================================
    // INTERACTION TRACKING
    // ==========================================

    public void RecordInteraction(string interactionID)
    {
        if (string.IsNullOrWhiteSpace(interactionID))
            return;

        if (interactions.Add(interactionID))
        {
            LogMessage("Interaction recorded: " + interactionID);
            NotifyStateChanged();
        }
    }

    public bool HasInteracted(string interactionID)
    {
        return interactions.Contains(interactionID);
    }

    public List<string> GetInteractions()
    {
        return new List<string>(interactions);
    }

    // ==========================================
    // PROGRESSION FLAGS
    // ==========================================

    public void SetFlag(string flagID)
    {
        if (string.IsNullOrWhiteSpace(flagID))
            return;

        if (progressionFlags.Add(flagID))
        {
            LogMessage("Flag activated: " + flagID);
            NotifyStateChanged();
        }
    }

    public void RemoveFlag(string flagID)
    {
        if (string.IsNullOrWhiteSpace(flagID))
            return;

        if (progressionFlags.Remove(flagID))
        {
            LogMessage("Flag removed: " + flagID);
            NotifyStateChanged();
        }
    }

    public bool HasFlag(string flagID)
    {
        return progressionFlags.Contains(flagID);
    }

    public List<string> GetFlags()
    {
        return new List<string>(progressionFlags);
    }

    // ==========================================
    // SAVE DATA EXPORT
    // ==========================================

    public SaveData GetSaveData()
    {
        SaveData data = new SaveData();

        data.collectedItems =
            new List<string>(collectedItems);

        data.interactions =
            new List<string>(interactions);

        data.progressionFlags =
            new List<string>(progressionFlags);

        // Keep output order consistent.
        data.collectedItems.Sort();
        data.interactions.Sort();
        data.progressionFlags.Sort();

        return data;
    }

    // ==========================================
    // SAVE DATA RESTORATION
    // ==========================================

    public void LoadSaveData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError(
                "[GameStateManager] Cannot load null save data."
            );
            return;
        }

        // Replace the previous state with the loaded state.
        collectedItems = new HashSet<string>(
            data.collectedItems ?? new List<string>()
        );

        interactions = new HashSet<string>(
            data.interactions ?? new List<string>()
        );

        progressionFlags = new HashSet<string>(
            data.progressionFlags ?? new List<string>()
        );

        LogMessage("Game state restored from save data.");

        // Notify the notebook and other listeners once.
        NotifyStateChanged();
    }

    // ==========================================
    // NEW GAME / RESET
    // ==========================================

    public void ResetGameState()
    {
        collectedItems.Clear();
        interactions.Clear();
        progressionFlags.Clear();

        LogMessage("Game state reset.");

        NotifyStateChanged();
    }

    // ==========================================
    // NOTIFICATIONS
    // ==========================================

    private void NotifyStateChanged()
    {
        OnGameStateChanged?.Invoke();
    }

    // ==========================================
    // DEBUGGING
    // ==========================================

    private void LogMessage(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[GameStateManager] " + message);
        }
    }
}
