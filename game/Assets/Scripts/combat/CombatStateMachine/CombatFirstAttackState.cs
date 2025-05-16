using UnityEngine;
public class CombatFirstAttackState : CombatState // This is part of the player finite StateMachine
{
    public CombatFirstAttackState(Combat combat, CombatStateMachine stateMachine) 
        : base(combat, stateMachine) { }

    public override void EnterState()
    {
        combat.canFirstAttack = false; // Prevent further attacks until cooldown
        combat.attackTimer = combat.attackCooldownTime; // Start cooldown timer
        combat.animator.SetBool("isFirstAttacking", true);
    }
    public override void UpdateState() // Member to you dont need make transitions to all states to all states. Make only base of the state the valid transitions
    {
        if (combat.isAttacking)
        {
            stateMachine.ChangeState(new CombatSecondAttackState(combat, stateMachine));
        }
        else
        {
            stateMachine.ChangeState(new CombatIdleState(combat, stateMachine));
        }
    }

    public override void FixedUpdateState()
    {

    }
}