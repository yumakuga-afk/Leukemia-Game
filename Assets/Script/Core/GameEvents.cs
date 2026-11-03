// ============ Core/GameEvents.cs ============
using System;

public static class GameEvents
{
    public static event Action<TaskDefinition> TaskAccepted;
    public static event Action<TaskDefinition> TaskCompleted;
    public static event Action<ItemDefinition> ItemGained;
    public static event Action<InteractableDefinition> Interacted;
    public static event Action<NoteDefinition> NoteUnlocked;

    public static void RaiseTaskAccepted(TaskDefinition t)  => TaskAccepted?.Invoke(t);
    public static void RaiseTaskCompleted(TaskDefinition t) => TaskCompleted?.Invoke(t);
    public static void RaiseItemGained(ItemDefinition i)    => ItemGained?.Invoke(i);
    public static void RaiseInteracted(InteractableDefinition i) => Interacted?.Invoke(i);
    public static void RaiseNoteUnlocked(NoteDefinition n)  => NoteUnlocked?.Invoke(n);
}