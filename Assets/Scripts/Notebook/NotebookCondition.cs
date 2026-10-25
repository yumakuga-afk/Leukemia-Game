
using System;
using UnityEngine;

// Defines the different types of conditions
// the notebook can evaluate.
public enum NotebookConditionType
{
    HasItem,
    HasInteracted,
    HasFlag
}

[Serializable]
public class NotebookCondition
{
    [Header("Condition Settings")]

    [Tooltip("The type of game state to check.")]
    public NotebookConditionType conditionType;

    [Tooltip("The unique ID of the item, interaction, or flag.")]
    public string conditionID;

    [Tooltip("If enabled, the condition passes when the requirement is NOT met.")]
    public bool invertCondition = false;

    // ==========================================
    // CONDITION EVALUATION
    // ==========================================

    public bool IsMet()
    {
        // Make sure the GameStateManager exists.
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning(
                "[NotebookCondition] GameStateManager not found!"
            );

            return false;
        }

        // Conditions must have a valid ID.
        if (string.IsNullOrWhiteSpace(conditionID))
        {
            Debug.LogWarning(
                "[NotebookCondition] Condition ID is empty!"
            );

            return false;
        }

        bool result = false;

        // Check the appropriate game state.
        switch (conditionType)
        {
            case NotebookConditionType.HasItem:

                result = GameStateManager.Instance.HasItem(
                    conditionID
                );

                break;

            case NotebookConditionType.HasInteracted:

                result = GameStateManager.Instance.HasInteracted(
                    conditionID
                );

                break;

            case NotebookConditionType.HasFlag:

                result = GameStateManager.Instance.HasFlag(
                    conditionID
                );

                break;

            default:

                Debug.LogWarning(
                    "[NotebookCondition] Unknown condition type!"
                );

                return false;
        }

        // Reverse the result if requested.
        if (invertCondition)
        {
            result = !result;
        }

        return result;
    }
}

public enum NotebookConditionLogic
{
    All,
    Any
}

[Serializable]
public class NotebookConditionGroup
{
    [Header("Condition Group Settings")]

    [Tooltip("Determines whether ALL or ANY conditions must be met.")]
    public NotebookConditionLogic logic = NotebookConditionLogic.All;

    [Tooltip("The conditions that must be evaluated.")]
    public NotebookCondition[] conditions;

    // ==========================================
    // GROUP EVALUATION
    // ==========================================

    public bool IsMet()
    {
        // No requirements means the content is available.
        if (conditions == null || conditions.Length == 0)
        {
            return true;
        }

        if (logic == NotebookConditionLogic.All)
        {
            // Every condition must pass.
            foreach (NotebookCondition condition in conditions)
            {
                if (condition == null || !condition.IsMet())
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            // At least one condition must pass.
            foreach (NotebookCondition condition in conditions)
            {
                if (condition != null && condition.IsMet())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
