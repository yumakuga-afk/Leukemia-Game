// ============ Data/NoteDefinition.cs ============
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Note Definition", fileName = "Note_New")]
public class NoteDefinition : ScriptableObject
{
    public string noteId;
    [TextArea(3, 10)] public string content;

    [Tooltip("Conditions for unlocking")]
    public List<NoteCondition> conditions = new();

    void OnValidate()
    {
        if (string.IsNullOrEmpty(noteId)) noteId = name;
    }
}