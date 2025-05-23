using UnityEngine;
using UnityEngine.InputSystem;

public class Katana : MonoBehaviour, IWeapon, ILootable
{
    public int Damage { get; set; } = 20; // Damage dealt by the katana
    public float AttackRange { get; set; } = 2f; // Range of the katana attack
    public float AttackCooldown { get; set; } = 1f; // Time between attacks
    private hp_system playerHpSystem; // Reference to the player's health system
    [SerializeField] private PlayerInputManager GetInput; // Reference to the hand position where the katana will be attached

    public void SpecialAbility()
    {
        // Implement the special ability logic here
        Debug.Log("Special ability activated!" + gameObject.name);

    }

    public void Attack()
    {
        // Implement the attack logic here
        Debug.Log("Katana attack performed!");
        // Perform a raycast to check for enemies in range
        if (Physics.SphereCast(transform.position, AttackRange, GameObject.FindGameObjectWithTag("playerCamera").transform.forward, out RaycastHit hit, AttackRange, LayerMask.GetMask("enemy_1", "enemyLayer")))
        {   // Deal damage to the enemy
            foreach (Collider collider in hit.collider.GetComponents<Collider>())
            {
                collider.GetComponent<hp_system>().take_damage(Damage);
            }
        }
    }

    public void ItemPickedUp()
    {
        playerHpSystem = GetComponentInParent<hp_system>();
        GetInput = GetComponentInParent<PlayerInputManager>();
    }

    public void ItemDropped()
    {
        playerHpSystem = null; // Reset the health system reference
        GetInput = null; // Reset the input reference
    }
}
