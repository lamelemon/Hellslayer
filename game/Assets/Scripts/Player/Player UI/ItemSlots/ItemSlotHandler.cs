using UnityEngine;

public class ItemSlotHandler : MonoBehaviour
{
    [SerializeField] private Transform handPosition; // Reference to the player's hand position
    [SerializeField] private ItemSlotManager itemSlotManager; // Reference to the ItemSlotManager
    [SerializeField] private PlayerInteraction PlayerInteraction; // Reference to the PlayerItemInteraction script
    private TestItem[] itemSlots = new TestItem[2]; // Array to store items in slots
    private GameObject currentWeapon; // Reference to the currently visible weapon
    private int activeSlot = 0; // Currently active slot

    private void Start()
    {
        UpdateHand();
    }

    private void Update()
    {
        if (itemSlots[activeSlot] != null)
        {
            PlayerInteraction.currentlyHeldItem = itemSlots[activeSlot]; // Update the currently held item in PlayerInteraction
        }
        else
        {
            PlayerInteraction.currentlyHeldItem = null; // Clear the currently held item if the slot is empty
        }
    }
    /// <summary>

    public void PickUpItem(TestItem newItem)
    {
        int slotToFill = -1;
        if (itemSlots[activeSlot] == null)
        {
            slotToFill = activeSlot;
        }
        else if (itemSlots[1 - activeSlot] == null)
        {
            slotToFill = 1 - activeSlot;
        }
        else
        {
            PlayerInteraction.DropItem();
            DropItem();
            slotToFill = activeSlot;
        }

        itemSlots[slotToFill] = newItem;
        UpdateItemTransform(newItem);
        UpdateSlotIcons();

        // Only update hand if we picked up into the active slot
        if (slotToFill == activeSlot)
        {
            UpdateHand();
        }
    }

    // Helper method to set the item's position and rotation
    private void UpdateItemTransform(TestItem item)
    {
        if (item == null) return;

        // Set the item's position and rotation
        item.transform.localPosition = item.ItemPosition;
        item.transform.localRotation = item.ItemRotation;

        // Disable physics
        Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = true;
        }
    }

    private void UpdateSlotIcons()
    {
        itemSlotManager.UpdateSlotIcon(1, itemSlots[0]?.itemIcon);
        itemSlotManager.UpdateSlotIcon(2, itemSlots[1]?.itemIcon);
    }

    public void SwitchSlot(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > 2) return;

        activeSlot = slotNumber - 1;
        itemSlotManager.HighlightSlot(slotNumber);

        // Update the item's transform for the active slot
        if (itemSlots[activeSlot] != null)
        {
            UpdateItemTransform(itemSlots[activeSlot]);
        }

        UpdateHand();
    }

    private void UpdateHand()
    {
        // Hide the current weapon and disable its script/UI
        if (currentWeapon != null)
        {
            // Disable weapon script and hide UI
            PlayerInteraction.DisableWeaponScript(currentWeapon);
            currentWeapon.SetActive(false);
        }

        // If the active slot is empty, use fists (no weapon)
        if (itemSlots[activeSlot] == null)
        {
            UseFists();
            currentWeapon = null;
        }
        else
        {
            // Equip the weapon in the active slot
            EquipWeapon(itemSlots[activeSlot]);
            currentWeapon = itemSlots[activeSlot].gameObject;
            currentWeapon.SetActive(true);

            // Only enable weapon script for the equipped item
            PlayerInteraction.EnableWeaponScript(currentWeapon);
        }
    }

    private void EquipWeapon(TestItem item)
    {
        if (item == null) return;

        // If the item is already in the hand, just make it visible
        if (item.transform.parent == handPosition)
        {
            item.gameObject.SetActive(true);
        }
        else
        {
            // Move the item to the hand
            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = true; // Disable physics
            }

            item.transform.SetParent(handPosition); // Parent the item to the hand
            item.transform.localPosition = item.ItemPosition; // Use the item's position
            item.transform.localRotation = item.ItemRotation; // Use the item's rotation

            foreach (Collider collider in item.GetComponents<Collider>())
            {
                collider.enabled = false; // Disable the collider to prevent further interactions
            }

            item.gameObject.SetActive(true); // Make the item visible
        }

        currentWeapon = item.gameObject; // Update the current weapon reference
    }

    private void UseFists()
    {
        // If fists are already equipped, do nothing
        if (currentWeapon == null)
        {
            Debug.Log("Using fists (no weapon equipped).");
        }

        currentWeapon = null; // Clear the current weapon reference
    }

    public void DropItem()
    {
        itemSlots[activeSlot] = null; // Clear the item from the active slot
        currentWeapon = null; // Clear the current weapon reference

        UpdateSlotIcons();
        UpdateHand();
    }
}
