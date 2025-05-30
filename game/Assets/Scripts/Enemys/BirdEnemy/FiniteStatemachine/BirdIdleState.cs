using UnityEngine;

public class BirdIdleState : BirdState // This is part of the enemy finite StateMachine
{
    public BirdIdleState(BirdMovement bird, BirdStateMachine stateMachine) 
        : base(bird, stateMachine) { }

    public override void EnterState()
    {
        //Debug.Log("Entering idle State");
        // Any logic you want to trigger when entering the idle state, like setting animations, etc.
    }

    public override void UpdateState()
    {
        if (bird.IsPlayerWatchRange())
        {
            // If the player is out of watch range, switch to idle state
            bird.stateMachine.ChangeState(new BirdIdleState(bird, bird.stateMachine));
        }
    }

    public override void FixedUpdateState()
    {
        // Stay still in idle and apply a small deceleration to the bird
        Vector3 IdleDeceleration = new Vector3(bird.rb.linearVelocity.x, 0, bird.rb.linearVelocity.z) * bird.Deceleration;
        bird.rb.AddForce(IdleDeceleration, ForceMode.VelocityChange);
    }
}
