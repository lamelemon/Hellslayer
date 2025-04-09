using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public Transform handPosition; // Assign a GameObject in the player (e.g., "Hand") or find it by tag

    private void Awake()
    {
        if (handPosition == null)
        {
            GameObject handObject = GameObject.FindGameObjectWithTag("handPosition");
            if (handObject != null)
            {
                handPosition = handObject.transform;
            }
            else
            {
                Debug.LogError("Hand position object with tag 'handPosition' not found!");
            }
        }
    }
    private GameObject currentWeapon;

    public void EquipWeapon(GameObject weaponPrefab)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon); // Remove existing weapon
        }

        currentWeapon = Instantiate(weaponPrefab, handPosition.position, handPosition.rotation);
        currentWeapon.transform.SetParent(handPosition); // Attach to player's hand
    }
}
