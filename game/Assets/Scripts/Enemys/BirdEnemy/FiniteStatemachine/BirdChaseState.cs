using UnityEngine;

public class BirdChaseState : BirdState
{
    public BirdChaseState(BirdMovement bird, BirdStateMachine stateMachine) 
        : base(bird, stateMachine) { }
    Vector3 moveDirection;
    public override void EnterState()
    {
        // Any logic you want to trigger when entering the chase state
    }

    public override void UpdateState()
    {
        if (!bird.IsPlayerWatchRange())
        {
            // If the player is out of watch range, switch to idle state
            bird.stateMachine.ChangeState(new BirdIdleState(bird, bird.stateMachine));
        }
    }

    public override void FixedUpdateState()
    {
        // Calculate direction towards the player, including vertical (Y) movement
        moveDirection = (bird.target.position - bird.rb.transform.position).normalized;

        // If using a path, follow the next corner (including Y axis)
        if (bird.path.corners.Length > 1)
        {
            moveDirection = (bird.path.corners[1] - bird.rb.transform.position).normalized;
        }

        // Add up and down movement over time (sinusoidal on Y axis)
        float verticalOscillation = Mathf.Sin(Time.time * bird.verticalOscillationSpeed) * bird.verticalOscillationAmplitude;
        moveDirection.y += verticalOscillation;

        // Normalize again to avoid excessive Y
        moveDirection = moveDirection.normalized;

        // Get current velocity (including Y axis)
        Vector3 currentVelocity = bird.rb.linearVelocity;

        // If under max speed, apply acceleration force
        if (currentVelocity.magnitude < bird.chaseMaxSpeed)
        {
            bird.rb.AddForce(moveDirection * bird.chaseAcceleration, ForceMode.Acceleration);
        }
        else
        {
            // Clamp the velocity to max speed (including Y axis)
            Vector3 clampedVelocity = currentVelocity.normalized * bird.chaseMaxSpeed;
            bird.rb.linearVelocity = clampedVelocity;
        }
    }
}
