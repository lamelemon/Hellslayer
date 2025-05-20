using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    private bool DefaultFirstAttackAnimationReady = false;
    private bool DefaultSecondAttackAnimationReady = false;
    private bool HasAttackedOneTime = false;
    private int Damage = 0;
    private float Knockback = 0.0f;
    private bool canAttack = true;
    private int DefaultAttack = 1;
    private bool IsAttacking = false;
    private float KnockbackForce = 0f;

    [Header("Default First Attack Settings")]
    public int DefaultFirstAttackDamage = 8;
    public float DefaultFirstAttackKnockback = 9.0f;

    [Header("Default Second Attack Settings")]
    public int DefaultSecondAttackDamage = 8;
    public float DefaultSecondAttackKnockback = 9.0f;

    public float FirstAttackKnockbackForce = 15.0f;
    public float SecondAttackKnockbackForce = 20.0f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.0f;
    public Vector3 CombatBox = new(0.8f, 0.8f, 0.8f); // Adjust as needed

    [Header("References")]
    [SerializeField] private PlayerInteraction playerItemInteraction; // Reference to PlayerItemInteraction for player item attacks
    public LayerMask enemyLayer;
    public Animator animator;
    public Transform AttackPoint; // You can if needed chance the AttackPosition game object transform base of the *animation

    private InputAction AttackAction;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput(); // Generated Input Actions class
    }

    private void OnEnable()
    {
        AttackAction = playerInput.Player.AttackInput;
        AttackAction.Enable();
    }

    private void OnDisable()
    {
        AttackAction.Disable();
    }

    private void InputsValuesReader()
    {
        IsAttacking = AttackAction.WasPressedThisFrame() && !PauseMenu.isPaused && canAttack; // Here canAttack is used
    }

    private void Update()
    {
        InputsValuesReader();

        if (IsAttacking)
        {
            if(playerItemInteraction.currentlyHeldItem != null)
            {
                WeapondAttack();
            }
            else
            {
                canAttack = false;
                ComboSelection();
                Attack();
            }
        }
    }

    public void DefaultFirstAttack() // function for animation to set animation event
    {
        DefaultFirstAttackAnimationReady = true;
    }

    public void DefaultSecondAttack()
    {
        DefaultSecondAttackAnimationReady = true;
    }

    private void ComboSelection()
    {
        if (!HasAttackedOneTime)
        {
            Damage = DefaultFirstAttackDamage;
            Knockback = DefaultFirstAttackKnockback;
            animator.SetTrigger("DefaultAttack1"); // active animation
            HasAttackedOneTime = true;
        }
        else
        {
            Damage = DefaultSecondAttackDamage;
            Knockback = DefaultSecondAttackKnockback;
            animator.SetTrigger("DefaultAttack2");
            HasAttackedOneTime = false;
        }
    }
    private void WeapondAttack()
    {
        if (playerItemInteraction.currentlyHeldItem != null && playerItemInteraction.currentlyHeldItem.TryGetComponent<IWeapon>(out var heldItem))
        {
            heldItem.Attack();
            if (playerItemInteraction.currentlyHeldItem.TryGetComponent<IReloadable>(out var reloadableItem))
            {
                reloadableItem.DeductAmmo();
            }
            StartCoroutine(AttackCooldownRoutine(heldItem.AttackCooldown));
        }
    }

    private void Attack()
    {
        if (DefaultFirstAttackAnimationReady || DefaultSecondAttackAnimationReady)
        {
            DefaultFirstAttackAnimationReady = false;
            DefaultSecondAttackAnimationReady = false;
        }

        Collider[] hitEnemies = Physics.OverlapBox(AttackPoint.position, CombatBox, AttackPoint.rotation, enemyLayer);
                // Try to get Rigidbody on the same GameObject, if not found, try parent
        foreach (Collider enemy in hitEnemies)
        {
            hp_system hpSystem = enemy.GetComponent<hp_system>();
            if (hpSystem != null)
            {
                hpSystem.take_damage(5); // Default damage
                // Debug.Log("We hit " + enemy.name);
            }

            Rigidbody targetRigidbody = enemy.GetComponent<Rigidbody>();
            if (!enemy.TryGetComponent<Rigidbody>(out targetRigidbody))
                {
                    targetRigidbody = enemy.GetComponentInParent<Rigidbody>();
                }
            if (targetRigidbody != null)
            {
                if (HasAttackedOneTime)
                {
                    KnockbackForce = FirstAttackKnockbackForce;
                }
                else
                {
                    KnockbackForce = FirstAttackKnockbackForce;
                }
                Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                targetRigidbody.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
            }
            
        }

        StartCoroutine(AttackCooldownRoutine(attackCooldown));
    }

    private IEnumerator AttackCooldownRoutine(float attackCooldown)
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(AttackPoint.position, AttackPoint.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, CombatBox * 2);
    }
}
