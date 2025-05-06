using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// Notes:

[RequireComponent(typeof(Rigidbody))]
public class SpikyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Range(0f, 10f)] public float groundDrag = 1.3f; // Drag applied when grounded

    [Header("Dependencies")]
    [HideInInspector] public Rigidbody rb;
    public SpikyStateMachine stateMachine;

    // Ground detection Settings/Dependencies
    public SphereCollider GroundCollider; // Reference to the player's CapsuleCollider
    private List<Collider> feetColliders = new List<Collider>(); // Colliders at feet level
    public bool IsGrounded => feetColliders.Count > 0; // Check if the player is grounded


    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent tipping over
        // Initialize components and state machine
        stateMachine = new SpikyStateMachine();
    }

    private void Start() 
    {
        stateMachine.Initialize(new SpikyIdleState(this, stateMachine)); // Initialize the state machine with the idle state
    }

    private void Update() 
    {
        GroundDrag(); // Apply drag when grounded
        if (1 == 1) // Handle state transitions based on input and conditions
        {
            stateMachine.ChangeState(new SpikyIdleState(this, stateMachine));
        }

        
        stateMachine.currentState.UpdateState(); // Delegate update logic to the current state
    }

    private void FixedUpdate()
    {
        
        stateMachine.currentState.FixedUpdateState(); // Delegate physics-related logic to the current state
    }



    private void OnCollisionEnter(Collision collision) // Ground detection Enter -using collision events
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // Calculate the feet level (center - extents.y)
            float feetLevel = GroundCollider.bounds.center.y - GroundCollider.bounds.extents.y;

            // If the contact point is at feet level, add the collider to the list
            if (Mathf.Abs(contact.point.y - feetLevel) < 0.1f) // Tolerance to check feet level
            {
                if (!feetColliders.Contains(collision.collider))
                {
                    feetColliders.Add(collision.collider);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision) // Ground detection Exit
    {
        // Remove the collider from the list when the collision ends
        if (feetColliders.Contains(collision.collider))
        {
            feetColliders.Remove(collision.collider);
        }
    }

    private void GroundDrag()
    {
        // Apply drag when grounded
        if (IsGrounded)
        {
            rb.linearDamping = groundDrag; // Apply drag when grounded
        }
        else
        {
            rb.linearDamping = groundDrag; // No drag in the air
        }
    }
    private void OnDrawGizmos()
    {
        if (GroundCollider != null)
        {
            float feetLevel = GroundCollider.bounds.center.y - GroundCollider.bounds.extents.y;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(GroundCollider.bounds.min.x, feetLevel, GroundCollider.bounds.min.z),
                            new Vector3(GroundCollider.bounds.max.x, feetLevel, GroundCollider.bounds.max.z));
        }
    }
}
