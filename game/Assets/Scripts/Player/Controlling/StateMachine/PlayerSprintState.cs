using UnityEngine;

public class PlayerSprintState : PlayerState // This is part of the player finite StateMachine
{
    public PlayerSprintState(PlayerMovement player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void EnterState()
    {
        //Debug.Log("Entering Run State");
        // Any logic you want to trigger when entering the run state, like setting animations, etc.
    }

    public override void FixedUpdateState()
    {
        // Calculate movement direction
        Vector3 moveDirection = player.orientation.forward * player.GetInput.MoveValue.y + player.orientation.right * player.GetInput.MoveValue.x;
        moveDirection.Normalize();

        // Get current horizontal velocity
        Vector3 horizontalVelocity = new Vector3(player.rb.linearVelocity.x, 0f, player.rb.linearVelocity.z);
        // If under max speed, apply acceleration force
        if (horizontalVelocity.magnitude < player.sprintMaxSpeed)
        {
            player.rb.AddForce(moveDirection * player.sprintAcceleration, ForceMode.Acceleration);
        }
        else
        {
            // Clamp the horizontal velocity to max speed
            Vector3 clampedVelocity = horizontalVelocity.normalized * player.sprintMaxSpeed;
            player.rb.linearVelocity = new Vector3(clampedVelocity.x, player.rb.linearVelocity.y, clampedVelocity.z);
        }
    }

    public override bool CanExitState()
    {
        return base.CanExitState();
    }
}
