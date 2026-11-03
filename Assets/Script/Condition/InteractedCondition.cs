using UnityEngine;

[CreateAssetMenu(menuName = "Game/Condition/Interacted", fileName = "Cond_Interacted")]
public class InteractedCondition : NoteCondition
{
    public InteractableDefinition target;
    public override bool IsMet()
        => target != null && InteractableRegistry.Instance.HasInteracted(target);
}