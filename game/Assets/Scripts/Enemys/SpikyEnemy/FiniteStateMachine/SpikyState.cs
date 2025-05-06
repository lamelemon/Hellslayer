public abstract class SpikyState
{
    protected SpikyMovement spiky;
    protected SpikyStateMachine stateMachine;

    public SpikyState(SpikyMovement spiky, SpikyStateMachine stateMachine)
    {
        this.spiky = spiky;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
