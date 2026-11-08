// ============ Bridge/NPCInteractable.cs ============
using Ink.Runtime;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    [Header("The NPC itself as an interactive object")]
    public InteractableDefinition selfInteractable;

    [Header("task")]
    public TaskDefinition task;

    [Header("dialogue file")]
    public TextAsset inkJSON;

    private Story story;

    void Start()
    {
        story = new Story(inkJSON.text);
        story.BindExternalFunction("hasFlag", (string flag) => (object)WorldState.Instance.HasFlag(flag));
    }

    private bool isTalking;

    public void Interact()
    {
        if (isTalking) return;
        isTalking = true;

        if (selfInteractable != null)
            InteractableRegistry.Instance.MarkInteracted(selfInteractable);

        TaskState stateBefore = TaskManager.Instance.GetState(task);

        InkStateBinder.Write(story, task);
        story.ChoosePathString("talk");

        DialogueRunner.Instance.Run(story, () =>
        {
            try
            {
                OnDialogueEnd(stateBefore);
            }
            finally
            {
                isTalking = false;
            }
        });
    }

  

    private void OnDialogueEnd(TaskState stateBefore)
    {
        TaskState stateAfter = InkStateBinder.ReadTaskState(story);

        // want take the task
        if (stateBefore == TaskState.NotStarted && stateAfter == TaskState.Accepted)
            TaskManager.Instance.AcceptTask(task);
    }
}