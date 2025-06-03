using UnityEngine;

public class PlayerJumpState : PlayerState // This is part of the player finite StateMachine
{
    public PlayerJumpState(PlayerMovement player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void EnterState()
    {
        if (player.IsGrounded && Time.time >= player.lastJumpTime + player.jumpCooldown)
        {
            player.readyToJump = false;
            Jump();
            player.lastJumpTime = Time.time; // Record the time of the jump
            //Debug.Log("Entering Jump State");
        }

        else if (!player.IsGrounded)
        {
            Mantle();
        }
    }

    public override void UpdateState()
    {
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

    private void Mantle()
    {
        // Calculate the mantle position based on player dimensions and orientation
        Vector3 mantlePosition = player.transform.position + (player.standingHeight - player.playerHitbox.bounds.extents.y) * Vector3.up + Quaternion.Euler(0, player.rb.transform.eulerAngles.y, 0) * (player.playerHitboxRadius * 2 * Vector3.forward);
        Debug.Log(mantlePosition);
        // Check for a ledge and if the player fits on the ledge
        if (Physics.Raycast(mantlePosition + Vector3.up * player.standingHeight / 2, Vector3.down, out RaycastHit hit, player.standingHeight, player.rb.excludeLayers) && Physics.OverlapCapsule(mantlePosition, mantlePosition + player.standingHeight * Vector3.up, player.playerHitboxRadius, player.rb.excludeLayers).Length == 0)
        {
            Debug.Log("Mantling");

            // Move the player to the ledge position
            player.transform.position = hit.point + Vector3.up * player.playerHitbox.bounds.extents.y;
        }
    }

    public override bool CanExitState()
    {
        return base.CanExitState();
    }

}