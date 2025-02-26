using System;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameObject playerCamera;
    private CharacterController controller;
    private Vector3 gravity;
    private float playerVelocityY;
    private float playerSpeed;
    private readonly float playerWalkSpeed = 10.0f;
    private readonly float playerRunSpeed = 15.0f;
    private readonly float playerCrouchSpeed = 5.0f;
    private readonly float playerJumpHeight = 10.0f;
    private float playerStandingHeight;
    private readonly float playerCrouchingHeight = 3.5f;
    private Vector3 playerStandingCenter;
    private Vector3 playerCrouchingCenter;
    public bool isCrouching;
    private bool canUnCrouch = true;
    private bool isOnFloor;
    private float playerCameraY;
    [SerializeField] PlayerInputSubscription GetInput;

    private void Awake()
    {
        GetInput = GetComponent<PlayerInputSubscription>();
        playerCamera = GameObject.FindGameObjectWithTag("playerCamera");
        controller = gameObject.GetComponent<CharacterController>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        playerStandingHeight = controller.height;
        playerStandingCenter = controller.center;
        playerCrouchingCenter = 1.5f * Vector3.up;
        gravity = Physics.gravity;
    }

    // Update is called once per frame
    void Update()
    {
        playerCameraY = playerCamera.transform.eulerAngles.y;
        isOnFloor = controller.isGrounded;

        if (isOnFloor)
        {
            playerVelocityY = 0;
            if (GetInput.JumpInput) // Handle jumping
            {
                playerVelocityY = Mathf.Sqrt(playerJumpHeight * -gravity.y);
            }
        }
        else // Apply gravity
        {
            playerVelocityY += gravity.y * Time.deltaTime * 2;
        }

        Crouch();

        if (controller.height == playerStandingHeight)
        {
            if (GetInput.SprintInput)
            {
                playerSpeed = playerRunSpeed;
            }
            else
            {
                playerSpeed = playerWalkSpeed;
            }
        }


        // Player movement direction & velocity
        Vector3 move = new Vector3(GetInput.MoveInput.x * playerSpeed, playerVelocityY, GetInput.MoveInput.y * playerSpeed);
        move = Quaternion.Euler(0, playerCameraY, 0) * move; // Calculate movement direction
        controller.transform.rotation = Quaternion.Euler(0, playerCameraY, 0); // Turn player model
        controller.Move(move * Time.deltaTime); // Move the player <Ö>
    }

    private void Crouch()
    {
        if (controller.height == playerCrouchingHeight && controller.center == playerCrouchingCenter)
        {
            OverLap();
        }
        if (GetInput.CrouchInput)
        {
            playerSpeed = playerCrouchSpeed; 
            controller.height = playerCrouchingHeight; // Make the player crouch
            controller.center = playerCrouchingCenter;
            isCrouching = true; // Gee. I wonder what this does?
        }
        else if (isCrouching && canUnCrouch)
        {   // Return player to standing
            controller.height = controller.height > playerStandingHeight - 0.1f ? controller.height = playerStandingHeight : Mathf.Lerp(controller.height, playerStandingHeight, playerStandingHeight / 200);
            controller.center = controller.center.y < playerStandingCenter.y + 0.05f ? controller.center = playerStandingCenter : Vector3.Lerp(controller.center, playerStandingCenter, playerCrouchingCenter.y / 100);

            if (controller.height == playerStandingHeight && controller.center.y == playerStandingCenter.y)
            {
                isCrouching = false;
            }
            else if (controller.height > playerStandingHeight || controller.center.y < playerStandingCenter.y)
            {
                controller.height = playerStandingHeight;
                controller.center = playerStandingCenter;
                isCrouching = false;
            }

        }
    }
    private void OverLap()
    {
        canUnCrouch = Physics.OverlapCapsule(transform.position + playerStandingCenter, transform.position + playerStandingCenter + playerStandingHeight * Vector3.up, controller.radius).Length <= 1;

    }
}