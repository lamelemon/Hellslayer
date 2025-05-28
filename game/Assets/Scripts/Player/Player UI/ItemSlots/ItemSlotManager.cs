using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro support

public class ItemSlotManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager getInput; // Reference to the input manager
    [SerializeField] private GameObject itemSlot1; // Reference to the first item slot
    [SerializeField] private GameObject itemSlot2; // Reference to the second item slot
    [SerializeField] private Color highlightColor = Color.yellow; // Color for highlighting
    [SerializeField] private Color defaultColor = Color.white; // Default color
    [SerializeField] private Image itemSlot1Icon; // Icon for the first item slot
    [SerializeField] private Image itemSlot2Icon; // Icon for the second item slot
    [SerializeField] private TMP_Text itemSlot1AmmoText; // Ammo text for the first item slot
    [SerializeField] private TMP_Text itemSlot2AmmoText; // Ammo text for the second item slot

    private Image itemSlot1Image;
    private Image itemSlot2Image;

    private void Start()
    {
        itemSlot1Icon.gameObject.SetActive(false); // Hide the icon for slot 1
        itemSlot2Icon.gameObject.SetActive(false); // Hide the icon for slot 2
        itemSlot1AmmoText.gameObject.SetActive(false); // Hide the ammo text for slot 1
        itemSlot2AmmoText.gameObject.SetActive(false); // Hide the ammo text for slot 2

        // Get the Image components of the item slots
        itemSlot1Image = itemSlot1.GetComponent<Image>();
        itemSlot2Image = itemSlot2.GetComponent<Image>();

        // Set the default highlight
        HighlightSlot(1);
    }

    private void Update()
    {
        if (getInput.ItemSlot1Input.WasPressedThisFrame())
        {
            HighlightSlot(1);
        }
        else if (getInput.ItemSlot2Input.WasPressedThisFrame())
        {
            HighlightSlot(2);
        }
    }

    public void HighlightSlot(int slotNumber)
    {
        // Highlight the selected slot and reset the other
        if (slotNumber == 1)
        {
            itemSlot1Image.color = highlightColor;
            itemSlot2Image.color = defaultColor;
        }
        else if (slotNumber == 2)
        {
            itemSlot1Image.color = defaultColor;
            itemSlot2Image.color = highlightColor;
        }
    }

    public void UpdateSlotIcon(int slotNumber, Sprite icon, string ammoText = null)
    {
        if (slotNumber == 1)
        {
            itemSlot1Icon.gameObject.SetActive(true); // Show the icon for slot 1
            itemSlot1Icon.sprite = icon;
            itemSlot1Icon.enabled = icon != null; // Hide if no icon

            if (!string.IsNullOrEmpty(ammoText))
            {
                itemSlot1AmmoText.gameObject.SetActive(true); // Show the ammo text
                itemSlot1AmmoText.text = ammoText;
            }
            else
            {
                itemSlot1AmmoText.gameObject.SetActive(false); // Hide the ammo text
            }
        }
        else if (slotNumber == 2)
        {
            itemSlot2Icon.gameObject.SetActive(true); // Show the icon for slot 2
            itemSlot2Icon.sprite = icon;
            itemSlot2Icon.enabled = icon != null; // Hide if no icon

            if (!string.IsNullOrEmpty(ammoText))
            {
                itemSlot2AmmoText.gameObject.SetActive(true); // Show the ammo text
                itemSlot2AmmoText.text = ammoText;
            }
            else
            {
                itemSlot2AmmoText.gameObject.SetActive(false); // Hide the ammo text
            }
        }
    }
}
