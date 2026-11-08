// ============ Data/GameDatabase.cs ============
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Game Database", fileName = "GameDatabase")]
public class GameDatabase : ScriptableObject
{
    public List<TaskDefinition> tasks = new();
    public List<ItemDefinition> items = new();
    public List<NoteDefinition> notes = new();
    public List<InteractableDefinition> interactables = new();

    private Dictionary<string, TaskDefinition> taskById;
    private Dictionary<string, ItemDefinition> itemById;
    private Dictionary<string, NoteDefinition> noteById;
    private Dictionary<string, InteractableDefinition> interactableById;

    public TaskDefinition GetTask(string id) => Get(tasks, ref taskById, id);
    public ItemDefinition GetItem(string id) => Get(items, ref itemById, id);
    public NoteDefinition GetNote(string id) => Get(notes, ref noteById, id);
    public InteractableDefinition GetInteractable(string id) => Get(interactables, ref interactableById, id);

    public ScriptableGuidObject GetById(string id)
        => (ScriptableGuidObject)GetTask(id)
        ?? (ScriptableGuidObject)GetItem(id)
        ?? (ScriptableGuidObject)GetNote(id)
        ?? (ScriptableGuidObject)GetInteractable(id);

    private static T Get<T>(List<T> list, ref Dictionary<string, T> index, string id)
        where T : ScriptableGuidObject
    {
        if (string.IsNullOrEmpty(id)) return null;

        if (index == null)
        {
            index = new Dictionary<string, T>();
            foreach (var item in list)
                if (item != null && item.HasId && !index.ContainsKey(item.Id))
                    index[item.Id] = item;
        }

        return index.TryGetValue(id, out var found) ? found : null;
    }
}
