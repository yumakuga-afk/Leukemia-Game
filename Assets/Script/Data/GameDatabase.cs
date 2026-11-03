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
}