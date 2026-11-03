// ============ Data/TaskDefinition.cs ============
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Task Definition", fileName = "Task_New")]
public class TaskDefinition : ScriptableObject
{
    [Header("Only ID(for Ink )")]
    public string taskId;

    [Header("Completion Condition")]
    public List<ItemDefinition> requiredItems = new();
    public List<InteractableDefinition> requiredInteractables = new();

    [Header("MapChange?")]
    public bool changesMap;
    public string mapFlagKey;

    void OnValidate()
    {
        if (string.IsNullOrEmpty(taskId)) taskId = name;
    }
}