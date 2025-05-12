using UnityEngine;

public class SpikyChaseState : SpikyState
{
    public SpikyChaseState(SpikyMovement spiky, SpikyStateMachine stateMachine) 
        : base(spiky, stateMachine) { }
    Vector3 moveDirection;
    public override void EnterState()
    {
        // Any logic you want to trigger when entering the chase state
    }

    public override void UpdateState()
    {
        if (!spiky.IsPlayerWatchRange())
        {
            // If the player is out of watch range, switch to idle state
            spiky.stateMachine.ChangeState(new SpikyIdleState(spiky, spiky.stateMachine));
        }
    }

    public override void FixedUpdateState()
    {
        moveDirection = (spiky.target.position - spiky.rb.transform.position).normalized;
        // Calculate direction towards the player
        if (spiky.path.corners.Length > 1)
        {
            moveDirection = (spiky.path.corners[1] - spiky.rb.transform.position).normalized;
        }

        // Get current horizontal velocity
        Vector3 horizontalVelocity = new Vector3(spiky.rb.linearVelocity.x, 0f, spiky.rb.linearVelocity.z);

        // If under max speed, apply acceleration force
        if (horizontalVelocity.magnitude < spiky.chaseMaxSpeed)
        {
            spiky.rb.AddForce(moveDirection * spiky.chaseAcceleration, ForceMode.Acceleration);
        }
        else
        {
            // Clamp the horizontal velocity to max speed
            Vector3 clampedVelocity = horizontalVelocity.normalized * spiky.chaseMaxSpeed;
            spiky.rb.linearVelocity = new Vector3(clampedVelocity.x, spiky.rb.linearVelocity.y, clampedVelocity.z);
        }
    }
}
