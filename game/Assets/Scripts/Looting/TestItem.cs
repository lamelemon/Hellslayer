using Unity.VisualScripting;
using UnityEngine;

public class TestItem : MonoBehaviour
{
    public string itemName = "TestItem"; // Name of the item
    public int itemValue = 10;           // Value of the item

    public void RotateTo(Vector3 desiredRotation)
    {
        transform.rotation = Quaternion.Euler(desiredRotation);
    }

    // Reference to the hand position where the item will be attached
    [SerializeField] Transform handPosition;
    
    // Called when the item is picked up
    public void PickupInput()
    {
        Debug.Log($"picked up: {itemName}, value: {itemValue}");
        // Add logic to add the item to the player's inventory here
        // Example: InventorySystem.AddItem(this);

        // Destroy the item after picking it up
        // Destroy(gameObject);
        // transform.SetParent(handPosition); // Attach to hand
        // transform.localPosition = Vector3.zero; // Reset positions
        RotateTo(new Vector3(0, 0, 0f));
        // transform.localRotation = Quaternion.identity; // Reset rotation
        GetComponent<Rigidbody>().isKinematic = true; // Disable physics
    }
}
