public abstract class PlayerState // This is part of the player finite StateMachine
{
    protected PlayerMovement player;
    protected PlayerStateMachine stateMachine;

    public PlayerState(PlayerMovement player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
