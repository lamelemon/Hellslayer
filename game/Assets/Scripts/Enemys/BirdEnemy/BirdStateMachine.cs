public class BirdStateMachine
{
    public BirdState currentState { get; private set; }

    public void Initialize(BirdState startingState)
    {
        currentState = startingState;
        currentState.EnterState();
    }

    public void ChangeState(BirdState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
