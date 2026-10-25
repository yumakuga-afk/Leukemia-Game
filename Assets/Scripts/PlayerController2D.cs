
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference interactAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private LayerMask interactionLayer;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool jumpRequested;
    private bool isGrounded;
    private Vector2 facingDirection = Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();

        if (jumpAction != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += OnJump;
        }

        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.action.performed -= OnJump;
            jumpAction.action.Disable();
        }

        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }

        if (moveAction != null)
            moveAction.action.Disable();

        horizontalInput = 0f;
        jumpRequested = false;
    }

    private void Update()
    {
        ReadMovement();
        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        MovePlayer();
        ProcessJump();
    }

    // -------------------------
    // MOVEMENT
    // -------------------------

    private void ReadMovement()
    {
        if (moveAction == null)
        {
            horizontalInput = 0f;
            return;
        }

        Vector2 movement = moveAction.action.ReadValue<Vector2>();

        horizontalInput = Mathf.Clamp(movement.x, -1f, 1f);
    }

    private void MovePlayer()
    {
        rb.velocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.velocity.y
        );
    }

    // -------------------------
    // JUMPING
    // -------------------------

    private void OnJump(InputAction.CallbackContext context)
    {
        jumpRequested = true;
    }

    private void ProcessJump()
    {
        if (!jumpRequested)
            return;

        jumpRequested = false;

        if (!isGrounded)
            return;

        rb.velocity = new Vector2(
            rb.velocity.x,
            jumpForce
        );
    }

    // -------------------------
    // GROUND DETECTION
    // -------------------------

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        ) != null;
    }

    // -------------------------
    // INTERACTION
    // -------------------------

    private void OnInteract(InputAction.CallbackContext context)
    {
        Interact();
    }

    private void Interact()
    {
        Vector2 interactionPosition =
            (Vector2)transform.position +
            facingDirection * (interactionRadius * 0.5f);

        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(
            interactionPosition,
            interactionRadius,
            interactionLayer
        );

        Interactable2D closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D hit in nearbyObjects)
        {
            Interactable2D interactable =
                hit.GetComponentInParent<Interactable2D>();

            if (interactable == null)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                hit.ClosestPoint(transform.position)
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = interactable;
            }
        }

        if (closestObject != null)
            closestObject.Interact();
    }

    // -------------------------
    // SPRITE FACING DIRECTION
    // -------------------------

    private void UpdateFacingDirection()
    {
        if (horizontalInput > 0.01f)
        {
            facingDirection = Vector2.right;

            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
        }
        else if (horizontalInput < -0.01f)
        {
            facingDirection = Vector2.left;

            if (spriteRenderer != null)
                spriteRenderer.flipX = true;
        }
    }

    // -------------------------
    // DEBUG GIZMOS
    // -------------------------

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        Gizmos.color = Color.yellow;

        Vector2 interactionPosition =
            (Vector2)transform.position +
            facingDirection * (interactionRadius * 0.5f);

        Gizmos.DrawWireSphere(
            interactionPosition,
            interactionRadius
        );
    }
}
