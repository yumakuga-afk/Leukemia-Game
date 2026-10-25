
using System;
using System.Collections.Generic;
using UnityEngine;

// Determines what type of change to make.
public enum NotebookWorldActionType
{
    RecordInteraction,
    AddItem,
    RemoveItem,
    SetFlag,
    RemoveFlag
}

// A single action performed by an interactable object.
[Serializable]
public class NotebookWorldAction
{
    [Tooltip("What should happen when this action runs?")]
    public NotebookWorldActionType actionType;

    [Tooltip("The item, interaction, or progression flag ID.")]
    public string actionID;
}

public class NotebookWorldEvent : MonoBehaviour
{
    // ==========================================
    // ACTION SETTINGS
    // ==========================================

    [Header("Actions")]
    [SerializeField]
    private List<NotebookWorldAction> actions =
        new List<NotebookWorldAction>();

    // ==========================================
    // PERSISTENT OBJECT SETTINGS
    // ==========================================

    [Header("Persistent Object Settings")]

    [Tooltip("Unique ID for this particular world object. " +
             "Must be different for every one-time object.")]
    [SerializeField] private string persistentObjectID;

    [Tooltip("Prevent this object from activating more than once.")]
    [SerializeField] private bool oneTimeInteraction = true;

    [Tooltip("Hide this object after it has been activated.")]
    [SerializeField] private bool hideAfterInteraction = false;

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;

    // ==========================================
    // RUNTIME DATA
    // ==========================================

    private bool hasActivated = false;

    private GameStateManager gameState;
    private bool subscribedToGameState = false;

    // Prefix prevents collision with ordinary
    // discovery flags.
    private const string ActivationPrefix = "world_activated:";

    private string ActivationFlagID
    {
        get
        {
            return ActivationPrefix + persistentObjectID;
        }
    }

    // ==========================================
    // INITIALIZATION
    // ==========================================

    private void Start()
    {
        ConnectToGameState();
        RestoreObjectState();
    }

    private void OnEnable()
    {
        // Handles objects reactivated during gameplay.
        if (gameState != null)
        {
            ConnectToGameState();
            RestoreObjectState();
        }
    }

    private void OnDisable()
    {
        DisconnectFromGameState();
    }

    private void OnDestroy()
    {
        DisconnectFromGameState();
    }

    // ==========================================
    // GAME STATE CONNECTION
    // ==========================================

    private void ConnectToGameState()
    {
        GameStateManager currentState =
            GameStateManager.Instance;

        if (subscribedToGameState &&
            gameState == currentState)
        {
            return;
        }

        DisconnectFromGameState();

        gameState = currentState;

        if (gameState == null)
        {
            Debug.LogWarning(
                "[NotebookWorldEvent] GameStateManager not found!",
                this
            );

            return;
        }

        gameState.OnGameStateChanged += HandleGameStateChanged;
        subscribedToGameState = true;
    }

    private void DisconnectFromGameState()
    {
        if (gameState != null && subscribedToGameState)
        {
            gameState.OnGameStateChanged -= HandleGameStateChanged;
        }

        subscribedToGameState = false;
    }

    private void HandleGameStateChanged()
    {
        // Respond when a save is loaded or when
        // the persistent activation flag changes.
        RestoreObjectState();
    }

    // ==========================================
    // PERSISTENT STATE
    // ==========================================

    private bool HasValidPersistentID()
    {
        return !string.IsNullOrWhiteSpace(persistentObjectID);
    }

    private bool WasPreviouslyActivated()
    {
        if (!oneTimeInteraction ||
            !HasValidPersistentID() ||
            gameState == null)
        {
            return false;
        }

        return gameState.HasFlag(ActivationFlagID);
    }

    private void RestoreObjectState()
    {
        if (!oneTimeInteraction || gameState == null)
            return;

        bool wasActivated = WasPreviouslyActivated();

        hasActivated = wasActivated;

        if (wasActivated)
        {
            LogMessage("Previously completed interaction restored.");

            if (hideAfterInteraction)
            {
                gameObject.SetActive(false);
            }
        }
    }

    // ==========================================
    // INTERACTION
    // ==========================================

    public void Activate()
    {
        if (gameState == null)
        {
            ConnectToGameState();
        }

        if (gameState == null)
        {
            Debug.LogError(
                "[NotebookWorldEvent] Cannot activate without " +
                "GameStateManager.",
                this
            );
            return;
        }

        // One-time objects require a stable ID.
        if (oneTimeInteraction && !HasValidPersistentID())
        {
            Debug.LogError(
                "[NotebookWorldEvent] One-time object '" +
                gameObject.name +
                "' needs a Persistent Object ID.",
                this
            );
            return;
        }

        // Check persistent state, not only local memory.
        if (oneTimeInteraction &&
            (hasActivated || WasPreviouslyActivated()))
        {
            LogMessage("Interaction already completed.");
            return;
        }

        bool executedAction = false;

        // ======================================
        // EXECUTE CONFIGURED ACTIONS
        // ======================================

        foreach (NotebookWorldAction action in actions)
        {
            if (action == null ||
                string.IsNullOrWhiteSpace(action.actionID))
            {
                continue;
            }

            switch (action.actionType)
            {
                case NotebookWorldActionType.RecordInteraction:
                    gameState.RecordInteraction(action.actionID);
                    executedAction = true;
                    break;

                case NotebookWorldActionType.AddItem:
                    gameState.AddItem(action.actionID);
                    executedAction = true;
                    break;

                case NotebookWorldActionType.RemoveItem:
                    gameState.RemoveItem(action.actionID);
                    executedAction = true;
                    break;

                case NotebookWorldActionType.SetFlag:
                    gameState.SetFlag(action.actionID);
                    executedAction = true;
                    break;

                case NotebookWorldActionType.RemoveFlag:
                    gameState.RemoveFlag(action.actionID);
                    executedAction = true;
                    break;
            }

            LogMessage(
                "Executed " + action.actionType +
                " with ID: " + action.actionID
            );
        }

        if (!executedAction)
        {
            LogMessage("No valid actions configured.");
            return;
        }

        // ======================================
        // RECORD PERSISTENT ACTIVATION
        // ======================================

        if (oneTimeInteraction)
        {
            // Set local state before broadcasting
            // the persistent flag change.
            hasActivated = true;

            gameState.SetFlag(ActivationFlagID);

            LogMessage(
                "Persistent activation recorded: " +
                ActivationFlagID
            );
        }

        // ======================================
        // HIDE OBJECT IF CONFIGURED
        // ======================================

        if (hideAfterInteraction && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    // ==========================================
    // RESET LOCAL INTERACTION
    // ==========================================

    public void ResetInteraction()
    {
        // This does not erase the persistent flag.
        hasActivated = false;

        if (gameState != null)
        {
            RestoreObjectState();
        }

        LogMessage("Local interaction state refreshed.");
    }

    // ==========================================
    // DEBUGGING
    // ==========================================

    private void LogMessage(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log(
                "[NotebookWorldEvent] " +
                gameObject.name + ": " + message,
                this
            );
        }
    }

    // ==========================================
    // EDITOR VALIDATION
    // ==========================================

    private void OnValidate()
    {
        if (oneTimeInteraction &&
            string.IsNullOrWhiteSpace(persistentObjectID))
        {
            // ID is assigned manually in the Inspector.
            // Warning is issued when activation is attempted.
        }
    }
}
