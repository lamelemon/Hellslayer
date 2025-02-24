using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform PlayerCharacter;
    public float sensitivity = 1f;
    private float totalXRot;
    private float totalYRot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        totalYRot += Input.GetAxis("Mouse X") * sensitivity;
        totalXRot = Mathf.Clamp(totalXRot + Input.GetAxis("Mouse Y") * -sensitivity, -90f, 90f);

        transform.rotation = Quaternion.Euler(totalXRot, totalYRot, 0);
    }
}
