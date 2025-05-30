using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
//Notes:
// The deffault-- is the fist attack / no weapon attack
// Attack & animation timing is base of animation events thats trigers when animation is in right frame of the animation

// Summary:
// Implements a two-hit combo system synced with animation events.
// Combo attacks trigger different damage, knockback, and animations.
// Attack effects are timed using Unity Animation Events.
// Prevents attack spamming via cooldowns.
// Delegates attack logic to weapons if equipped.
// Uses Physics.OverlapBox for enemy hit detection.
// Applies directional knockback to hit enemies. // enemy dosent need any script for knockback. But need rigibody component
// Shows hitbox in editor using Gizmos.
// Animation Events: call DefaultFirstAttack or DefaultSecondAttack.
// Input alternates between DefaultAttack1 and DefaultAttack2.
// Attach to player and assign Animator, AttackPoint, etc.
// Ensures damage syncs with visuals and allows weapon expansion.

public class PlayerCombat : MonoBehaviour
{
    private int Damage = 0;
    private float KnockbackForce = 0f;
    private bool canAttack = true;
    private bool HasAttackedOneTime = false;
    private bool IsAttacking = false;
    private bool IsUsingSpecial = false;

    [Header("Default First Attack Settings")]
    public int DefaultFirstAttackDamage = 8;
    public float DefaultFirstAttackKnockback = 9.0f;

    [Header("Default Second Attack Settings")]
    public int DefaultSecondAttackDamage = 9;
    public float DefaultSecondAttackKnockback = 9.0f;

    [Header("Attack Settings")]
    public float DefaultAttackCooldown = 0.2f;
    public float attackCooldown = 0.2f;
    public Vector3 CombatBox = new(0.56f, 0.38f, 0.7f);

    [Header("References")]
    [SerializeField] private PlayerInteraction playerItemInteraction;
    public LayerMask enemyLayer;
    public Animator animator;
    public Transform AttackPoint;

    private InputAction AttackAction;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
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
        IsAttacking = AttackAction.WasPressedThisFrame() && !PauseMenu.isPaused && canAttack;
        IsUsingSpecial = playerInput.Player.SpecialInput.WasPressedThisFrame() && !PauseMenu.isPaused;
    }

    private void Update()
    {
        InputsValuesReader();

        if (IsAttacking)
        {
            if (playerItemInteraction.currentlyHeldItem != null)
            {
                WeapondAttack();
            }
            else
            {
                canAttack = false;
                ComboSelection();
            }
        }

        if (IsUsingSpecial)
        {
            WeaponSpecialAbility();
        }
    }

    // These are called by Animation Events!
    public void DefaultFirstAttack()
    {
        Damage = DefaultFirstAttackDamage;
        KnockbackForce = DefaultFirstAttackKnockback;
        Attack();
    }

    public void DefaultSecondAttack()
    {
        Damage = DefaultSecondAttackDamage;
        KnockbackForce = DefaultSecondAttackKnockback;
        Attack();
    }

    private void ComboSelection()
    {
        if (!HasAttackedOneTime)
        {
            animator.SetTrigger("DefaultAttack1");
            HasAttackedOneTime = true;
        }
        else
        {
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
            StartCoroutine(AttackCooldownRoutine(heldItem.AttackCooldown)); // make maybe difrrent cooldown?
        }
    }

    private void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapBox(AttackPoint.position, CombatBox, AttackPoint.rotation, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            hp_system hpSystem = enemy.GetComponent<hp_system>();
            if (hpSystem != null)
            {
                hpSystem.take_damage(Damage);
            }

            EnemyHealth enemyHealth = enemy.GetComponentInChildren<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(Damage);
            }

            Rigidbody targetRigidbody = enemy.GetComponent<Rigidbody>();
            if (targetRigidbody == null)
            {
                targetRigidbody = enemy.GetComponentInParent<Rigidbody>();
            }
            if (targetRigidbody != null)
            {
                Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                targetRigidbody.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
            }
        }

        StartCoroutine(DefaultAttackCooldownRoutine(DefaultAttackCooldown));
    }

    private void WeaponSpecialAbility()
    {
        if (playerItemInteraction.currentlyHeldItem != null && playerItemInteraction.currentlyHeldItem.TryGetComponent(out ISpecialAbility specialAbility))
        {
            specialAbility.SpecialAbility();
            StartCoroutine(SpecialAbilityCooldownRoutine(specialAbility.SpecialCooldown));
        }
        else
        {
            Debug.LogWarning("No weapon with a special ability equipped.");
        }
    }

    private IEnumerator AttackCooldownRoutine(float attackCooldown)
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator DefaultAttackCooldownRoutine(float DefaultAttackCooldown)
    {
        yield return new WaitForSeconds(DefaultAttackCooldown);
        canAttack = true;
    }

    private IEnumerator SpecialAbilityCooldownRoutine(float specialAbilityCooldown)
    {
        yield return new WaitForSeconds(specialAbilityCooldown);
        canAttack = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(AttackPoint.position, AttackPoint.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, CombatBox * 2);
    }
}

public interface IWeapon
{
    int Damage { get; set; } // Property to get and set damage
    float AttackRange { get; set; } // Property to get and set attack range
    float AttackCooldown { get; set; } // Property to get and set attack cooldown
    void Attack();
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

public interface ISpecialAbility
{
    void SpecialAbility();
    float SpecialCooldown { get; set; } // Property to get and set special ability cooldown
}