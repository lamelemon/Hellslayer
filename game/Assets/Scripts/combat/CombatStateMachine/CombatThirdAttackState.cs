using UnityEngine;
public class CombatThirdAttackState : CombatState // This is part of the player finite StateMachine
{
    public CombatThirdAttackState(Combat combat, CombatStateMachine stateMachine) 
        : base(combat, stateMachine) { }

    public override void EnterState()
    {
        combat.animator.SetBool("isThirdAttacking", true);
    }
    public override void UpdateState() // Member to you dont need make transitions to all states to all states. Make only base of the state the valid transitions
    {
        if (!combat.isAttacking)
        {
            stateMachine.ChangeState(new CombatIdleState(combat, stateMachine));
        }
    }

    public override void FixedUpdateState()
    {

    }
}