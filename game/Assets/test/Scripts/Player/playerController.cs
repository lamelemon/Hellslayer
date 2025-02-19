using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameObject playerCamera;
    private CharacterController controller;
    private Vector3 gravity;
    private Vector3 playerVelocity;
    private float playerSpeed;
    private float playerWalkSpeed = 10.0f;
    private float playerRunSpeed = 15.0f;
    private float playerJumpHeight = 10.0f;
    private bool isOnFloor;
    private float playerCameraY;
    [SerializeField] PlayerInputSubscription GetInput;

    private void Awake()
    {
        GetInput = GetComponent<PlayerInputSubscription>();
        playerCamera = GameObject.FindGameObjectWithTag("playerCamera");
        controller = gameObject.AddComponent<CharacterController>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        controller.height = 4.8f;
        gravity = Physics.gravity;
    }

    // Update is called once per frame
    void Update()
    {
        playerCameraY = playerCamera.transform.eulerAngles.y;
        isOnFloor = controller.isGrounded;
        print(isOnFloor);

        if (GetInput.SprintInput)  // replace with check for the correct button
        {
            playerSpeed = playerRunSpeed;
        }
        else
        {
            playerSpeed = playerWalkSpeed;
        }

        if (isOnFloor && GetInput.JumpInput) // Handle jumping
        {
            playerVelocity.y += Mathf.Sqrt(playerJumpHeight * -gravity.y);
//            print(Mathf.Sqrt(playerJumpHeight * -2f * gravity.y));
        }
        else if (!isOnFloor)  // Apply gravity
        {
            playerVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = new Vector3(GetInput.MoveInput.x * playerSpeed, playerVelocity.y, GetInput.MoveInput.y * playerSpeed);
        move = Quaternion.Euler(0, playerCameraY, 0) * move;
        controller.transform.rotation = Quaternion.Euler(0, playerCameraY, 0);
        print(move);
        controller.Move(move * Time.deltaTime);
    }
}
