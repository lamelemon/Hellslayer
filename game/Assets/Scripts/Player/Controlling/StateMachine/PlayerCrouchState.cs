using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(PlayerMovement player, PlayerStateMachine stateMachine)
    : base(player, stateMachine) { }

    public override void EnterState()
    {
        player.playerHitbox.height = player.crouchHeight; // Set the player's height to crouch height
        player.rb.MovePosition(player.transform.position - (player.standingHeight - player.crouchHeight) / 2 * Vector3.up); // Physically move the player to the crouch position
    }

    public override void ExitState()
    {
        player.rb.MovePosition(player.transform.position + (player.standingHeight - player.crouchHeight) / 2 * Vector3.up); // Move the player to the standing position
        player.playerHitbox.height = player.standingHeight; // Reset the player's height to stand height
    }

    public override void UpdateState()
    {
        if (!player.GetInput.CrouchInput.IsPressed() && CanUnCrouch())
        {
            if (player.GetInput.MoveValue.magnitude <= 0.1f)
            {
                stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
            }
            else if (player.GetInput.MoveValue.magnitude > 0.1f && !player.isJumping)
            {
                stateMachine.ChangeState(new PlayerWalkState(player, stateMachine));
            }
            else stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
        }
    }

    public override void FixedUpdateState()
    {
        // Calculate movement direction
        Vector3 moveDirection = player.orientation.forward * player.GetInput.MoveValue.y + player.orientation.right * player.GetInput.MoveValue.x;
        moveDirection.Normalize();

        // Get current horizontal velocity
        Vector3 horizontalVelocity = new Vector3(player.rb.linearVelocity.x, 0f, player.rb.linearVelocity.z);

        // If under max speed, apply acceleration force
        if (horizontalVelocity.magnitude < player.crouchMaxSpeed)
        {
            player.rb.AddForce(moveDirection * player.crouchAcceleration, ForceMode.Acceleration);
        }
        else
        {
            // Clamp the horizontal velocity to max speed
            Vector3 clampedVelocity = horizontalVelocity.normalized * player.crouchMaxSpeed;
            player.rb.linearVelocity = new Vector3(clampedVelocity.x, player.rb.linearVelocity.y, clampedVelocity.z);
        }
    }

    private bool CanUnCrouch()
    {
        Collider[] overlapResults = Physics.OverlapCapsule(player.transform.position - (player.playerHitbox.height / 2 - player.playerHitbox.radius) * Vector3.up, player.transform.position + (player.standingHeight - player.playerHitbox.height / 2) * Vector3.up, player.playerHitbox.radius, player.layerMask);
        Bounds playerHitboxBounds = new(player.transform.position + (player.standingHeight - player.playerHitbox.height) / 2 * Vector3.up, new(player.playerHitbox.radius , player.standingHeight, player.playerHitbox.radius));

        foreach (Collider collision in overlapResults)
        {
            if (playerHitboxBounds.Intersects(collision.bounds))
            {
                return false;
            }
        }

        return true;
    }
}
