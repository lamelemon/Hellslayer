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
        if (getInput.AttackInput.WasPressedThisFrame() && !PauseMenu.isPaused)
        {
            Attack_Clicked(true);
        }
        else
        {
            Attack_Clicked(false);
        }
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
        if (!canAttack) return;
        print("Attacking!");
        // Check if the player is holding an item with the TestItem script
        if (playerItemInteraction.currentlyHeldItem != null)
        {
            TestItem heldItem = playerItemInteraction.currentlyHeldItem.GetComponent<TestItem>();
            if (heldItem != null)
            {
                // Call the PerformAttack method on the held item
                heldItem.PerformAttack();
                return; // Exit to avoid the default attack logic
            }
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
        }

        // Default attack logic
        canAttack = false;
        StartCoroutine(AttackCooldownRoutine());
    }

    IEnumerator AttackCooldownRoutine()
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
