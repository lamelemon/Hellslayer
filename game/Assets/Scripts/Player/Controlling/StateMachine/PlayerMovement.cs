using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour // Part of the player finite StateMachine
{
    // Movement settings
    [Header("Movement Settings")]
    [Range(0.001f, 100.0f)] public float deceleration = 0.1f; // Deceleration factor
    [Range(10f, 150f)] public float walkAcceleration; // Acceleration while walking
    [Range(1f, 150f)] public float walkMaxSpeed = 12f; // Maximum walking speed
    [Range(10f, 150f)] public float sprintAcceleration = 5f; // Acceleration while sprinting
    [Range(1f, 150f)] public float sprintMaxSpeed = 20f; // Maximum sprinting speed
    [Range(0f, 10f)] public float groundDrag = 4f; // Drag applied when grounded
    public float jumpForce = 5f; // Force applied when jumping
    public float jumpCooldown = 5f; // Cooldown time between jumps
    [HideInInspector] public float lastJumpTime; // Timestamp of the last jump

    // Ground detection settings
    [Header("Ground Detection Settings")]
    public CapsuleCollider playerCollider; // Reference to the player's CapsuleCollider
    private List<Collider> feetColliders = new List<Collider>(); // Colliders at feet level
    public bool IsGrounded => feetColliders.Count > 0; // Check if the player is grounded

    // Dependencies
    [Header("Dependencies")]
    public Transform orientation; // Object used to orient the player

    // Input and state variables
    [HideInInspector] public bool isSprinting = false; // Is the player sprinting?
    [HideInInspector] public Rigidbody rb; // Rigidbody component
    [HideInInspector] public Vector2 moveInput; // Movement input
    [HideInInspector] public bool isJumping; // Is the player attempting to jump?
    [HideInInspector] public bool readyToJump = true; // Is the player ready to jump?

    private PlayerInput playerInput; // Input system reference
    private InputAction moveAction; // Movement input action
    private InputAction sprintAction; // Sprint input action
    private InputAction jumpAction; // Jump input action

    public PlayerStateMachine stateMachine; // Player state machine

    private void Awake()
    {
        // Initialize components and state machine
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent tipping over
        playerInput = new PlayerInput(); // Generated Input System class
        stateMachine = new PlayerStateMachine();
    }

    private void OnEnable()
    {
        // Enable input actions
        moveAction = playerInput.Player.MovementInput;
        sprintAction = playerInput.Player.SprintInput;
        jumpAction = playerInput.Player.JumpInput;

        moveAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        // Disable input actions
        moveAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
    }

    private void Start()
    {
        // Initialize the state machine with the idle state
        stateMachine.Initialize(new PlayerIdleState(this, stateMachine));
    }

    private void Update()
    {
        // Apply drag when grounded
        if (IsGrounded)
        {
            rb.linearDamping = groundDrag; // Apply drag when grounded
        }
        else
        {
            rb.linearDamping = groundDrag; // No drag in the air
        }
        //rb.linearDamping = IsGrounded ? groundDrag : 0f;

        // Read input values
        moveInput = moveAction.ReadValue<Vector2>();
        isSprinting = sprintAction.ReadValue<float>() > 0.1f;
        isJumping = jumpAction.ReadValue<float>() > 0;

        // Handle state transitions based on input and conditions
        if (isJumping && IsGrounded && readyToJump)
        {
            Debug.Log("jump");
            stateMachine.ChangeState(new PlayerJumpState(this, stateMachine));
        }
        else if (isSprinting && moveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(new PlayerRunState(this, stateMachine));
        }
        else if (moveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(new PlayerWalkState(this, stateMachine));
        }
        else
        {
            stateMachine.ChangeState(new PlayerIdleState(this, stateMachine));
        }

        // Delegate update logic to the current state
        stateMachine.currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        // Delegate physics-related logic to the current state
        stateMachine.currentState.FixedUpdateState();
    }

    // Ground detection using collision events
    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // Calculate the feet level (center - extents.y)
            float feetLevel = playerCollider.bounds.center.y - playerCollider.bounds.extents.y;

            // If the contact point is at feet level, add the collider to the list
            if (Mathf.Abs(contact.point.y - feetLevel) < 0.1f) // Tolerance to check feet level
            {
                if (!feetColliders.Contains(collision.collider))
                {
                    feetColliders.Add(collision.collider);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Remove the collider from the list when the collision ends
        if (feetColliders.Contains(collision.collider))
        {
            feetColliders.Remove(collision.collider);
        }
    }

    // Visualize the feet level in the editor
    private void OnDrawGizmos()
    {
        if (playerCollider != null)
        {
            float feetLevel = playerCollider.bounds.center.y - playerCollider.bounds.extents.y;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(playerCollider.bounds.min.x, feetLevel, playerCollider.bounds.min.z),
                            new Vector3(playerCollider.bounds.max.x, feetLevel, playerCollider.bounds.max.z));
        }
    }
}
