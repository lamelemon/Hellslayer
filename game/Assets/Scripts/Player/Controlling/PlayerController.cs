using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Game Objects
    private Rigidbody rb; // References to the CharacterController
    private CapsuleCollider playerHitbox;

    // Moving
    private float playerSpeed; // Current player speed
    private float steer = 1f; // How much input affects velocity
    private readonly float aerialsteer = 0.7f;
    private readonly float playerWalkSpeed = 10.0f; // Walking speed constant
    private readonly float playerRunMultiplier = 1.5f; // Multiplier for running speed
    private readonly float playerCrouchSpeedMultiplier = 0.5f; // Multiplier for crouch speed
    private Vector3 Movement;
    private Vector3 externalForces;
    public bool IsRunning { get; private set; } // Boolean indicating if the player is running;

    // Jumping
    public float playerJumpHeight = 7.0f; // Jump height constant
    private readonly float maxCoyoteTime = 0.2f;
    private float coyoteTime;
    public bool IsJumping { get; private set; } // Boolean indicating if the player is currently jumping
    private readonly float jumpingCooldown = 0.3f; // jump cooldown in seconds
    private bool canJump = true;

    // Crouching
    private float playerStandingHeight; // Player height when standing
    private readonly float playerCrouchingHeight = 3.5f; // Player height when crouching
    public bool IsCrouching { get; private set; } // Boolean indicating if the player is crouching

    // Sliding
    private readonly float playerSlidingHeight = 3.5f;
    private bool isSliding;

    // Mantling
    private readonly Collider[] mantleResults = new Collider[10]; // Array to store colliders for mantling detection
    private Vector3 mantlePosition;

    // Buffers
    private readonly List<Collider> overlapResults = new(); // Used for checking collisions when standing up from crouch
    public bool IsOnFloor { get; private set; } 
    private readonly List<ContactPoint> floorContacts = new();
    private int layerMask; // Layer mask for collision detection
    private Bounds playerHitboxBounds;

    // Model position
    private Vector3 modelStartPosition; // Store the initial position of the player model (for crouch animations)

    // Camera Position
    private Vector3 cameraStartPosition; // Store the initial position of the camera (for crouch animations) 
    // Serializations
    [SerializeField] PlayerInputManager GetInput; // Reference to the PlayerInputManager for capturing input
    [SerializeField] PlayerCamera playerCamera; // Reference to the PlayerCamera to handle camera rotation
    [SerializeField] GameObject model; // Reference to the player model (used for crouching animation)
    [SerializeField] Stamina_System staminaSystem; // Reference to the stamina system

    private void Awake()
    {
        // Initializing the references
        GetInput = GetComponent<PlayerInputManager>();
        rb = gameObject.GetComponent<Rigidbody>();
        playerHitbox = GameObject.FindGameObjectWithTag("PlayerHitbox").GetComponent<CapsuleCollider>();
        layerMask = ~LayerMask.GetMask("Player", "Ignore Overlaps", "Ignore Raycast");
    }

    private void Start()
    {
        // Set the initial player height and center offsets
        playerStandingHeight = playerHitbox.height;
        modelStartPosition = playerStandingHeight / 2 * Vector3.down; // Store the starting position of the model for crouching
        coyoteTime = maxCoyoteTime;
        cameraStartPosition = playerCamera.transform.localPosition; // Store the starting position of the camera for crouching
        playerSpeed = playerWalkSpeed; // Set the initial player speed to walking speed
        playerHitboxBounds = playerHitbox.bounds; // Set the bounds of the player hitbox for collision detection
    }

    void Update()
    {
        if (!IsOnFloor)
        {
            coyoteTime -= Time.deltaTime;
        }

        Crouch();
        Sliding();
        Mantle();

        if (!IsCrouching && !isSliding && staminaSystem.CanSprint && GetInput.SprintInput.WasPressedThisFrame())
        {
            playerSpeed *= playerRunMultiplier;
            IsRunning = true;
        }

        else if (IsRunning && (!GetInput.SprintInput.IsPressed() || isSliding || IsCrouching || !staminaSystem.CanSprint))
        {
            IsRunning = false;
            playerSpeed = playerWalkSpeed;
        }

        // Rotate the player model to face the camera's direction
        rb.rotation = Quaternion.Euler(0, playerCamera.TotalYRot, 0);

        // random debugging outputs
        //print($"{IsOnFloor} {rb.linearVelocity} FPS: {Mathf.Round(1 / Time.deltaTime)}");
    }

    void FixedUpdate()
    {
        if (IsOnFloor)
        {
            steer = 1;
            coyoteTime = maxCoyoteTime;
            IsJumping = false;
        }

        else
        {
            steer = aerialsteer;
        }

        Jumping();
        Moving();
    }

    void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            // Check if the contact point is below the player's hitbox and mark the player as on the floor
            if (collision.GetContact(i).point.y < transform.position.y - playerHitbox.height / 2 + playerHitbox.radius / 2)
            {
                IsOnFloor = true;
                floorContacts.Add(collision.GetContact(i)); // Add the collider to the list of floor contacts
                break;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Check if the collision object was part of the floor contacts
        for (int i = 0; i < floorContacts.Count; i++)
        {
            if (floorContacts[i].otherCollider == collision.collider)
            {
                floorContacts.RemoveAt(i);
                i --;
            }
        }

        // Update the IsOnFloor status
        IsOnFloor = floorContacts.Count > 0;
    }

    // Handle crouching behavior
    private void Crouch()
    {
        if (!isSliding && !IsCrouching && GetInput.CrouchInput.IsPressed())
        {
            // Adjust controller height for crouching
            playerHitbox.height = playerCrouchingHeight; // Adjust the center position for crouching
            model.transform.localPosition = playerHitbox.height / 2 * Vector3.down; // Adjust model position for crouching
            playerSpeed *= playerCrouchSpeedMultiplier; // Reduce the speed when crouching
            IsCrouching = true; // Mark the player as crouching
            playerCamera.transform.localPosition = new(playerCamera.transform.localPosition.x, playerCrouchingHeight / 2, playerCamera.transform.localPosition.z); // Adjust camera position for crouching
            rb.MovePosition(transform.position - (playerStandingHeight - playerCrouchingHeight) / 2 * Vector3.up); // Physically move the player to the crouch position
        }

        else if (IsCrouching && !GetInput.CrouchInput.IsPressed() && CanUnCrouch())
        {
            // Physically move the player to the standing position
            rb.MovePosition(transform.position + (playerStandingHeight - playerCrouchingHeight) / 2 * Vector3.up); // Move the player to the standing position
            playerSpeed = playerWalkSpeed; // Reset the speed to normal
            playerCamera.transform.localPosition = cameraStartPosition; // Adjust camera position for crouching
            model.transform.localPosition = modelStartPosition; // Return the model to its original position
            playerHitbox.height = playerStandingHeight; // Restore the standing height
            IsCrouching = false; // Mark the player as not crouching
        }
    }

    private void Jumping()
    {
        if (GetInput.JumpInput.IsPressed() && staminaSystem.CanJump && canJump && coyoteTime > 0) // Handle jumping input
        {
            coyoteTime = 0;
            IsJumping = true; // Mark the player as jumping
            rb.linearVelocity += Mathf.Sqrt(playerJumpHeight * -Physics.gravity.y) * Vector3.up; // Set the jump velocity based on the jump height
            canJump = false;
            IsOnFloor = false;
            staminaSystem.ConsumeStaminaForJump();
            StartCoroutine(JumpCooldownCoroutine());
        }
    }

    IEnumerator JumpCooldownCoroutine()
    {
        yield return new WaitForSeconds(jumpingCooldown);
        canJump = true;
    }

    // Handle actually moving the player
    private void Moving()
    {

        // Handle sliding movement
        if (isSliding)
        {
            Movement = Vector3.Lerp(Movement, Vector3.zero, rb.linearDamping * Time.deltaTime); // Smoothly reduce slide movement over time

            if (Movement.magnitude < 0.1f)
            {
                Movement = Vector3.zero;
            }
        }
        
        else
        {
            // Calculate movement based on input and camera rotation
            Movement = Quaternion.Euler(0, playerCamera.TotalYRot, 0) * new Vector3(GetInput.MoveValue.x * steer, 0, GetInput.MoveValue.y) * playerSpeed;
        }

        // Apply the combined movement to the rigidbody
        rb.linearVelocity = Movement + rb.linearVelocity.y * Vector3.up + externalForces;



        // Smoothly reduce external forces over time
        externalForces = Vector3.Lerp(externalForces, Vector3.zero, rb.linearDamping * Time.deltaTime);

        // Debugging outputs
        // print($"Velocity: {rb.linearVelocity} external: {externalForces} input: {Movement} input magnitude: {Movement.magnitude}");
    }

    private void Sliding()
    {
        if (GetInput.SlideInput.WasPressedThisFrame() && new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).sqrMagnitude != 0)
        {
            playerHitbox.height = playerSlidingHeight;
            isSliding = true;
            model.transform.localPosition = playerSlidingHeight / 2 * Vector3.down; // Adjust model position for sliding
            model.transform.SetLocalPositionAndRotation(playerSlidingHeight / 2 * Vector3.down, Quaternion.Euler(-80, 0, 0)); // Adjust model position for sliding
            playerCamera.transform.localPosition = new(playerCamera.transform.localPosition.x, playerStandingHeight / 2, playerCamera.transform.localPosition.z); // Adjust camera position for crouching
            rb.MovePosition(transform.position - (playerStandingHeight - playerSlidingHeight) / 2 * Vector3.up); // Physically move the player to the crouch position
        }

        else if (isSliding && (!GetInput.SlideInput.IsPressed() || new Vector3(Movement.x, 0, Movement.z).magnitude < 0.5f) && CanUnCrouch())
        {
            rb.MovePosition(transform.position + (playerStandingHeight - playerSlidingHeight) / 2 * Vector3.up); // Physically move the player to the crouch position
            playerHitbox.height = playerStandingHeight;
            isSliding = false;
            model.transform.SetLocalPositionAndRotation(modelStartPosition, Quaternion.Euler(0, 0, 0)); // Return the model to its original position
            playerCamera.transform.localPosition = cameraStartPosition; // Adjust camera position for standing
        }
    }

    private bool CanUnCrouch()
    {
        overlapResults.Clear();
        playerHitboxBounds = new(transform.position + (playerStandingHeight - playerHitbox.height) / 2 * Vector3.up, new(playerHitbox.radius , playerStandingHeight, playerHitbox.radius));
        overlapResults.AddRange(Physics.OverlapCapsule(transform.position - (playerHitbox.height / 2 - playerHitbox.radius) * Vector3.up, transform.position + (playerStandingHeight - playerHitbox.height / 2) * Vector3.up, playerHitbox.radius, layerMask));

        foreach (Collider collision in overlapResults)
        {
            if (playerHitboxBounds.Intersects(collision.bounds))
            {
                return false;
            }
        }

        return true;
    }

    private void Mantle()
    {
        // Calculate the mantle position based on player dimensions and orientation
        mantlePosition = transform.position + (playerStandingHeight / 2 - playerHitbox.radius) * Vector3.up + Quaternion.Euler(0, rb.transform.eulerAngles.y, 0) * (playerHitbox.radius * 2 * Vector3.forward);

        // Check for a ledge and if the player fits on the ledge
        if (GetInput.JumpInput.WasPressedThisFrame() && Physics.Raycast(mantlePosition + Vector3.up * playerStandingHeight / 2, Vector3.down, out RaycastHit hit, (playerStandingHeight + playerHitbox.height) / 2, layerMask) && Physics.OverlapCapsuleNonAlloc(mantlePosition + playerHitbox.radius * Vector3.up, mantlePosition + (playerStandingHeight + playerHitbox.radius) * Vector3.up, playerHitbox.radius, mantleResults, layerMask) == 0)
        {
            // Move the player to the ledge position
            transform.position = hit.point + Vector3.up * playerStandingHeight / 2;
            IsOnFloor = true;
            StartCoroutine(JumpCooldownCoroutine());
        }
    }

    public void AddExternalForces(Vector3 force)
    {
        externalForces += force; // Add external forces to the player's movement
    }
}
