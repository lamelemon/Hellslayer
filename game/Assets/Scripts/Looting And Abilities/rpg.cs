using UnityEngine;

public class RPG : MonoBehaviour
{
    public string itemName = "rpg";
    [SerializeField] private PlayerInputManager getInput;
    [SerializeField] private GameObject rocketPrefab;      // Assign your Rocket prefab in the Inspector
    [SerializeField] private Transform rocketSpawnPoint;   // Assign the RPG barrel or muzzle
    [SerializeField] private float rocketSpeed = 50f;      // Speed of the rocket
    [SerializeField] private PlayerInteraction PlayerItemInteraction;     // Reference to the currently held item
    void Update()
    {
        if (getInput.AttackInput.WasPressedThisFrame())
        {
            // Check if the currently held item is a rocket launcher
            if (PlayerItemInteraction.currentlyHeldItem.itemName == "rpg")
            {
                FireRocket();
            }
        }
        // {
        //     FireRocket();
        //     Debug.Log(PlayerItemInteraction.currentlyHeldItem);
        // }
    }

    private void FireRocket()
    {
        Debug.Log("Rocket Launched");

        GameObject rocketInstance = Instantiate(rocketPrefab, rocketSpawnPoint.position, rocketSpawnPoint.rotation);

        Rigidbody rb = rocketInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = rocketSpawnPoint.forward * rocketSpeed;
        }
    }
}
