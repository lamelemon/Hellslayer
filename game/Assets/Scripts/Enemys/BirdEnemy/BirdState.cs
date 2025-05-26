public abstract class BirdState
{
    protected BirdMovement bird;
    protected BirdStateMachine stateMachine;

    public BirdState(BirdMovement bird, BirdStateMachine stateMachine)
    {
        this.bird = bird;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
