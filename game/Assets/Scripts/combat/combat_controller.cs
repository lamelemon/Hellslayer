using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
public class combat_controller : MonoBehaviour
{
    [Header("References about player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInputManager getInput;
    [SerializeField] private PlayerInteraction playerItemInteraction; // Reference to PlayerItemInteraction

    [Header("")]

    [Header("Animation settings")]
    public Animator animator;

    [Header("")]

    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackCooldown = 1.0f;

    [Header("")]

    [Header("Enemy layers Settings")]
    public LayerMask enemyLayer;

    [Header("")]

    private bool canAttack = true;

    void Awake()
    {
        enemyLayer = LayerMask.GetMask("enemy_1");
    }
    void Update()
    {
        Attack_Clicked(canAttack && getInput.AttackInput.WasPressedThisFrame() && !PauseMenu.isPaused);
    }

    void Attack_Clicked(bool is_attacking)
    {
        animator.SetBool("is_attacking", is_attacking);
        if (is_attacking)
        {
            Attack();
        }
    }


    void Attack()
    {
        print("Attacking!");
        canAttack = false;
        // Check if the player is holding an item with the IWeapon interface
        if (playerItemInteraction.currentlyHeldItem != null && playerItemInteraction.currentlyHeldItem.TryGetComponent<IWeapon>(out var heldItem))
        {
            heldItem.Attack();
            if (playerItemInteraction.currentlyHeldItem.TryGetComponent<IReloadable>(out var reloadableItem))
            {
                reloadableItem.DeductAmmo();
            }
            StartCoroutine(AttackCooldownRoutine(heldItem.AttackCooldown));
        }
        else
        {
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
            //print(hitEnemies.Length);
            foreach (Collider enemy in hitEnemies)
            {
                hp_system hpSystem = enemy.GetComponent<hp_system>();
                if (hpSystem != null)
                {
                    hpSystem.take_damage(5); // Default damage
                    // Debug.Log("We hit " + enemy.name);
                }
            }
            StartCoroutine(AttackCooldownRoutine(attackCooldown));
        }
    }

    IEnumerator AttackCooldownRoutine(float attackCooldown)
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }


    // see attack radios on when clikking on the player
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

public interface IWeapon
{
    int Damage { get; set; } // Property to get and set damage
    float AttackRange { get; set; } // Property to get and set attack range
    float AttackCooldown { get; set; } // Property to get and set attack cooldown
    void Attack();
    void SpecialAbility();
}

public interface IReloadable
{
    bool ReloadsFully { get; set; }
    int ReloadAmount { get; set; }
    float ReloadSpeed { get; set; }
    float AmmoPerShot { get; set; }
    void Reload(int amount = 0, bool ReloadsFully = false, float ReloadSpeed = 0);
    void DeductAmmo();
}
