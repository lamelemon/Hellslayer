using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 8.5f; // Maximum range to interact with items
    public InputAction pickupAction; // Input Action for picking up items
    public InputAction dropAction; // Input Action for dropping items
    private Camera playerCamera;
    private bool OnHand = false; // Flag to check if the item is on hand

    [SerializeField] private PlayerInputManager getInput;
    [SerializeField] private Transform handPosition; // Reference to the player's hand position

    private TestItem currentlyHeldItem; // Reference to the currently held item

    private void Start()
    {
        // Find the camera tagged as "playerCamera"
        playerCamera = GameObject.FindWithTag("playerCamera")?.GetComponent<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("Camera with tag 'playerCamera' not found. Please ensure your camera is tagged correctly.");
        }

        if (getInput == null)
        {
            Debug.LogError("PlayerInputManager not assigned. Please assign it in the inspector.");
        }

        if (handPosition == null)
        {
            Debug.LogError("Hand position not assigned. Please assign it in the inspector.");
        }
    }

    private void Update()
    {
        if (playerCamera == null)
        {
            return;
        }

        // Perform a raycast from the center of the screen
        Ray ray = playerCamera.ScreenPointToRay(new UnityEngine.Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            // Check if the object hit by the raycast has a TestItem component
            if (hit.collider.TryGetComponent<TestItem>(out TestItem item))
            {
                // Display a message or UI prompt (optional)
                Debug.Log($"Looking at {item.itemName}, press the pickup key to pick up");

                // Check if the player presses the pickup key using GetInput
                // Ensure the player can hold only one item at a time
                if (getInput.PickupInput.WasPressedThisFrame() && currentlyHeldItem == null)
                {
                    MoveItemToHand(item);
                    item.PickupInput(); // Call the pickup method on the item
                }

                else if (getInput.PickupInput.WasPressedThisFrame() && currentlyHeldItem != null)
                {
                    DropItem(); // Drop the item if already holding one
                    MoveItemToHand(item);
                    item.PickupInput(); // Call the pickup method on the item
                }
            }
        }

        // Check if the player presses the drop key
        if (getInput.DropInput.WasPressedThisFrame() && currentlyHeldItem != null)
        {
            DropItem();
        }
    }

    private void MoveItemToHand(TestItem item)
    {
        // Disable the item's physics and move it to the player's hand
        Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true; // Disable physics
        }

        item.transform.SetParent(handPosition); // Parent the item to the hand

        foreach (Collider collider in item.GetComponents<Collider>())
        {
            collider.enabled = false; // Disable the collider to prevent further interactions
        }

        currentlyHeldItem = item; // Set the currently held item
        Debug.Log($"{item.itemName} has been picked up and moved to the hand.");
    }

    private void DropItem()
    {
        if (currentlyHeldItem == null) return;

        // Re-enable the item's physics
        Rigidbody itemRigidbody = currentlyHeldItem.GetComponent<Rigidbody>();
        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = false; // Enable physics
        }

        // Unparent the item from the hand
        currentlyHeldItem.transform.SetParent(null);

        // Re-enable the item's colliders
        foreach (Collider collider in currentlyHeldItem.GetComponents<Collider>())
        {
            collider.enabled = true;
        }

        // Optionally, apply a small force to the item to drop it
        if (itemRigidbody != null)
        {
            itemRigidbody.AddForce(playerCamera.transform.forward * 2f, ForceMode.Impulse);
        }

        Debug.Log($"{currentlyHeldItem.itemName} has been dropped.");
        currentlyHeldItem = null; // Clear the reference to the held item
    }
}