using UnityEngine;
public class CombatIdleState : CombatState // This is part of the player finite StateMachine
{
    public CombatIdleState(Combat combat, CombatStateMachine stateMachine) 
        : base(combat, stateMachine) { }

    public override void EnterState()
    {
        combat.animator.SetBool("isFirstAttacking", false);
        combat.animator.SetBool("isSecondAttacking", false);
        combat.animator.SetBool("isThirdAttacking", false);
    }
    public override void UpdateState() // Member to you dont need make transitions to all states to all states. Make only base of the state the valid transitions
    {
        if (combat.isAttacking && combat.canFirstAttack)
        {
            stateMachine.ChangeState(new CombatFirstAttackState(combat, stateMachine));
        }
    }

    public override void FixedUpdateState()
    {

    }
}