using UnityEngine;

public class SpikyIdleState : SpikyState // This is part of the player finite StateMachine
{
    public SpikyIdleState(SpikyMovement spiky, SpikyStateMachine stateMachine) 
        : base(spiky, stateMachine) { }

    public override void EnterState()
    {
        //Debug.Log("Entering idle State");
        // Any logic you want to trigger when entering the run state, like setting animations, etc.
    }
    public override void UpdateState() // Member to you dont need make transitions to all states to all states. Make only base of the state the valid transitions
    {
        if (spiky.IsPlayerWatchRange())
        {
            // If the player is out of watch range, switch to idle state
            spiky.stateMachine.ChangeState(new SpikyIdleState(spiky, spiky.stateMachine));
        }
    }

    public override void FixedUpdateState()
    {
        // Stay still in idle and apply a small deceleration to the player	
        Vector3 IdleDeceleration = new Vector3(spiky.rb.linearVelocity.x, 0, spiky.rb.linearVelocity.z) * spiky.Deceleration;
        spiky.rb.AddForce(IdleDeceleration, ForceMode.VelocityChange);
    }
}
