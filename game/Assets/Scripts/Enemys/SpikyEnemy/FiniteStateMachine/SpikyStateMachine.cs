public class SpikyStateMachine
{
    public SpikyState currentState { get; private set; }

    public void Initialize(SpikyState startingState)
    {
        currentState = startingState;
        currentState.EnterState();
    }

    public void ChangeState(SpikyState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
