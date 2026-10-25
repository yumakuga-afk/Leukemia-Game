
using System;
using System.Collections.Generic;
using UnityEngine;

public class NotebookManager : MonoBehaviour
{
    // ==========================================
    // SINGLETON
    // ==========================================

    public static NotebookManager Instance { get; private set; }

    [Header("Notebook Entries")]
    [SerializeField]
    private List<NotebookEntry> allEntries =
        new List<NotebookEntry>();

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;

    // Currently unlocked entries and sections.
    private HashSet<string> unlockedEntryIDs =
        new HashSet<string>();

    private HashSet<string> unlockedSectionIDs =
        new HashSet<string>();

    private GameStateManager gameState;

    // ==========================================
    // EVENTS
    // ==========================================

    public event Action OnNotebookChanged;

    public event Action<NotebookEntry> OnEntryUnlocked;

    public event Action<NotebookEntry, NotebookEntrySection>
        OnSectionUnlocked;

    // ==========================================
    // INITIALIZATION
    // ==========================================

    private void Awake()
    {
        // Prevent duplicate Notebook Managers.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this manager alive between scene changes.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Connect to GameStateManager.
        gameState = GameStateManager.Instance;

        if (gameState == null)
        {
            Debug.LogError(
                "[NotebookManager] GameStateManager not found!"
            );

            return;
        }

        // Initialize the notebook without generating
        // notifications for existing discoveries.
        RefreshNotebook(true);

        // Subscribe to game state changes.
        gameState.OnGameStateChanged += HandleGameStateChanged;

        LogMessage("NotebookManager initialized and persistent.");
    }

    private void OnDestroy()
    {
        // Disconnect from GameStateManager.
        if (gameState != null)
        {
            gameState.OnGameStateChanged -= HandleGameStateChanged;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ==========================================
    // GAME STATE CHANGES
    // ==========================================

    private void HandleGameStateChanged()
    {
        RefreshNotebook();
    }

    // ==========================================
    // NOTEBOOK REFRESH
    // ==========================================

    public void RefreshNotebook(bool initialize = false)
    {
        HashSet<string> currentEntries =
            new HashSet<string>();

        HashSet<string> currentSections =
            new HashSet<string>();

        List<NotebookEntry> newEntries =
            new List<NotebookEntry>();

        List<NotebookEntry> sectionOwners =
            new List<NotebookEntry>();

        List<NotebookEntrySection> newSections =
            new List<NotebookEntrySection>();

        foreach (NotebookEntry entry in allEntries)
        {
            if (entry == null || !entry.IsUnlocked())
                continue;

            if (string.IsNullOrWhiteSpace(entry.entryID))
            {
                Debug.LogWarning(
                    "[NotebookManager] Entry has an empty ID.",
                    entry
                );

                continue;
            }

            if (!currentEntries.Add(entry.entryID))
            {
                Debug.LogWarning(
                    "[NotebookManager] Duplicate entry ID: " +
                    entry.entryID,
                    entry
                );

                continue;
            }

            if (!initialize &&
                !unlockedEntryIDs.Contains(entry.entryID))
            {
                newEntries.Add(entry);
            }

            foreach (NotebookEntrySection section in
                     entry.GetUnlockedSections())
            {
                if (section == null ||
                    string.IsNullOrWhiteSpace(section.sectionID))
                    continue;

                string sectionKey =
                    entry.entryID + ":" + section.sectionID;

                if (!currentSections.Add(sectionKey))
                    continue;

                if (!initialize &&
                    !unlockedSectionIDs.Contains(sectionKey))
                {
                    sectionOwners.Add(entry);
                    newSections.Add(section);
                }
            }
        }

        bool changed =
            !unlockedEntryIDs.SetEquals(currentEntries) ||
            !unlockedSectionIDs.SetEquals(currentSections);

        unlockedEntryIDs = currentEntries;
        unlockedSectionIDs = currentSections;

        if (initialize || !changed)
            return;

        foreach (NotebookEntry entry in newEntries)
        {
            LogMessage(
                "New entry unlocked: " + entry.entryTitle
            );

            OnEntryUnlocked?.Invoke(entry);
        }

        for (int i = 0; i < newSections.Count; i++)
        {
            LogMessage(
                "New section unlocked: " +
                newSections[i].sectionTitle
            );

            OnSectionUnlocked?.Invoke(
                sectionOwners[i],
                newSections[i]
            );
        }

        OnNotebookChanged?.Invoke();
    }

    // ==========================================
    // ENTRY RETRIEVAL
    // ==========================================

    public List<NotebookEntry> GetAllUnlockedEntries()
    {
        List<NotebookEntry> results =
            new List<NotebookEntry>();

        HashSet<string> addedIDs =
            new HashSet<string>();

        foreach (NotebookEntry entry in allEntries)
        {
            if (entry == null)
                continue;

            if (unlockedEntryIDs.Contains(entry.entryID) &&
                addedIDs.Add(entry.entryID))
            {
                results.Add(entry);
            }
        }

        return results;
    }

    public List<NotebookEntry> GetEntriesByCategory(
        NotebookCategory category)
    {
        List<NotebookEntry> results =
            new List<NotebookEntry>();

        foreach (NotebookEntry entry in GetAllUnlockedEntries())
        {
            if (entry.category == category)
            {
                results.Add(entry);
            }
        }

        return results;
    }

    public NotebookEntry GetEntryByID(string entryID)
    {
        foreach (NotebookEntry entry in allEntries)
        {
            if (entry != null && entry.entryID == entryID)
            {
                return entry;
            }
        }

        return null;
    }

    // ==========================================
    // UNLOCK CHECKS
    // ==========================================

    public bool IsEntryUnlocked(string entryID)
    {
        return unlockedEntryIDs.Contains(entryID);
    }

    public bool IsSectionUnlocked(
        string entryID,
        string sectionID)
    {
        string sectionKey =
            entryID + ":" + sectionID;

        return unlockedSectionIDs.Contains(sectionKey);
    }

    // ==========================================
    // ENTRY REGISTRATION
    // ==========================================

    public void RegisterEntry(NotebookEntry entry)
    {
        if (entry == null ||
            string.IsNullOrWhiteSpace(entry.entryID))
            return;

        if (GetEntryByID(entry.entryID) != null)
        {
            Debug.LogWarning(
                "[NotebookManager] Entry already registered: " +
                entry.entryID
            );

            return;
        }

        allEntries.Add(entry);

        RefreshNotebook();
    }

    // ==========================================
    // DEBUGGING
    // ==========================================

    private void LogMessage(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[NotebookManager] " + message);
        }
    }
}
