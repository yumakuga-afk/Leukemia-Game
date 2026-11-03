using UnityEngine;

[CreateAssetMenu(menuName = "Game/Condition/Item Held", fileName = "Cond_ItemHeld")]
public class ItemHeldCondition : NoteCondition
{
    public ItemDefinition item;
    public override bool IsMet() => item != null && Inventory.Instance.HasItem(item);
}