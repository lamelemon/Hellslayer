using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour
{
    private bool FistFirstAttackAnimationReady = false;
    private bool FistSecondAttackAnimationReady = false;
    private bool HasAttackedOneTime = false;
    private int Damage = 0;
    private float Knockback = 0.0f;
    private bool canAttack = true;

    public int FistFirstAttackDamage = 8;
    public int FistSecondAttackDamage = 10;

    public float FistFirstAttackKnockback = 8.0f;
    public float FistSecondAttackKnockback = 9.0f;

    public float attackCooldown = 1.0f;

    public LayerMask enemyLayer;
    
    /*[SerializeField] private PlayerInteraction playerItemInteraction;
    
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
        //isAttacking = AttackAction.ReadValue<float>() > 0.1f;
        isAttacking = AttackAction.WasPressedThisFrame() && !PauseMenu.isPaused && canAttack;// && AttackAction.ReadValue<float>() > 0.1f;
    }

    private void Update()
    {
        InputsValuesReader();

        if (isAttacking)
        {
            canAttack = false;
            ComboSelection();
            Attack();
        }
    }

    public void FistFirstAttack()
    {
        FistFirstAttackAnimationReady = true;
    }

    public void FistSecondAttack()
    {
        FistSecondAttackAnimationReady = true;
    }

    private void ComboSelection()
    {
        if (!HasAttackedOneTime)
        {
            Damage = FistFirstAttackDamage;
            Knockback = FistFirstAttackKnockback;
        }
        else
        {
            Damage = FistSecondAttackDamage;
            Knockback = FistSecondAttackKnockback;
        }
    }

    private void Attack();
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

        else
        {
            StartCoroutine(AttackCooldownRoutine(attackCooldown));
        }
    }

    IEnumerator AttackCooldownRoutine(float attackCooldown)
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }*/
}