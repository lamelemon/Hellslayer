using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerWalkState : PlayerState // This is part of the player finite StateMachine
{
    public PlayerWalkState(PlayerMovement player, PlayerStateMachine stateMachine) 
        : base(player, stateMachine) { }
    
    public override void EnterState()
    {
        //Debug.Log("Entering Walk State");
        // Any logic you want to trigger when entering the run state, like setting animations, etc.
    }
    public override void UpdateState()
    {
        if (player.isSprinting && player.moveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(new PlayerSprintState(player, stateMachine));
        }
        else if (player.moveInput.magnitude <= 0.1f)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    public override void FixedUpdateState()
    {
        // Calculate movement direction
        Vector3 moveDirection = player.orientation.forward * player.moveInput.y + player.orientation.right * player.moveInput.x;
        moveDirection.Normalize();

        // Get current horizontal velocity
        Vector3 horizontalVelocity = new Vector3(player.rb.linearVelocity.x, 0f, player.rb.linearVelocity.z);

        // If under max speed, apply acceleration force
        if (horizontalVelocity.magnitude < player.walkMaxSpeed)
        {
            player.rb.AddForce(moveDirection * player.walkAcceleration, ForceMode.Acceleration);
        }
        else
        {
            // Clamp the horizontal velocity to max speed
            Vector3 clampedVelocity = horizontalVelocity.normalized * player.walkMaxSpeed;
            player.rb.linearVelocity = new Vector3(clampedVelocity.x, player.rb.linearVelocity.y, clampedVelocity.z);
        }
    }
}
