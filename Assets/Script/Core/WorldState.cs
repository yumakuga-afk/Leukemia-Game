// ============ Core/WorldState.cs ============
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static WorldState Instance;

    private readonly HashSet<string> flags = new();

    void Awake() => Instance = this;

    void OnEnable()
    {
        GameEvents.TaskAccepted  += OnTaskAccepted;
        GameEvents.TaskCompleted += OnTaskCompleted;
        GameEvents.ItemGained    += OnItemGained;
        GameEvents.Interacted    += OnInteracted;
    }

    void OnDisable()
    {
        GameEvents.TaskAccepted  -= OnTaskAccepted;
        GameEvents.TaskCompleted -= OnTaskCompleted;
        GameEvents.ItemGained    -= OnItemGained;
        GameEvents.Interacted    -= OnInteracted;
    }

    public bool HasFlag(string flag) => flags.Contains(flag);

    public void SetFlag(string flag)
    {
        if (!string.IsNullOrEmpty(flag)) flags.Add(flag);
    }

    private void OnTaskAccepted(TaskDefinition t)
        => SetFlag("task_accepted_" + t.Id);

    private void OnTaskCompleted(TaskDefinition t)
    {
        SetFlag("task_completed_" + t.Id);
        if (t.changesMap) SetFlag(t.mapFlagKey);
    }

    private void OnItemGained(ItemDefinition i)
        => SetFlag("item_" + i.Id);

    private void OnInteracted(InteractableDefinition i)
        => SetFlag("interacted_" + i.Id);
}