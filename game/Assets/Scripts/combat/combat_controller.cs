using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
public class combat_controller : MonoBehaviour
{
    [Header("References about player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInputManager getInput;

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
    public LayerMask enemyLayers;

    [Header("")]
    
    private bool canAttack = true;
    
    void Update()
    {
        if (getInput.AttackInput.IsPressed())
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

        canAttack = false;
        StartCoroutine(AttackCooldownRoutine());

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            hp_system hpSystem = enemy.GetComponent<hp_system>();
            if (hpSystem != null)
            {
                hpSystem.take_damage(5);
                //Debug.Log("We hit " + enemy.name);
            }
        }
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
