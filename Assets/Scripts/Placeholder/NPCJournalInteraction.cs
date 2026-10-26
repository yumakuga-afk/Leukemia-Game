
using UnityEngine;

public class NPCJournalInteraction : MonoBehaviour
{
    [Header("NPC Information")]
    [Tooltip("The notebook entry containing this NPC's profile.")]
    [SerializeField] private NotebookEntry npcEntry;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    // ==========================================
    // NPC IDENTIFICATION
    // ==========================================

    private string NPCID
    {
        get
        {
            return npcEntry != null
                ? npcEntry.entryID
                : "";
        }
    }

    private string MetFlag
    {
        get
        {
            return "npc:" + NPCID + ":met";
        }
    }

    private string HintFlag(int index)
    {
        return "npc:" + NPCID + ":hint:" + index;
    }

    // ==========================================
    // FIRST MEETING
    // ==========================================

    public void MeetNPC()
    {
        if (!ValidateNPC())
            return;

        GameStateManager.Instance.SetFlag(MetFlag);

        Log("NPC met: " + npcEntry.entryTitle);
    }

    // ==========================================
    // DISCOVER NPC INFORMATION
    // ==========================================

    public void RevealHint(int hintIndex)
    {
        if (!ValidateNPC())
            return;

        if (hintIndex < 0)
        {
            Debug.LogWarning(
                "[NPCJournalInteraction] Invalid hint index.",
                this
            );
            return;
        }

        // Ensure the NPC has been encountered.
        MeetNPC();

        GameStateManager.Instance.SetFlag(
            HintFlag(hintIndex)
        );

        Log(
            "Hint " + hintIndex +
            " discovered for " + npcEntry.entryTitle
        );
    }

    // ==========================================
    // QUERY METHODS
    // ==========================================

    public bool HasMetNPC()
    {
        if (!ValidateNPC())
            return false;

        return GameStateManager.Instance.HasFlag(MetFlag);
    }

    public bool HasDiscoveredHint(int hintIndex)
    {
        if (!ValidateNPC() || hintIndex < 0)
            return false;

        return GameStateManager.Instance.HasFlag(
            HintFlag(hintIndex)
        );
    }

    // ==========================================
    // VALIDATION
    // ==========================================

    private bool ValidateNPC()
    {
        if (npcEntry == null ||
            string.IsNullOrWhiteSpace(NPCID))
        {
            Debug.LogWarning(
                "[NPCJournalInteraction] NPC entry missing " +
                "or has an empty ID.",
                this
            );

            return false;
        }

        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning(
                "[NPCJournalInteraction] GameStateManager missing.",
                this
            );

            return false;
        }

        return true;
    }

    // ==========================================
    // DEBUGGING
    // ==========================================

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[NPCJournalInteraction] " + message);
        }
    }
}
