public abstract class PlayerState // This is part of the player finite StateMachine
{
    protected PlayerMovement player;
    protected PlayerStateMachine stateMachine;
    public bool canExitState { get; set; } = true; // This is a flag to check if the state can be exited

    public PlayerState(PlayerMovement player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
    public virtual bool CanExitState() { return canExitState; }
}
