// ============ Data/ItemDefinition.cs ============
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Definition", fileName = "Item_New")]
public class ItemDefinition : ScriptableObject
{
    public string itemId;

    [Tooltip("Are there have note?")]
    public bool hasNote;

    void OnValidate()
    {
        if (string.IsNullOrEmpty(itemId)) itemId = name;
    }
}