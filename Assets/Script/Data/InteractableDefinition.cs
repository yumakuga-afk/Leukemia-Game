// ============ Data/InteractableDefinition.cs ============
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Interactable Definition", fileName = "Int_New")]
public class InteractableDefinition : ScriptableObject
{
    public string interactableId;

    void OnValidate()
    {
        if (string.IsNullOrEmpty(interactableId)) interactableId = name;
    }
}