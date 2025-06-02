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
    [Range(10f, 150f)] public float crouchAcceleration = 17.5f; // Acceleration while crouching
    [Range(1f, 150f)] public float crouchMaxSpeed = 2.5f; // Maximum speed while crouching
    [SerializeField][Range(0f, 10f)] private float groundDrag = 1.3f; // Drag applied when grounded
    public float jumpForce = 10f; // Force applied when jumping
    public float jumpCooldown = 0.58f; // Cooldown time between jumps
    [HideInInspector] public float lastJumpTime; // Timestamp of the last jump

    [Header("Sound Settings")]
    [Range(0.01f, 20.0f)] public float JumpSoundPitchMin = 0.70f;
    [Range(0.01f, 20.0f)] public float JumpSoundPitchMax = 0.82f;

    // Dependencies
    [Header("Dependencies")]
    public Transform orientation; // Object used to orient the player
    public Animator ArmsAnimator;

    // Ground detection Settings/Dependencies
    public CapsuleCollider GroundCollider; // Reference to the player's CapsuleCollider
    private readonly List<Collider> feetColliders = new(); // Colliders at feet level
    public bool IsGrounded => feetColliders.Count > 0; // Check if the player is grounded
    private float playerHitboxRadius; // Only used for mantle detection as an estimate so pretty please don't change it
    private float feetLevel; // Feet level based on the GroundCollider's bounds
    // Input and state variables
    [HideInInspector] public bool isSprinting = false; // Is the player sprinting?
    [HideInInspector] public Rigidbody rb; // Rigidbody component
    [HideInInspector] public bool isJumping; // Is the player attempting to jump?
    [HideInInspector] public bool readyToJump = true; // Is the player ready to jump?
    [HideInInspector] public float ZeroToOneMaxSpeed; // Speed variable for animator (0.0 to 1.0 based on velocity / max sprint speed)
    [HideInInspector] public PlayerInputManager GetInput;
    [HideInInspector] public float standingHeight; // Height of the player when standing
    [HideInInspector] public float crouchHeight;

    // Crouching variables
    [HideInInspector] public bool isCrouching = false;
    [HideInInspector] public CapsuleCollider playerHitbox; // Player's hitbox collider (used for crouching and mantle detection)
    [HideInInspector] public LayerMask layerMask; // Layer mask for ground detection


    public PlayerStateMachine stateMachine = new(); // Player state machine



    private void Awake()
    {
        // Initialize components and state machine
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent tipping over
        GetInput = GetComponent<PlayerInputManager>(); // Find the PlayerInputManager in the scene
        playerHitbox = GetComponentInChildren<CapsuleCollider>(); // get hitbox collider (change if you use different collider)
    }


    private void Start()
    {
        // Initialize the state machine with the idle state
        stateMachine.Initialize(new PlayerIdleState(this, stateMachine));
        layerMask = ~LayerMask.GetMask("Player", "Ignore Overlaps", "Ignore Raycast", "Loot", "enemyLayer");
        standingHeight = playerHitbox.height; // Get the player's standing height from the hitbox collider
        crouchHeight = standingHeight / 2; // Set the crouch height to half of the standing height
        playerHitboxRadius = playerHitbox.bounds.extents.z;
    }



    private void Update()
    {
        GroundDrag(); // Apply drag when grounded
        InputsValuesReader(); // Read input values
        print(IsGrounded);


        // Handle state transitions based on input and conditions
        if (isCrouching && IsGrounded)
        {
            print("crouching");
            stateMachine.ChangeState(new PlayerCrouchState(this, stateMachine)); // Change to crouch state
        }
        else if (isJumping && readyToJump)
        {
            if (IsGrounded) stateMachine.ChangeState(new PlayerJumpState(this, stateMachine));
            else Mantle(); // If not grounded, try to mantle
        }

        else if (GetInput.MoveValue.magnitude > 0.1f)
        {
            if (isSprinting) stateMachine.ChangeState(new PlayerSprintState(this, stateMachine));
            else stateMachine.ChangeState(new PlayerWalkState(this, stateMachine));
        }

        else
        {
            stateMachine.ChangeState(new PlayerIdleState(this, stateMachine));
        }


        ArmsAnimatorSpeedVariable(); // Update animator speed parameter (0.0 to 1.0 based on velocity)

        // Delegate update logic to the current state
        stateMachine.currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        // Delegate physics-related logic to the current state
        stateMachine.currentState.FixedUpdateState();
    }



    private void OnCollisionEnter(Collision collision) // Ground detection Enter -using collision events
    {
        feetLevel = GroundCollider.bounds.center.y - GroundCollider.bounds.extents.y;
        foreach (ContactPoint contact in collision.contacts)
        {
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

    private void OnCollisionExit(Collision collision) // Ground detection Exit
    {
        // Remove the collider from the list when the collision ends
        if (feetColliders.Contains(collision.collider))
        {
            feetColliders.Remove(collision.collider);
        }
    }

    private void GroundDrag()
    {
        // Apply drag when grounded
        if (IsGrounded) rb.linearDamping = groundDrag; // Apply drag when grounded
        else rb.linearDamping = 0f; // No drag in the air
    }

    private void InputsValuesReader()
    {
        // Read input values
        isSprinting = GetInput.SprintInput.IsPressed();
        isJumping = GetInput.JumpInput.WasPressedThisFrame();
        isCrouching = GetInput.CrouchInput.IsPressed();
    }

    private void ArmsAnimatorSpeedVariable()
    {
        // Update animator speed parameter (0.0 to 1.0 based on velocity)
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        // Apply threshold to prevent tiny floating point noise
        if (horizontalSpeed < 0.01f)
            horizontalSpeed = 0f;

        float normalizedSpeed = horizontalSpeed / sprintMaxSpeed;
        ZeroToOneMaxSpeed = Mathf.Clamp01(normalizedSpeed); // Clamp the value between 0.0 and 1.0
        ArmsAnimator.SetFloat("Speed", ZeroToOneMaxSpeed);
        //Debug.Log(ArmsAnimator.GetFloat("Speed"));
    }

    private void Mantle()
    {
        // Calculate the mantle position based on player dimensions and orientation
        Vector3 mantlePosition = transform.position + (standingHeight / 2 + playerHitbox.bounds.size.y) * Vector3.up + Quaternion.Euler(0, rb.transform.eulerAngles.y, 0) * (playerHitboxRadius * 2 * Vector3.forward);

        // Check for a ledge and if the player fits on the ledge
        if (Physics.Raycast(mantlePosition + Vector3.up * standingHeight / 2, Vector3.down, out RaycastHit hit, standingHeight, layerMask) && Physics.OverlapCapsule(mantlePosition + playerHitboxRadius * Vector3.up, mantlePosition + (standingHeight + playerHitboxRadius) * Vector3.up, playerHitboxRadius, layerMask).Length == 0)
        {
            // Move the player to the ledge position
            transform.position = hit.point + Vector3.up * standingHeight / 2;
        }
    }

    // *Not nesesary functions/code

    // Visualize the feet level in the editor
    private void OnDrawGizmos()
    {
        if (GroundCollider != null)
        {
            float feetLevel = GroundCollider.bounds.center.y - GroundCollider.bounds.extents.y;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(GroundCollider.bounds.min.x, feetLevel, GroundCollider.bounds.min.z),
                            new Vector3(GroundCollider.bounds.max.x, feetLevel, GroundCollider.bounds.max.z));
        }
    }
    
}
