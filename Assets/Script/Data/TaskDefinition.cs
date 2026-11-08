// ============ Data/TaskDefinition.cs ============
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Task Definition", fileName = "Task_New")]
public class TaskDefinition : ScriptableGuidObject
{
    [Header("Completion Condition")]
    public List<ItemDefinition> requiredItems = new();
    public List<InteractableDefinition> requiredInteractables = new();

    [Header("MapChange?")]
    public bool changesMap;
    [Tooltip("作者命名的叙事 flag 名（非实体 ID，供 Ink 引用地图变化）")]
    public string mapFlagKey;
}
