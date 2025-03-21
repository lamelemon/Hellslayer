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
    private CharacterController controller; // References to the CharacterController
    private Vector3 gravity; // For storing the gravity vector
    private float playerSpeed; // Current player speed
    private readonly float playerWalkSpeed = 10.0f; // Walking speed constant
    private readonly float playerRunMultiplier = 1.5f; // Multiplier for running speed
    private readonly float playerCrouchSpeedMultiplier = 0.5f; // Multiplier for crouch speed
    private readonly float playerJumpHeight = 9.0f; // Jump height constant
    private Vector3 playerVelocity; // Stores player movement velocity
    private float playerStandingHeight; // Player height when standing
    private readonly float playerCrouchingHeight = 3.5f; // Player height when crouching
    private Vector3 playerStandingCenter; // Center offset when standing
    private Vector3 playerCrouchingCenter; // Center offset when crouching
    public bool IsCrouching { get; private set; } // Boolean indicating if the player is crouching
    private readonly Collider[] overlapResults = new Collider[10]; // Used for checking collisions when standing up from crouch
    public bool IsJumping { get; private set; } // Boolean indicating if the player is currently jumping
    private Vector3 modelStartPosition; // Store the initial position of the player model (for crouch animations)
    private readonly float maxMomentumChange = 0.01f; // Maximum rate at which velocity can change (for momentum effect)
    
    [SerializeField] PlayerInputManager GetInput; // Reference to the PlayerInputManager for capturing input
    [SerializeField] PlayerCamera PlayerCamera; // Reference to the PlayerCamera to handle camera rotation
    [SerializeField] GameObject model; // Reference to the player model (used for crouching animation)

    private void Awake()
    {
        // Initializing the references
        GetInput = GetComponent<PlayerInputManager>();
        controller = gameObject.GetComponent<CharacterController>();
    }

    private void Start()
    {
        // Set the initial player height and center offsets
        playerStandingHeight = controller.height;
        playerStandingCenter = controller.center;
        playerCrouchingCenter = 1.5f * Vector3.up;
        gravity = Physics.gravity; // Get gravity from the physics engine
        modelStartPosition = model.transform.localPosition; // Store the starting position of the model for crouching
    }

    void Update()
    {
        // Reset the speed to walking speed each frame
        playerSpeed = playerWalkSpeed;

        // Call the methods to handle crouch, velocity, and movement
        Crouch();
        YVelocity();
        Moving();

        // Rotate the player model to face the camera's direction
        controller.transform.rotation = Quaternion.Euler(0, PlayerCamera.transform.eulerAngles.y, 0);

        // random debugging outputs
        print($"{controller.isGrounded} {controller.velocity} {Quaternion.Euler(0, controller.transform.eulerAngles.y, 0) * playerVelocity} FPS: {Mathf.Round(1 / Time.deltaTime)}");
    }

    // Handle crouching behavior
    private void Crouch()
    {
        // If the player presses crouch and isn't jumping, crouch the player
        if (!IsJumping && GetInput.CrouchInput.IsPressed())
        {
            // Adjust controller height for crouching
            controller.height = playerCrouchingHeight;
            controller.center = playerCrouchingCenter; // Adjust the center position for crouching
            model.transform.localPosition = -playerStandingCenter; // Adjust model position for crouching
            IsCrouching = true; // Mark the player as crouching
            controller.Move((playerStandingHeight - playerCrouchingHeight) * Vector3.down - playerStandingCenter); // Physically move the player to the crouch position
        }

        // If the player is crouching and releases crouch input (and enough space above), uncrouch
        if (IsCrouching && Physics.OverlapCapsuleNonAlloc(transform.position + playerStandingCenter, (playerStandingHeight - controller.radius) * Vector3.up + transform.position - playerStandingCenter, controller.radius, overlapResults) <= 1)
        {
            // Physically move the player to the standing position
            controller.Move((playerStandingHeight - playerCrouchingHeight) * Vector3.up + playerStandingCenter);
            model.transform.localPosition = modelStartPosition; // Return the model to its original position
            controller.height = playerStandingHeight; // Restore the standing height
            controller.center = playerStandingCenter; // Restore the standing center
            IsCrouching = false; // Mark the player as not crouching
        }
    }

    // Handle Y-axis velocity (gravity and jumping)
    private void YVelocity()
    {
        // Apply gravity to the player's Y velocity (falling speed)
        playerVelocity.y += gravity.y * Time.deltaTime * 2;

        if (controller.isGrounded) // If the player is on the ground
        {
            playerVelocity.y = Mathf.Max(0, playerVelocity.y); // Prevent negative velocity (no falling when grounded)
            IsJumping = false; // The player is not jumping when on the ground

            if (GetInput.JumpInput.IsPressed()) // Handle jumping input
            {
                IsJumping = true; // Mark the player as jumping
                playerVelocity.y = Mathf.Sqrt(playerJumpHeight * -gravity.y); // Set the jump velocity based on the jump height
            }
        }
    }

    // Handle actually moving the player
    private void Moving()
    {
        if (IsCrouching)
        {
            playerSpeed *= playerCrouchSpeedMultiplier; // Decrease speed when crouching
        }

        else if (GetInput.SprintInput.IsPressed()) // Increase speed when sprinting
        {
            playerSpeed *= playerRunMultiplier;

            /*if (IsJumping)
            {
                playerSpeed *= 1.3f; // slight minecraft reference <Ö>
            }*/
        }

        // Smoothly interpolate between current velocity and target velocity (momentum effect)
        if (GetInput.MoveValue != Vector2.zero)
        {
            playerVelocity = Vector3.Lerp(playerVelocity, new(GetInput.MoveValue.x * playerSpeed, playerVelocity.y, GetInput.MoveValue.y * playerSpeed), maxMomentumChange);
        }

        else
        {
            playerVelocity = Vector3.Lerp(playerVelocity, new(0, playerVelocity.y, 0), maxMomentumChange); // to be changed to maybe sliding
        }

        // Move the player <Ö>
        controller.Move(Quaternion.Euler(0, controller.transform.eulerAngles.y, 0) * playerVelocity * Time.deltaTime);
    }
}
