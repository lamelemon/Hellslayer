using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public int attackCooldownTimer;
    public int attackComboTimer; // normally is longer than the attackCooldownTimer
    public int baseAttackDamage = 5;

    [Header("Animation settings")]
    public Animator animator;
    [SerializeField] private PlayerInputManager getInput;
    bool isAttacking = false;
    private PlayerInput playerInput; // Input system reference
    private InputAction AttackAction; // Movement input action

    public PlayerCombatStateMachine stateMachine;


    private void Awake()
    {
        playerInput = new PlayerInput(); // Generated Input System class
        stateMachine = new PlayerCombatStateMachine();
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
        stateMachine.Initialize(new PlayerCombatIdleState(this, stateMachine));
    }

    private void Update()
    {
        InputsValuesReader(); // Read input values
        //Debug.Log("isAttacking: " + isAttacking);

        if (isAttacking)
        {
            stateMachine.ChangeState(new PlayerBaseAttackState(this, stateMachine));
        }
        else
        {
            stateMachine.ChangeState(new PlayerCombatIdleState(this, stateMachine));
        }

        stateMachine.currentState.UpdateState();
    }


    private void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdateState();
    }
}
