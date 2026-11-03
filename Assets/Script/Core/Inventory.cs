// ============ Core/Inventory.cs ============
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private readonly HashSet<ItemDefinition> items = new();

    void Awake() => Instance = this;

    public bool HasItem(ItemDefinition item)
        => item != null && items.Contains(item);

    public bool AddItem(ItemDefinition item)
    {
        if (item == null || !items.Add(item)) return false;

        GameEvents.RaiseItemGained(item);
        TaskManager.Instance.RefreshAllTasks();
        return true;
    }

    public bool RemoveItem(ItemDefinition item)
    {
        bool ok = items.Remove(item);
        if (ok) NotebookSystem.Instance.RefreshAllNotes();
        return ok;
    }
}