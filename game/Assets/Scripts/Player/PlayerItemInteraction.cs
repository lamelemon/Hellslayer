using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 8.5f; 
    public InputAction pickupAction;
    public InputAction dropAction;
    private Camera playerCamera;
    private bool OnHand = false;

    [SerializeField] private PlayerInputManager getInput;
    [SerializeField] private Transform handPosition;
    [SerializeField] private ItemSlotHandler itemSlotHandler;
    [SerializeField] private string crimson_sword = "crimson_sword";
    [SerializeField] private string katana = "katana";
    [SerializeField] private string rpg = "rpg";
    [SerializeField] private GameObject arms;

    public TestItem currentlyHeldItem;

    private void Start()
    {
        playerCamera = GameObject.FindWithTag("playerCamera")?.GetComponent<Camera>();
        if (playerCamera == null)
            Debug.LogError("Camera with tag 'playerCamera' not found.");
        if (getInput == null)
            Debug.LogError("PlayerInputManager not assigned.");
        if (handPosition == null)
            Debug.LogError("Hand position not assigned.");
    }

    private void Update()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            if (hit.collider.TryGetComponent<TestItem>(out TestItem item))
            {
                Debug.Log($"Looking at {item.itemName}, press pickup key");

                if (getInput.PickupInput.WasPressedThisFrame())
                {
                    
                    itemSlotHandler.PickUpItem(item);
                    item.PickupInput();
                }
            }
        }

        if (getInput.DropInput.WasPressedThisFrame() && currentlyHeldItem != null)
        {
            DropItem();
            itemSlotHandler.DropItem();
        }

        if (getInput.ItemSlot1Input.WasPressedThisFrame())
        {
            itemSlotHandler.SwitchSlot(1);
        }
        else if (getInput.ItemSlot2Input.WasPressedThisFrame())
        {
            itemSlotHandler.SwitchSlot(2);
        }
    }

    private void MoveItemToHand(TestItem item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        item.transform.SetParent(handPosition);

        foreach (Collider collider in item.GetComponents<Collider>())
            collider.enabled = false;

        currentlyHeldItem = item;
        Debug.Log($"{item.itemName} has been picked up and moved to the hand.");
    }

    public void DropItem()
    {
        if (currentlyHeldItem == null) return;

        Rigidbody rb = currentlyHeldItem.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;

        currentlyHeldItem.transform.SetParent(null);

        foreach (Collider collider in currentlyHeldItem.GetComponents<Collider>())
            collider.enabled = true;

        if (rb != null)
            rb.AddForce(playerCamera.transform.forward * 2f, ForceMode.Impulse);

        Debug.Log($"{currentlyHeldItem.itemName} has been dropped.");

        // Disable weapon scripts on dropped item
        DisableWeaponScript(currentlyHeldItem.gameObject);

        currentlyHeldItem = null;
    }

public void EnableWeaponScript(GameObject itemObject)
{
    // Disable all other RPG and LaserDesertEagle scripts on other items
    RPG[] allRPGs = Object.FindObjectsByType<RPG>(FindObjectsSortMode.None);
    foreach (RPG rpgScript in allRPGs)
    {
        rpgScript.enabled = false;
    }

    LaserDesertEagle[] allLasers = Object.FindObjectsByType<LaserDesertEagle>(FindObjectsSortMode.None);
    foreach (LaserDesertEagle laser in allLasers)
    {
        laser.enabled = false;
        laser.SetEquipped(false); // Hide ammo and cancel reload for all
    }

    // Enable RPG script if present
    RPG thisRPG = itemObject.GetComponent<RPG>();
    if (thisRPG != null)
    {
        thisRPG.enabled = true;
        Debug.Log("Enabled RPG script on equipped item.");
    }

    // Enable LaserDesertEagle script if present
    LaserDesertEagle thisLaser = itemObject.GetComponent<LaserDesertEagle>();
    if (thisLaser != null)
    {
        thisLaser.enabled = true;
        thisLaser.SetEquipped(true); // Show ammo for equipped weapon
        Debug.Log("Enabled LaserDesertEagle script on equipped item.");
    }
}

public void DisableWeaponScript(GameObject itemObject)
{
    RPG rpgScript = itemObject.GetComponent<RPG>();
    if (rpgScript != null)
    {
        rpgScript.enabled = false;
        Debug.Log("Disabled RPG script on dropped item.");
    }

    LaserDesertEagle laser = itemObject.GetComponent<LaserDesertEagle>();
    if (laser != null)
    {
        laser.enabled = false;
        laser.SetEquipped(false); // Hide ammo and cancel reload
        Debug.Log("Disabled LaserDesertEagle script on dropped item.");
    }
}


}
