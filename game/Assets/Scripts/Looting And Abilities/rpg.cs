using UnityEngine;

public class RPG : MonoBehaviour
{
    public string itemName = "rpg";

    [SerializeField] private PlayerInputManager getInput;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform rocketSpawnPoint;
    [SerializeField] private float rocketSpeed = 50f;
    [SerializeField] private Transform playerCamera;

    void Update()
    {
        if (getInput.AttackInput.WasPressedThisFrame())
        {
            FireRocket();
        }
    }

    private void FireRocket()
    {
        Debug.Log("Rocket Launched");

        // Raycast from player camera to find target point
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f)) // Max distance 100 units
        {
            targetPoint = hit.point; // Hit something - aim there
        }
        else
        {
            targetPoint = playerCamera.position + playerCamera.forward * 100f; // Nothing hit - aim far forward
        }

        // Calculate direction from rocket spawn point to target point
        Vector3 direction = (targetPoint - rocketSpawnPoint.position).normalized;

        // Rotate spawn point to face direction
        rocketSpawnPoint.rotation = Quaternion.LookRotation(direction);

        // Instantiate rocket and fire it
        GameObject rocketInstance = Instantiate(rocketPrefab, rocketSpawnPoint.position, rocketSpawnPoint.rotation);

        Rigidbody rb = rocketInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearDamping = 0f;
            rb.linearVelocity = direction * rocketSpeed;
        }
        else
        {
            Debug.LogWarning("Rigidbody missing on rocket prefab!");
        }
    }
}
