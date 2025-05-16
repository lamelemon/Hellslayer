using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class Combat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackCooldownTime = 5.0f;
    public float attackComboTime = 15.0f; // normally is longer than the attackCooldownTimer
    public int baseAttackDamage = 5;

    [Header("Animation settings")]
    public Animator animator;
    [SerializeField] private PlayerInputManager getInput;
    public bool isAttacking = false;
    private PlayerInput playerInput; // Input system reference
    private InputAction AttackAction; // Movement input action
    public LayerMask enemyLayer;

    public CombatStateMachine stateMachine;

    public bool canFirstAttack = true; // Tracks if the player can perform the first attack
    public float attackTimer = 0f; // Timer for attack cooldown

    private void Awake()
    {
        playerInput = new PlayerInput(); // Generated Input System class
        stateMachine = new CombatStateMachine();
    }
    private void OnEnable()
    {
        // Enable input actions
        AttackAction = playerInput.Player.AttackInput;

        AttackAction.Enable();
    }
    private void OnDisable()
    {
        // Disable input actions
        AttackAction.Disable();
    }
    private void InputsValuesReader()
    {
        // Read input values
        isAttacking = AttackAction.ReadValue<float>() > 0.1f;
    }

    private void Start()
    {
        stateMachine.Initialize(new CombatIdleState(this, stateMachine));
    }

    private void Update()
    {
        InputsValuesReader(); // Read input values
        if (!canFirstAttack) {Timer();} // Update the attack cooldown timer

        if (isAttacking && canFirstAttack)
        {
            stateMachine.ChangeState(new CombatFirstAttackState(this, stateMachine));
        }
        else
        {
            stateMachine.ChangeState(new CombatIdleState(this, stateMachine));
        }


        stateMachine.currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdateState();
    }

    private void Timer()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            canFirstAttack = true;
            attackTimer = 0f; // Reset timer   
        }
    }
}
