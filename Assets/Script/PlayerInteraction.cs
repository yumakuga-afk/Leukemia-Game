using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private InteractableObject currentTarget;
    
    [Header("Detection")]
    [SerializeField] private Vector2 boxSize = new Vector2(1f, 1.5f);
    [SerializeField] private float castDistance = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    Vector2 currentPosition;
    Vector2 lastPosition;

    [SerializeField] private InputActionReference interactAction;

    private Vector2 facingDirection = Vector2.right;

   

    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact pressed");

        if (currentTarget != null)
        {
            currentTarget.Interact();   
        }
    }


    void LateUpdate()
    {
        Vector2 currentPosition = transform.position;

        Vector2 displacement = currentPosition - lastPosition;


        if (displacement.sqrMagnitude > 0.000001f)
            facingDirection = displacement.normalized;

        lastPosition = currentPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 direction = Application.isPlaying
            ? (Vector3)facingDirection
            : Vector3.right;

        Vector3 center = transform.position +
            direction * (castDistance * 0.5f);

        Vector2 previewSize = boxSize;

        previewSize.x += Mathf.Abs(direction.x) * castDistance;
        previewSize.y += Mathf.Abs(direction.y) * castDistance;

        Gizmos.DrawWireCube(center, previewSize);
    }

    void Update()
    {
        DetectInteractables();
    }

    private void DetectInteractables()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            transform.position,
            boxSize,
            0f,
            facingDirection,
            castDistance,
            interactableLayer
        );

        InteractableObject nearest = null;
        
        float minDistance = float.MaxValue;

        foreach (RaycastHit2D hit in hits)
        {
            Collider2D col = hit.collider;

            if (col == null) continue;

            // Ignore the player's own collider
            if (col.transform.IsChildOf(transform))
                continue;


            if (hit.collider.TryGetComponent<InteractableObject>(out var interactable))
            {
                
                // Get the point on the collider that is closest to the player
                Vector2 closestPoint =
                    col.ClosestPoint(transform.position);

                float distance = Vector2.SqrMagnitude(
                    closestPoint - (Vector2)transform.position
                );

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = interactable;
            }
            }

        }
        
        SetTarget(nearest);
    }

   private void SetTarget(InteractableObject newTarget)
    {   // Don't chaeck null because if it null, hide previous ui
        if (currentTarget == newTarget)
            return;
        currentTarget?.HideHighlight();

        currentTarget = newTarget;

        currentTarget?.ShowHighlight();
    }
}
