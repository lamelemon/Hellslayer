public abstract class CombatState
{
    protected Combat combat;
    protected CombatStateMachine stateMachine;

    public CombatState(Combat combat, CombatStateMachine stateMachine)
    {
        this.combat = combat;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}