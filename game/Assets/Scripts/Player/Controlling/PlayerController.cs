using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    private readonly float standingFloorHeight = -1.8f;
    private Vector3 inputMovement;
    private Vector3 externalForces;

    // Jumping
    public float playerJumpHeight = 7.0f; // Jump height constant
    private readonly float maxCoyoteTime = 0.2f;
    private float coyoteTime;
    public bool IsJumping { get; private set; } // Boolean indicating if the player is currently jumping
    private readonly float jumpingCooldown = 0.25f; // jump cooldown in milliseconds
    private bool canJump = true;

    // Crouching
    private float playerStandingHeight; // Player height when standing
    private readonly float playerCrouchingHeight = 3.5f; // Player height when crouching
    private Vector3 playerStandingCenter; // Center offset when standing
    private Vector3 playerCrouchingCenter = new(0, 1.5f, 0); // Center offset when crouching
    public bool IsCrouching { get; private set; } // Boolean indicating if the player is crouching
    private readonly float crouchingFloorHeight = 0.6f;

    // Sliding
    private readonly float playerSlidingHeight = 2.5f;
    private Vector3 playerSlidingCenter;
    private bool isSliding;
    private readonly float slidingFloorHeight;

    // Buffers
    private readonly Collider[] overlapResults = new Collider[10]; // Used for checking collisions when standing up from crouch
    public bool IsOnFloor { get; private set; } 
    private readonly List<Collider> floorContacts = new();
    private float floorHeight;

    // Model position
    private Vector3 modelStartPosition; // Store the initial position of the player model (for crouch animations)
    private readonly float modelSlidingAngle = -70f;
    
    // Serializations
    [SerializeField] PlayerInputManager GetInput; // Reference to the PlayerInputManager for capturing input
    [SerializeField] PlayerCamera PlayerCamera; // Reference to the PlayerCamera to handle camera rotation
    [SerializeField] GameObject model; // Reference to the player model (used for crouching animation)

    private void Awake()
    {
        // Initializing the references
        GetInput = GetComponent<PlayerInputManager>();
        rb = gameObject.GetComponent<Rigidbody>();
        playerHitbox = GameObject.FindGameObjectWithTag("PlayerHitbox").GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        // Set the initial player height and center offsets
        playerStandingHeight = playerHitbox.height;
        playerStandingCenter = playerHitbox.center;
        modelStartPosition = model.transform.localPosition; // Store the starting position of the model for crouching
        coyoteTime = maxCoyoteTime;
        floorHeight = standingFloorHeight;
    }

    void Update()
    {
        print(IsOnFloor);
        if (IsOnFloor)
        {
            steer = 1;
            coyoteTime = maxCoyoteTime;
            IsJumping = false;
        }

        else
        {
            coyoteTime -= Time.deltaTime;
            steer = aerialsteer;
        }

        Crouch();
        Sliding();

        // Rotate the player model to face the camera's direction
        rb.rotation = Quaternion.Euler(0, PlayerCamera.TotalYRot, 0);

        // random debugging outputs
        //print($"{isOnFloor} {rb.linearVelocity} FPS: {Mathf.Round(1 / Time.deltaTime)}");
    }

    void FixedUpdate()
    {
        playerSpeed = playerWalkSpeed;

        Jumping();
        Moving();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.GetContact(0).point.y < transform.position.y - playerHitbox.height / 2 + playerHitbox.center.y + playerHitbox.radius)
        {
            IsOnFloor = true;
            floorContacts.Add(collision.collider);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (floorContacts.Contains(collision.collider))
        {
            floorContacts.Remove(collision.collider);
        }

        IsOnFloor = floorContacts.Count != 0;
    }


    // Handle crouching behavior
    private void Crouch()
    {
        if (IsOnFloor && !isSliding && GetInput.CrouchInput.WasPressedThisFrame())
        {
            // Adjust controller height for crouching
            playerHitbox.height = playerCrouchingHeight;
            playerHitbox.center = playerCrouchingCenter; // Adjust the center position for crouching
            model.transform.localPosition = -playerStandingCenter; // Adjust model position for crouching
            playerSpeed *= playerCrouchSpeedMultiplier;
            floorHeight = crouchingFloorHeight;
            IsCrouching = true; // Mark the player as crouching
            rb.MovePosition(rb.position - (playerStandingHeight - playerCrouchingHeight - 0.05f) * Vector3.up - playerStandingCenter); // Physically move the player to the crouch position
        }

        else if (IsCrouching && !GetInput.CrouchInput.IsPressed() && Physics.OverlapCapsuleNonAlloc(rb.position + 2.45f * Vector3.up, rb.position + (2.45f + playerStandingHeight) * Vector3.up, playerHitbox.radius, overlapResults) < 3)
        {
            // Physically move the player to the standing position
            rb.MovePosition(rb.position + (playerStandingHeight - playerCrouchingHeight - 0.05f) * Vector3.up + playerStandingCenter);
            model.transform.localPosition = modelStartPosition; // Return the model to its original position
            playerHitbox.height = playerStandingHeight; // Restore the standing height
            playerHitbox.center = playerStandingCenter; // Restore the standing center
            floorHeight = standingFloorHeight;
            IsCrouching = false; // Mark the player as not crouching
        }
    }

    private void Jumping()
    {
        if (!IsCrouching && canJump && coyoteTime > 0 && GetInput.JumpInput.IsPressed()) // Handle jumping input
        {
            coyoteTime = 0;
            IsJumping = true; // Mark the player as jumping
            rb.linearVelocity += playerJumpHeight * Vector3.up; // Set the jump velocity based on the jump height
            canJump = false;
            StartCoroutine(JumpCooldownCoroutine());
        }
    }

    // Handle actually moving the player
    private void Moving()
    {
        if (!isSliding && IsCrouching)
        {
            playerSpeed *= playerCrouchSpeedMultiplier;
        }

        else if (!isSliding && GetInput.SprintInput.IsPressed())
        {
            playerSpeed *= playerRunMultiplier;
        }
        
        if (!isSliding)
        {
            inputMovement = Quaternion.Euler(0, PlayerCamera.TotalYRot, 0) * new Vector3(GetInput.MoveValue.x * steer, 0, GetInput.MoveValue.y) * playerSpeed;
        }

        rb.linearVelocity = new Vector3(inputMovement.x, rb.linearVelocity.y, inputMovement.z) + externalForces;
        externalForces = Vector3.Lerp(externalForces, Vector3.zero, rb.linearDamping);

        //print($"Velocity: {rb.linearVelocity} external: {externalForces} input: {inputMovement}");
    }

    private void Sliding()
    {
        if (GetInput.SlideInput.WasPressedThisFrame() && rb.linearVelocity.magnitude != 0)
        {
            playerHitbox.height = playerSlidingHeight;
            playerHitbox.center = playerSlidingCenter;
            model.transform.localPosition += playerCrouchingCenter;
            isSliding = true;
            floorHeight = slidingFloorHeight;
            model.transform.localRotation = Quaternion.Euler(modelSlidingAngle, 0, 0);

        }

        else if (isSliding && (!GetInput.SlideInput.IsPressed() || rb.linearVelocity.magnitude == 0))
        {
            floorHeight = standingFloorHeight;
            playerHitbox.height = playerStandingHeight;
            playerHitbox.center = playerStandingCenter;
            model.transform.SetLocalPositionAndRotation(modelStartPosition, Quaternion.Euler(Vector3.zero));
            isSliding = false;
        }
    }

    IEnumerator JumpCooldownCoroutine()
    {
        yield return new WaitForSeconds(jumpingCooldown);
        canJump = true;
    }
}
