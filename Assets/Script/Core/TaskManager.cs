// ============ Core/TaskManager.cs ============
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

   
    private readonly Dictionary<TaskDefinition, TaskState> states = new();

    void Awake() => Instance = this;

    // ---------- Check ----------


    public TaskState GetState(TaskDefinition task)
    {
        if (task == null) return TaskState.NotStarted;
        return states.TryGetValue(task, out var s) ? s : TaskState.NotStarted;
    }

    public bool HasRequiredItems(TaskDefinition task)
        => task != null && task.requiredItems.All(i => Inventory.Instance.HasItem(i));

    public bool HasRequiredInteractions(TaskDefinition task)
        => task != null && task.requiredInteractables.All(
            i => InteractableRegistry.Instance.HasInteracted(i));

    public bool IsConditionMet(TaskDefinition task)
        => HasRequiredItems(task) && HasRequiredInteractions(task);

    // ---------- State Change ----------

    
    public bool AcceptTask(TaskDefinition task)
    {
        if (task == null) return false;
        if (GetState(task) != TaskState.NotStarted) return false;

        states[task] = TaskState.Accepted;
        GameEvents.RaiseTaskAccepted(task);

        RefreshAllTasks(); // 接取时可能已满足条件
        return true;
    }

    public bool CompleteTask(TaskDefinition task)
    {
        if (GetState(task) != TaskState.Accepted) return false;

        states[task] = TaskState.Completed;
        GameEvents.RaiseTaskCompleted(task);
        NotebookSystem.Instance.RefreshAllNotes();
        return true;
    }

    /// <summary>Check all accepted tasks and automatically complete those that meet the requirements; refresh notes once at the end.</summary>
    public void RefreshAllTasks()
    {
        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var kv in states.ToList())
            {
                if (kv.Value == TaskState.Accepted && IsConditionMet(kv.Key))
                {
                    CompleteTask(kv.Key);
                    changed = true;
                }
            }
        }

        NotebookSystem.Instance.RefreshAllNotes();
    }
}