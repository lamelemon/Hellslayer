using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb; // References to the CharacterController
    private BoxCollider boxCollider;
    private Vector3 gravity; // For storing the gravity vector
    private float playerSpeed; // Current player speed
    private readonly float playerWalkSpeed = 10.0f; // Walking speed constant
    private readonly float playerRunMultiplier = 1.5f; // Multiplier for running speed
    private readonly float playerCrouchSpeedMultiplier = 0.5f; // Multiplier for crouch speed
    private readonly float playerJumpHeight = 7.0f; // Jump height constant
    private Vector3 playerStandingHeight; // Player height when standing
    private Vector3 playerCrouchingHeight; // Player height when crouching
    private Vector3 playerStandingCenter; // Center offset when standing
    private Vector3 playerCrouchingCenter; // Center offset when crouching
    public bool IsCrouching { get; private set; } // Boolean indicating if the player is crouching
    private readonly Collider[] overlapResults = new Collider[10]; // Used for checking collisions when standing up from crouch
    public bool IsJumping { get; private set; } // Boolean indicating if the player is currently jumping
    public bool IsOnFloor { get; private set; }
    private Vector3 modelStartPosition; // Store the initial position of the player model (for crouch animations)
    
    [SerializeField] PlayerInputManager GetInput; // Reference to the PlayerInputManager for capturing input
    [SerializeField] PlayerCamera PlayerCamera; // Reference to the PlayerCamera to handle camera rotation
    [SerializeField] GameObject model; // Reference to the player model (used for crouching animation)

    private void Awake()
    {
        // Initializing the references
        GetInput = GetComponent<PlayerInputManager>();
        rb = gameObject.GetComponent<Rigidbody>();
        boxCollider = GameObject.FindGameObjectWithTag("PlayerHitbox").GetComponent<BoxCollider>();
    }

    private void Start()
    {
        // Set the initial player height and center offsets
        playerStandingHeight = boxCollider.size;
        playerStandingCenter = boxCollider.center;
        playerCrouchingHeight = new(boxCollider.size.x, 3.5f, boxCollider.size.z);
        playerCrouchingCenter = 1.5f * Vector3.up;
        gravity = Physics.gravity; // Get gravity from the physics engine
        modelStartPosition = model.transform.localPosition; // Store the starting position of the model for crouching
    }

    void Update()
    {
        Crouch();
        Jumping();

        // Rotate the player model to face the camera's direction
        model.transform.rotation = Quaternion.Euler(0, PlayerCamera.TotalYRot, 0);

        // random debugging outputs
        //print($"{isOnFloor} {rb.linearVelocity} FPS: {Mathf.Round(1 / Time.deltaTime)}");
    }

    void FixedUpdate()
    {
        IsOnFloor = Physics.OverlapSphereNonAlloc(transform.position - boxCollider.center - (playerStandingHeight.y - playerCrouchingHeight.y) * Vector3.up, boxCollider.size.x, overlapResults) > 1;
        playerSpeed = playerWalkSpeed;

        Moving();
    }

    // Handle crouching behavior
    private void Crouch()
    {
        // If the player presses crouch and isn't jumping, crouch the player
        if (IsOnFloor && !IsCrouching && GetInput.CrouchInput.IsPressed())
        {
            // Adjust controller height for crouching
            boxCollider.size = playerCrouchingHeight;
            boxCollider.center = playerCrouchingCenter; // Adjust the center position for crouching
            model.transform.localPosition = -playerStandingCenter; // Adjust model position for crouching
            playerSpeed *= playerCrouchSpeedMultiplier;
            IsCrouching = true; // Mark the player as crouching
            rb.MovePosition(rb.position - playerStandingHeight + playerCrouchingHeight); // Physically move the player to the crouch position
        }

        // If the player is crouching and releases crouch input (and enough space above), uncrouch
        else if (IsCrouching && !GetInput.CrouchInput.IsPressed() && Physics.OverlapBoxNonAlloc(rb.position + 2.5f * Vector3.up, playerStandingHeight, overlapResults) < 3)
        {
            // Physically move the player to the standing position
            rb.MovePosition(rb.position + playerStandingHeight - playerCrouchingHeight);
            model.transform.localPosition = modelStartPosition; // Return the model to its original position
            boxCollider.size = playerStandingHeight; // Restore the standing height
            boxCollider.center = playerStandingCenter; // Restore the standing center
            IsCrouching = false; // Mark the player as not crouching
        }
    }

    // Handle Y-axis velocity (gravity and jumping)
    private void Jumping()
    {
        if (IsOnFloor) // If the player is on the ground
        {
            IsJumping = false; // The player is not jumping when on the ground

            if (!IsCrouching && GetInput.JumpInput.WasPressedThisFrame()) // Handle jumping input
            {
                IsJumping = true; // Mark the player as jumping
                rb.linearVelocity += Mathf.Sqrt(playerJumpHeight * -gravity.y) * Vector3.up; // Set the jump velocity based on the jump height
            }
        }
    }

    // Handle actually moving the player
    private void Moving()
    {
        if (IsCrouching)
        {
            playerSpeed *= playerCrouchSpeedMultiplier;
        }
        else if (GetInput.SprintInput.IsPressed()) // Increase speed when sprinting
        {
            playerSpeed *= playerRunMultiplier;
        }


        // Move the player <Ö>
        rb.linearVelocity = rb.linearVelocity.y * Vector3.up + Quaternion.Euler(0, PlayerCamera.TotalYRot, 0) * new Vector3(GetInput.MoveValue.x * playerSpeed, 0, GetInput.MoveValue.y * playerSpeed);
    }
}
