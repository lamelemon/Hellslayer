using UnityEngine;

public class PlayerIdleState : PlayerState // This is part of the player finite StateMachine
{
    public PlayerIdleState(PlayerMovement player, PlayerStateMachine stateMachine) 
        : base(player, stateMachine) { }

    public override void EnterState()
    {
        //Debug.Log("Entering idle State");
        // Any logic you want to trigger when entering the run state, like setting animations, etc.
    }
    public override void UpdateState() // Member to you dont need make transitions to all states to all states. Make only base of the state the valid transitions
    {
        if (player.isJumping && player.IsGrounded && player.readyToJump)
        {
            stateMachine.ChangeState(new PlayerJumpState(player, stateMachine));
        }
        else if (player.moveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(new PlayerWalkState(player, stateMachine));
        }
    }

    public override void FixedUpdateState()
    {
        // Stay still in idle and apply a small deceleration to the player	
        Vector3 IdleDeceleration = new Vector3(player.rb.linearVelocity.x, 0, player.rb.linearVelocity.z) * player.deceleration;
        player.rb.AddForce(IdleDeceleration, ForceMode.VelocityChange);
    }
}
