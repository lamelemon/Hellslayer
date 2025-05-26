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
        // Initialize the player's hand with the currently active slot
        UpdateHand();
    }

    private void Update()
    {
        // Update the currently held item in PlayerInteraction based on the active slot
        if (itemSlots[activeSlot] != null)
        {
            PlayerInteraction.currentlyHeldItem = itemSlots[activeSlot];
        }
        else
        {
            PlayerInteraction.currentlyHeldItem = null; // Clear the held item if the slot is empty
        }
    }

    public void PickUpItem(TestItem newItem)
    {
        int slotToFill = -1;

        // Determine which slot to fill
        if (itemSlots[activeSlot] == null)
        {
            slotToFill = activeSlot; // Fill the active slot if it's empty
        }
        else if (itemSlots[1 - activeSlot] == null)
        {
            slotToFill = 1 - activeSlot; // Fill the other slot if it's empty
        }
        else
        {
            // If both slots are full, drop the currently held item
            PlayerInteraction.DropItem();
            DropItem();
            slotToFill = activeSlot; // Replace the item in the active slot
        }

        // Assign the new item to the determined slot
        itemSlots[slotToFill] = newItem;

        // Update the item's position and rotation
        UpdateItemTransform(newItem);

        // Update the UI to reflect the new item
        UpdateSlotIcons();

        // Update the player's hand if the new item is in the active slot
        if (slotToFill == activeSlot)
        {
            UpdateHand();
        }
        else
        {
            // Ensure the ammo UI is updated for the inactive slot
            UpdateAmmoForInactiveSlot(slotToFill);
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

    public void UpdateSlotIcons()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            TestItem item = itemSlots[i];
            if (item != null)
            {
                // Check if the item is a Laser Desert Eagle
                LaserDesertEagle laserWeapon = item.GetComponent<LaserDesertEagle>();
                if (laserWeapon != null)
                {
                    // Update the slot icon with ammo information
                    itemSlotManager.UpdateSlotIcon(
                        i + 1,
                        item.itemIcon, // Pass the item's icon (Sprite)
                        $"{laserWeapon.CurrentAmmo}/{laserWeapon.MaxAmmo}" // Pass the ammo text
                    );
                }
                else
                {
                    // Update the slot icon without ammo information
                    itemSlotManager.UpdateSlotIcon(i + 1, item.itemIcon, null);
                }
            }
            else
            {
                // Clear the slot icon if no item is present
                itemSlotManager.UpdateSlotIcon(i + 1, null, null);
            }
        }
    }

    public void SwitchSlot(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > 2) return;

        activeSlot = slotNumber - 1; // Convert slot number (1 or 2) to index (0 or 1)
        itemSlotManager.HighlightSlot(slotNumber); // Highlight the selected slot in the UI

        // Update the item's transform for the active slot
        if (itemSlots[activeSlot] != null)
        {
            UpdateItemTransform(itemSlots[activeSlot]);
        }

        // Update the player's hand to reflect the active slot
        UpdateHand();
    }

    private void UpdateHand()
    {
        // Hide the current weapon and disable its script/UI
        if (currentWeapon != null)
        {
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
            PlayerInteraction.EnableWeaponScript(currentWeapon);
        }

        // Update slot icons to reflect the current ammo
        UpdateSlotIcons();
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

        // Update the slot icons to reflect the current ammo
        UpdateSlotIcons();
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

    // Helper method to update ammo for the inactive slot
    private void UpdateAmmoForInactiveSlot(int slotIndex)
    {
        TestItem item = itemSlots[slotIndex];
        if (item == null) return;

        LaserDesertEagle laserWeapon = item.GetComponent<LaserDesertEagle>();
        if (laserWeapon != null)
        {
            // Update the ammo UI for the inactive slot
            itemSlotManager.UpdateSlotIcon(
                slotIndex + 1,
                item.itemIcon,
                $"{laserWeapon.CurrentAmmo}/{laserWeapon.MaxAmmo}"
            );
        }
    }
}
