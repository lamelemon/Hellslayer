using UnityEngine;

public class SpikyWanderState : SpikyState // This is part of the player finite StateMachine
{
    public SpikyWanderState(SpikyMovement spiky, SpikyStateMachine stateMachine) 
        : base(spiky, stateMachine) { }

    public override void EnterState()
    {
        //Debug.Log("Entering idle State");
        // Any logic you want to trigger when entering the run state, like setting animations, etc.
    }
    public override void UpdateState() // Member to you dont need make transitions to all states to all states. Make only base of the state the valid transitions
    {

    }

    public override void FixedUpdateState()
    {

    }
}