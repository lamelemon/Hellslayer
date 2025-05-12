/*  // <- makes all the code in the file a comment

------------------------
------------------------
**SpikyState.cs file

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
------------------------
------------------------
**SpikyIdleState.cs file

using UnityEngine;

public class SpikyIdleState : SpikyState // This is part of the player finite StateMachine
{
    public SpikyIdleState(SpikyMovement spiky, SpikyStateMachine stateMachine) 
        : base(spiky, stateMachine) { }

    public override void EnterState()
    {
        //Debug.Log("Entering idle State");
        // Any logic you want to trigger when entering the run state, like setting animations, etc.
        Debug.Log("enterState test 4");
    }
    public override void UpdateState() // Member to you dont need make transitions to all states to all states. Make only base of the state the valid transitions
    {

    }

    public override void FixedUpdateState()
    {

    }
}
------------------------
------------------------
**SpikyMovement.cs file

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// Notes:

[RequireComponent(typeof(Rigidbody))]
public class SpikyMovement : MonoBehaviour
{
    public SpikyStateMachine stateMachine;


    private void Awake() 
    {
        Debug.Log("awake test 1");
        // Initialize components and state machine
        stateMachine = new SpikyStateMachine();
    }

    private void Start() 
    {
        
        stateMachine.Initialize(new SpikyIdleState(this, stateMachine)); // Initialize the state machine with the idle state
        Debug.Log("start test 2");
    }

    private void Update() 
    {
        if (1 == 1) // Handle state transitions based on input and conditions
        {
            Debug.Log("update test 3");
            stateMachine.ChangeState(new SpikyIdleState(this, stateMachine));
        }

        
        stateMachine.currentState.UpdateState(); // Delegate update logic to the current state
    }

    private void FixedUpdate()
    {
        
        stateMachine.currentState.FixedUpdateState(); // Delegate physics-related logic to the current state
    }
}
------------------------
------------------------
**SpikyStateMachine.cs file

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

*/  // <- makes all the code in the file a comment