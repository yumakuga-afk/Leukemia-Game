
using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestProgressStatus
{
    NotStarted,
    InProgress,
    Completed
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance
    {
        get;
        private set;
    }

    [Header("Available Quests")]
    [SerializeField]
    private List<QuestDefinition> quests =
        new List<QuestDefinition>();

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    public event Action<QuestDefinition> OnQuestUpdated;

    private GameStateManager State =>
        GameStateManager.Instance;

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
            Instance = null;
    }

    // ==========================================
    // QUEST LOOKUP
    // ==========================================

    public QuestDefinition GetQuest(string questID)
    {
        foreach (QuestDefinition quest in quests)
        {
            if (quest != null && quest.questID == questID)
                return quest;
        }

        return null;
    }

    // ==========================================
    // QUEST STATUS
    // ==========================================

    public QuestProgressStatus GetStatus(string questID)
    {
        if (State == null || GetQuest(questID) == null)
            return QuestProgressStatus.NotStarted;

        if (State.HasFlag(CompletedFlag(questID)))
            return QuestProgressStatus.Completed;

        if (State.HasFlag(AcceptedFlag(questID)))
            return QuestProgressStatus.InProgress;

        return QuestProgressStatus.NotStarted;
    }

    // ==========================================
    // ACCEPT QUEST
    // ==========================================

    public void AcceptQuest(string questID)
    {
        QuestDefinition quest = GetQuest(questID);

        if (quest == null || State == null)
            return;

        if (GetStatus(questID) !=
            QuestProgressStatus.NotStarted)
            return;

        State.SetFlag(AcceptedFlag(questID));

        Log("Quest accepted: " + quest.questTitle);

        OnQuestUpdated?.Invoke(quest);
    }

    // ==========================================
    // OBJECTIVE PROGRESS
    // ==========================================

    public int GetCompletedObjectives(string questID)
    {
        QuestDefinition quest = GetQuest(questID);

        if (quest == null || State == null)
            return 0;

        if (GetStatus(questID) ==
            QuestProgressStatus.NotStarted)
            return 0;

        int completed = 0;

        // Count completed objectives in sequence.
        for (int i = 0; i < quest.objectives.Count; i++)
        {
            if (State.HasFlag(ObjectiveFlag(questID, i)))
                completed++;
            else
                break;
        }

        return completed;
    }

    public QuestObjective GetCurrentObjective(string questID)
    {
        QuestDefinition quest = GetQuest(questID);

        if (quest == null ||
            GetStatus(questID) !=
            QuestProgressStatus.InProgress)
            return null;

        int index = GetCompletedObjectives(questID);

        if (index >= quest.objectives.Count)
            return null;

        return quest.objectives[index];
    }

    // ==========================================
    // ADVANCE QUEST
    // ==========================================

    public void AdvanceQuest(string questID)
    {
        QuestDefinition quest = GetQuest(questID);

        if (quest == null || State == null)
            return;

        if (GetStatus(questID) !=
            QuestProgressStatus.InProgress)
            return;

        int index = GetCompletedObjectives(questID);

        if (index >= quest.objectives.Count)
        {
            CompleteQuest(questID);
            return;
        }

        State.SetFlag(ObjectiveFlag(questID, index));

        Log(
            "Completed objective " + (index + 1) +
            " for quest: " + quest.questTitle
        );

        // Automatically finish after the final objective.
        if (index + 1 >= quest.objectives.Count)
        {
            CompleteQuest(questID);
        }
        else
        {
            OnQuestUpdated?.Invoke(quest);
        }
    }

    // ==========================================
    // COMPLETE QUEST
    // ==========================================

    public void CompleteQuest(string questID)
    {
        QuestDefinition quest = GetQuest(questID);

        if (quest == null || State == null)
            return;

        if (GetStatus(questID) !=
            QuestProgressStatus.InProgress)
            return;

        // Do not mark the quest complete while
        // required objectives remain unfinished.
        if (GetCompletedObjectives(questID) <
            quest.objectives.Count)
            return;

        State.SetFlag(CompletedFlag(questID));

        Log("Quest completed: " + quest.questTitle);

        OnQuestUpdated?.Invoke(quest);
    }

    // ==========================================
    // PERSISTENT FLAG IDS
    // ==========================================

    private string AcceptedFlag(string questID)
    {
        return "quest:" + questID + ":accepted";
    }

    private string CompletedFlag(string questID)
    {
        return "quest:" + questID + ":completed";
    }

    private string ObjectiveFlag(string questID, int index)
    {
        return "quest:" + questID +
               ":objective:" + index;
    }

    // ==========================================
    // DEBUGGING
    // ==========================================

    private void Log(string message)
    {
        if (enableDebugLogs)
            Debug.Log("[QuestManager] " + message);
    }
}
