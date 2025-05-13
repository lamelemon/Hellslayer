public class PlayerCombatStateMachine
{
    public PlayerCombatState currentState { get; private set; }

    public void Initialize(PlayerCombatState startingState)
    {
        currentState = startingState;
        currentState.EnterState();
    }

    public void ChangeState(PlayerCombatState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}