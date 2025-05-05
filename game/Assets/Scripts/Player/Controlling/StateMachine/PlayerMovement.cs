using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// Notes:
// If you want to improve the jump—make it heavier, slower, etc.—try adjusting the Rigidbody's mass or the gravity settings first. 
// Alternatively, you can create custom gravity behavior in a script.
// That chances how controllable in air is

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour // Part of the player finite StateMachine
{
    // !!! The values that are now in variables are tested values
    // Movement settings
    [Header("Movement Settings")]
    [Range(0.001f, 100.0f)] public float deceleration = 0.001f; // Deceleration factor
    [Range(10f, 150f)] public float walkAcceleration = 20f; // Acceleration while walking
    [Range(1f, 150f)] public float walkMaxSpeed = 5f; // Maximum walking speed
    [Range(10f, 150f)] public float sprintAcceleration = 21f; // Acceleration while sprinting
    [Range(1f, 150f)] public float sprintMaxSpeed = 7.15f; // Maximum sprinting speed
    [Range(0f, 10f)] public float groundDrag = 1.3f; // Drag applied when grounded
    public float jumpForce = 10f; // Force applied when jumping
    public float jumpCooldown = 0.58f; // Cooldown time between jumps
    [HideInInspector] public float lastJumpTime; // Timestamp of the last jump

    [Header("Effects Settings")]
    [Range(1f, 179.0f)] public float BaseFieldOfView = 95.0f; // Duration of the jump effect
    [Range(1f, 179.0f)] public float SprintFieldOfView = 105.0f; // Duration of the jump effect

    // Ground detection settings
    [Header("Ground Detection Settings")]
    public CapsuleCollider playerCollider; // Reference to the player's CapsuleCollider
    private List<Collider> feetColliders = new List<Collider>(); // Colliders at feet level
    public bool IsGrounded => feetColliders.Count > 0; // Check if the player is grounded

    // Dependencies
    [Header("Dependencies")]
    public Transform orientation; // Object used to orient the player
    public Animator ArmsAnimator;

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

    int SpeedHash; // Hash for speed parameter in animator

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
        SpeedHash = Animator.StringToHash("Speed"); // Hash for speed parameter in animator
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
            //Debug.Log("jump");
            stateMachine.ChangeState(new PlayerJumpState(this, stateMachine));
        }
        else if (isSprinting && moveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(new PlayerSprintState(this, stateMachine));
        }
        else if (moveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(new PlayerWalkState(this, stateMachine));
        }
        else
        {
            stateMachine.ChangeState(new PlayerIdleState(this, stateMachine));
        }

        // Update animator speed parameter (0.0 to 1.0 based on velocity)

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        // Apply threshold to prevent tiny floating point noise
        if (horizontalSpeed < 0.01f)
            horizontalSpeed = 0f;

        float normalizedSpeed = horizontalSpeed / sprintMaxSpeed;
        ArmsAnimator.SetFloat("Speed", Mathf.Clamp01(normalizedSpeed));

        Debug.Log(ArmsAnimator.GetFloat("Speed"));



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
