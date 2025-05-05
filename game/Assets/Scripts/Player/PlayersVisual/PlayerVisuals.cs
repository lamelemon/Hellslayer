using UnityEngine;

// The PlayerVisuals class is responsible for managing the visual effects 
// based on the player's movement speed.
public class PlayerVisuals : MonoBehaviour
{
    [Header("Effects Settings")]
    [Range(1f, 179f)] public float baseFov = 95.0f;
    public float fovChangeSpeed = 6.0f;
    public float maxFov = 120.0f;

    [Header("Dependencies")]
    public Camera playerCam;
    public Rigidbody playerRigidbody;

    private void Awake()
    {
        if (playerCam != null)
        {
            playerCam.fieldOfView = baseFov;
        }
    }

    private void Update()
    {
        if (playerRigidbody == null || playerCam == null) return;

        // Ignore Y-axis velocity
        float currentSpeed = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z).magnitude;

        // Make tiny values to 0 and not like 12004234E -21
        if (currentSpeed < 0.01f)
            currentSpeed = 0f;

        //Debug.Log(currentSpeed);
        float targetFov = Mathf.Lerp(baseFov, maxFov, currentSpeed / 10f);
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFov, Time.deltaTime * fovChangeSpeed);
        // use this way make velocity basesd run sound But sound system is right now shittt
    }
}
