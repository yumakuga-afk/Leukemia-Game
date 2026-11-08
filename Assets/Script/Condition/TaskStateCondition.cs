// ============ Condition/TaskStateCondition.cs ============
using UnityEngine;
public enum TaskState
{
    NotStarted = 0,
    Accepted   = 1,
    Completed  = 2
}

[CreateAssetMenu(menuName = "Game/Condition/Task State At Least", fileName = "Cond_TaskState")]
public class TaskStateCondition : NoteCondition
{
    public TaskDefinition task;
    public TaskState minState = TaskState.Accepted;

    public override bool IsMet()
    //change if met the minState, like if unlock this note have to Finish this task, then will check this task current state and minState
        => task != null && TaskManager.Instance.GetState(task) >= minState;
}