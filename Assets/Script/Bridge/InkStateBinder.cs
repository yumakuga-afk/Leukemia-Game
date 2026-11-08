// ============ Bridge/InkStateBinder.cs ============
using Ink.Runtime;
using System;

/// <summary>The sole transition boundary between Ink and C# states</summary>
public static class InkStateBinder
{
    public const string TaskIdVar            = "task_id";
    public const string TaskStateVar         = "task_state";
    public const string HasRequiredItemsVar  = "has_required_items";
    public const string HasRequiredObjectsVar = "has_required_objects";

    /// <summary>C# → Ink</summary>
    public static void Write(Story story, TaskDefinition task)
    {
        var state = TaskManager.Instance.GetState(task);

        story.variablesState[TaskIdVar] = task.Id; // GUID；当前 Ink 尚未读取此值
        story.variablesState[TaskStateVar] = (float)state;
        story.variablesState[HasRequiredItemsVar] =
            TaskManager.Instance.HasRequiredItems(task) ? 1f : 0f;
        story.variablesState[HasRequiredObjectsVar] =
            TaskManager.Instance.HasRequiredInteractions(task) ? 1f : 0f;
    }

    /// <summary>Ink → C#</summary>
    public static TaskState ReadTaskState(Story story)
    {
        object v = story.variablesState[TaskStateVar];
        int i = Convert.ToInt32(v);
        if (i <= 0) return TaskState.NotStarted;
        if (i == 1) return TaskState.Accepted;
        return TaskState.Completed;
    }
}