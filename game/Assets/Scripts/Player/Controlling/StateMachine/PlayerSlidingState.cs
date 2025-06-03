using UnityEngine;

public class PlayerSlidingState : PlayerState
{

    public PlayerSlidingState(PlayerMovement player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void EnterState()
    {
        player.playerHitbox.height = player.slideHeight;
        player.rb.MovePosition(player.transform.position - (player.standingHeight - player.slideHeight) / 2 * Vector3.up);
    }

    public override void ExitState()
    {
        player.rb.MovePosition(player.transform.position + (player.standingHeight - player.slideHeight) / 2 * Vector3.up); // Move the player to the standing position
        player.playerHitbox.height = player.standingHeight; // Reset the player's height to stand height
    }

    public override void FixedUpdateState()
    {
        // Calculate movement direction
        Vector3 moveDirection = player.orientation.right * player.GetInput.MoveValue.x;
        moveDirection.Normalize();
        player.rb.AddForce(moveDirection * player.slideSteeringStrength, ForceMode.Acceleration);
        
        Vector3 Deceleration = new Vector3(player.rb.linearVelocity.x, 0, player.rb.linearVelocity.z) * player.slideDeceleration;
        player.rb.AddForce(Deceleration, ForceMode.VelocityChange);
    }
    
    public bool CanStandUp()
    {
        Collider[] overlapResults = Physics.OverlapCapsule(player.transform.position - (player.playerHitbox.height / 2 - player.playerHitbox.radius) * Vector3.up, player.transform.position + (player.standingHeight - player.playerHitbox.height / 2) * Vector3.up, player.playerHitbox.radius, player.rb.excludeLayers);
        Bounds playerHitboxBounds = new(player.transform.position + (player.standingHeight - player.playerHitbox.height) / 2 * Vector3.up, new(player.playerHitbox.radius, player.standingHeight, player.playerHitbox.radius));

        foreach (Collider collision in overlapResults)
        {
            if (playerHitboxBounds.Intersects(collision.bounds))
            {
                return false;
            }
        }

        return true;
    }

    public override bool CanExitState()
    {
        bool result = CanStandUp();
        if (result)
        {
            return base.CanExitState();
        }
        else
        {
            stateMachine.ChangeStateForce(new PlayerSprintState(player, stateMachine));
            return false;
        }
    }
}
