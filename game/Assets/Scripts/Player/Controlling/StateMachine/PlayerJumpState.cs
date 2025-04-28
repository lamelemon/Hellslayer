using UnityEngine;

public class PlayerJumpState : PlayerState // This is part of the player finite StateMachine
{
    public PlayerJumpState(PlayerMovement player, PlayerStateMachine stateMachine) 
        : base(player, stateMachine) { }

    public override void EnterState()
    {
        if (Time.time >= player.lastJumpTime + player.jumpCooldown)
        {
            player.readyToJump = false;
            Jump();
            player.lastJumpTime = Time.time; // Record the time of the jump
            Debug.Log("Entering Jump State");
        }
        else
        {
            Debug.Log("Jump on cooldown");
        }
    }

    public override void UpdateState()
    {
        if (player.IsGrounded && player.moveInput.magnitude <= 0.1f)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    private void Jump()
    {
        player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, 0f, player.rb.linearVelocity.z); // reset y velocity
        player.rb.AddForce(player.transform.up * player.jumpForce, ForceMode.Impulse);
        player.readyToJump = true; // Reset jump state after applying force
    }
}