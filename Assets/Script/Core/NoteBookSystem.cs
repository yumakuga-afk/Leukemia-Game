// ============ Core/NotebookSystem.cs ============
using System.Collections.Generic;
using UnityEngine;

public class NotebookSystem : MonoBehaviour
{
    public static NotebookSystem Instance;

    private readonly HashSet<NoteDefinition> unlocked = new();

    void Awake() => Instance = this;

    public bool IsUnlocked(NoteDefinition note) => unlocked.Contains(note);

    /// <summary>Full re-run; any operation that "might affect notes" should trigger it.</summary>
    public void RefreshAllNotes()
    {
        var db = GameRoot.Instance != null ? GameRoot.Instance.database : null;
        if (db == null) return;

        foreach (var note in db.notes)
        {
            if (note == null || unlocked.Contains(note)) continue;
            if (!AreConditionsMet(note)) continue;

            unlocked.Add(note);
            GameEvents.RaiseNoteUnlocked(note);
            Debug.Log($"[笔记解锁] {note.name}");
        }
    }

    private bool AreConditionsMet(NoteDefinition note)
    {
        foreach (var c in note.conditions)
        {
            if (c == null) continue;
            if (!c.IsMet()) return false;
        }
        return true;
    }
}