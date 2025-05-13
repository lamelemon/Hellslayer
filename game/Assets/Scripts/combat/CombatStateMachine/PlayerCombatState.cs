public abstract class PlayerCombatState
{
    protected PlayerCombat combat;
    protected PlayerCombatStateMachine stateMachine;

    public PlayerCombatState(PlayerCombat combat, PlayerCombatStateMachine stateMachine)
    {
        this.combat = combat;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}