using UnityEngine;

public class RPG : MonoBehaviour
{
    public string itemName = "rpg";

    [SerializeField] private PlayerInputManager getInput;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform rocketSpawnPoint;
    [SerializeField] private float rocketSpeed = 60f;
    [SerializeField] private Transform playerCamera;

    [SerializeField] private float fireCooldown = 3f; // Cooldown in seconds
    private float lastFireTime = -Mathf.Infinity;

    void Update()
    {
        if (getInput.AttackInput.WasPressedThisFrame() && Time.time >= lastFireTime + fireCooldown)
        {
            FireRocket();
            lastFireTime = Time.time;
        }
    }

    private void FireRocket()
    {
        Debug.Log("Rocket Launched");

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        Vector3 targetPoint;
        Transform lockOnTarget = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            targetPoint = hit.point;

            Transform rootTarget = hit.collider.transform.root;
            if (rootTarget.CompareTag("Enemy"))
            {
                lockOnTarget = rootTarget;
            }
        }
        else
        {
            targetPoint = playerCamera.position + playerCamera.forward * 100f;
        }

        Vector3 direction = (targetPoint - rocketSpawnPoint.position).normalized;
        rocketSpawnPoint.rotation = Quaternion.LookRotation(direction);

        GameObject rocketInstance = Instantiate(rocketPrefab, rocketSpawnPoint.position, rocketSpawnPoint.rotation);

        Rigidbody rb = rocketInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearDamping = 0f;
            rb.linearVelocity = direction * rocketSpeed;
        }

        Rocket rocketScript = rocketInstance.GetComponent<Rocket>();
        if (rocketScript != null && lockOnTarget != null)
        {
            rocketScript.AssignTarget(lockOnTarget);
        }
    }
}
