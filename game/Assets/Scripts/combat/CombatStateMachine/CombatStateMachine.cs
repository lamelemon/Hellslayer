public class CombatStateMachine
{
    public CombatState currentState { get; private set; }

    public void Initialize(CombatState startingState)
    {
        currentState = startingState;
        currentState.EnterState();
    }

    public void ChangeState(CombatState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}