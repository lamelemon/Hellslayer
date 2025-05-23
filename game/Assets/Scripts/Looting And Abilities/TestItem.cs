using Unity.VisualScripting;
using UnityEngine;

public class TestItem : MonoBehaviour
{
    public string itemName = "TestItem"; // Name of the item
    public int itemValue = 10;           // Value of the item
    public Sprite itemIcon; // Icon for the item
    
    public int damageValue = 20;         // Damage the item can deal
    public float attackRange = 10f;      // Range of the melee attack
    public LayerMask enemyLayer;         // Layer for enemies

    public Quaternion ItemRotation = Quaternion.Euler(0, 0, 0); // Rotation of the item
    public Vector3 ItemPosition = new(0, 0, 0); // Position of the item

    [SerializeField] Transform handPosition; // Reference to the hand position where the item will be attached
    // Reference to the Crimson Sword
    private Camera playerCamera; // Reference to the player's camera

    private void Awake()
    {
        enemyLayer = LayerMask.GetMask("enemyLayer");
        playerCamera = GameObject.FindWithTag("playerCamera")?.GetComponent<Camera>();   
    }

    private void Start()
    {
        // Find the camera tagged as "playerCamera"
        if (playerCamera == null)
        {
            Debug.LogError("Camera with tag 'playerCamera' not found. Please ensure your camera is tagged correctly.");
        }
    }

    public void RotateTo(Vector3 desiredRotation)
    {
        transform.rotation = Quaternion.Euler(desiredRotation);
    }

    // Called when the item is picked up
    public void PickupInput()
    {
        Debug.Log($"Picked up: {itemName}, value: {itemValue}");
        transform.SetLocalPositionAndRotation(ItemPosition, ItemRotation);
        GetComponent<Rigidbody>().isKinematic = true; // Disable physics
    }

    // Method to perform a melee attack
    public void PerformAttack()
    {
        // Ensure the camera is assigned before proceeding
        if (playerCamera == null)
        {
            Debug.LogError("Player camera is not assigned. Cannot perform attack.");
            return;
        }

        // Perform a raycast from the center of the screen
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyLayer))
        {
            // Check if the hit object has an EnemyHealth component
            if (hit.collider.GetComponentInParent<EnemyHealth>() is EnemyHealth enemyHealth)
            {
                enemyHealth.TakeDamage(damageValue); // Deal damage to the enemy
                Debug.Log($"{itemName} attacked {hit.collider.gameObject.name} for {damageValue} damage.");
            }
            else
            {
                Debug.LogError("EnemyHealth component is missing on the target.");
            }
        }
        else
        {
            Debug.Log("No enemy in range to attack.");
        }
    }
}

public interface ILootable
{
    void ItemPickedUp();
    void ItemDropped();
}