using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour // This is part of the player finite StateMachine
{
    public float sensX;
    public float sensY;
    public Transform orientation;

    float xRotation;
    float yRotation;
    private Mouse mouse;

    private void Start()
    {
        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize the mouse input from the Input System
        mouse = Mouse.current;
    }

    private void Update()
    {
        if(PauseMenu.isPaused) // Check if the game is paused
        {
            return; // Skip camera rotation if the game is paused
        }
        // Get mouse input using the new Input System
        float mouseX = mouse.delta.x.ReadValue() * Time.fixedDeltaTime * sensX;
        float mouseY = mouse.delta.y.ReadValue() * Time.fixedDeltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit vertical rotation (up/down)

        // Rotate camera and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0); // Rotate only the orientation (not the camera)
    }
}
