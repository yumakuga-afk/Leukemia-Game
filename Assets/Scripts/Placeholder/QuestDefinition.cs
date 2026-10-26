
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestObjective
{
    public string objectiveID;
    public string objectiveDescription;
}

[CreateAssetMenu(
    fileName = "NewQuest",
    menuName = "Detective Notebook/Quest"
)]
public class QuestDefinition : ScriptableObject
{
    [Header("Quest Information")]
    public string questID;
    public string questTitle;

    [TextArea(3, 8)]
    public string questDescription;

    [Header("Objectives")]
    public List<QuestObjective> objectives =
        new List<QuestObjective>();

    [Header("Notebook")]
    public NotebookEntry notebookEntry;
}
