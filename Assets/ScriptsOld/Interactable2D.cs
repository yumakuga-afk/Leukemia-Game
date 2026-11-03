
using UnityEngine;
using UnityEngine.Events;

public class Interactable2D : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private bool canInteract = true;

    [Header("Interaction Events")]
    [SerializeField] private UnityEvent onInteract;

    public void Interact()
    {
        if (!canInteract)
            return;

        Debug.Log("Interacted with: " + gameObject.name);

        onInteract?.Invoke();
    }

    public void SetInteractable(bool value)
    {
        canInteract = value;
    }
}
