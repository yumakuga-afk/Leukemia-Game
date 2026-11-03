// ============ Core/InteractableRegistry.cs ============
using System.Collections.Generic;
using UnityEngine;

public class InteractableRegistry : MonoBehaviour
{
    public static InteractableRegistry Instance;

    private readonly HashSet<InteractableDefinition> interacted = new();

    void Awake() => Instance = this;

    public bool HasInteracted(InteractableDefinition def)
        => def != null && interacted.Contains(def);

    public void MarkInteracted(InteractableDefinition def)
    {
        if (def == null || !interacted.Add(def)) return;

        GameEvents.RaiseInteracted(def);
        TaskManager.Instance.RefreshAllTasks();
    }
}