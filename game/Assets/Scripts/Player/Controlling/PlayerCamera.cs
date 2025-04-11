using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float sensitivity = 0.1f;
    private float totalXRot;
    public float TotalYRot { get; private set; }
    [SerializeField] PlayerInputManager GetInput;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        totalXRot = transform.rotation.x;
        TotalYRot = transform.rotation.y;
    }

    void Update()
    {
        if(PauseMenu.isPaused) // Check if the game is paused
        {
            return; // Skip camera rotation if the game is paused
        }

        if (TotalYRot >= 360f || TotalYRot <= -360f) // prevent y rotation from going too high
        {
            TotalYRot = 0f;
        }

        TotalYRot += GetInput.MouseValue.x * sensitivity;

        totalXRot = Mathf.Clamp(GetInput.MouseValue.y * -sensitivity + totalXRot, -90f, 90);

        transform.localRotation = Quaternion.Euler(totalXRot, 0, 0);
    }

}
