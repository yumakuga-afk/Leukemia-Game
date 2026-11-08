// ============ Data/ItemDefinition.cs ============
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Definition", fileName = "Item_New")]
public class ItemDefinition : ScriptableGuidObject
{
    [Tooltip("Are there have note?")]
    public bool hasNote;
}
