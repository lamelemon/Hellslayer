using System.Diagnostics;

public class PlayerStateMachine // This is part of the player finite StateMachine
{
    public PlayerState CurrentState { get; private set; }

    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        if (CurrentState.GetType() == newState.GetType()) return; // Prevent changing to the same state
        else if (CurrentState.CanExitState())
        {
            CurrentState.ExitState();
            CurrentState = newState;
            CurrentState.EnterState();
        }
    }

    public void ChangeStateForce(PlayerState newState)
    {
        CurrentState = newState; // Assuming PlayerCrouchState is a singleton or static instance
    }
}
