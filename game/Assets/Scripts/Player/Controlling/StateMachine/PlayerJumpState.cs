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
            //Debug.Log("Entering Jump State");
        }
    }

    public override void UpdateState()
    {
        if (player.IsGrounded)
        {
            if (player.isSprinting && player.moveInput.magnitude > 0.1f)
            {
                stateMachine.ChangeState(new PlayerSprintState(player, stateMachine));
            }
            else if (player.moveInput.magnitude > 0.1f)
            {
                stateMachine.ChangeState(new PlayerWalkState(player, stateMachine));
            }
        }
    }
    private void Jump()
    {
        Sound(); // Play jump sound effect
        // Preserve horizontal velocity (x and z components)
        Vector3 horizontalVelocity = new Vector3(player.rb.linearVelocity.x, 0f, player.rb.linearVelocity.z);

        // Reset only the vertical velocity (y component)
        player.rb.linearVelocity = horizontalVelocity;

        // Apply the jump force
        player.rb.AddForce(player.transform.up * player.jumpForce, ForceMode.Impulse);

        // Reset jump state after applying force
        player.readyToJump = true;
    }

    private void Sound()
    {
        float JumpRandomPitch = UnityEngine.Random.Range(player.JumpSoundPitchMin, player.JumpSoundPitchMax);
        // Play jump sound effect
        audio_manager.Instance.PlaySFX("PlayerJump", JumpRandomPitch);
    }
}