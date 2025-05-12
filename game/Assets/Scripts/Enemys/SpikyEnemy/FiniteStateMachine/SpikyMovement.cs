using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.AI;

// Notes:
// NavMesh.CalculatePath docs: https://docs.unity3d.com/ScriptReference/AI.NavMesh.CalculatePath.html
// Using [SerializeField] private - - for variables that can be chanced in editor but cant be accessed from other scripts other than this one
// The settings are good, but member the rigibody settings e.g mass.

[RequireComponent(typeof(Rigidbody))]
public class SpikyMovement : MonoBehaviour
{
    public SpikyStateMachine stateMachine;

    [Header("Movement Settings")]
    public float Deceleration = 0.001f; // Deceleration when not moving
    public float chaseAcceleration = 13f; // Movement speed when targeting the player
    public float chaseMaxSpeed = 25f; // Maximum velocity for the enemy
    [SerializeField] private float groundDrag = 0.003f; // Drag applied when grounded
    [SerializeField] private float Mass = 1f; // Mass of the enemy
    [SerializeField] private float PathFindingUpdateTime = 0.5f; // Time interval for pathfinding updates

    [Header("Dedection/Pathfinding Settings")]
    [SerializeField] private float WatchRange = 20f;

    [Header("Dependencies")]
    [SerializeField] private SphereCollider GroundCollider;

    [HideInInspector] public Rigidbody rb;

    // Ground
    private List<Collider> feetColliders = new List<Collider>(); // Colliders at feet level
    public bool IsGrounded => feetColliders.Count > 0; // Check if the player is grounded

    // Pathfinding
    [HideInInspector] public Transform target; // Variable to store the target's transform
    [HideInInspector] public NavMeshPath path;
    private float elapsed = 0.0f;
    



    private void Awake() // Initialize components and state machine
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent tipping over
        
        stateMachine = new SpikyStateMachine();
    }

    private void Start() 
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
        rb.mass = Mass;
        target = player_manager.instance.player.transform; // Assign the player's transform to the target.. *Better perfromance this way
        stateMachine.Initialize(new SpikyIdleState(this, stateMachine)); // Initialize the state machine with the idle state
    }

    private void Update() 
    {
        GroundDrag(); // Apply drag when grounded

        elapsed += Time.deltaTime; // keep track of the time elapsed since the last pathfinding update
        if (IsPlayerWatchRange())
        {
            if (elapsed > PathFindingUpdateTime)
            {
                PathFinding();
            }

            stateMachine.ChangeState(new SpikyChaseState(this, stateMachine)); // start chase if in watch range and update pathfinding if elapsed time is greater than 1 second
        }
        else
        {
            stateMachine.ChangeState(new SpikyIdleState(this, stateMachine)); // start idle if out of watch range
        }

        stateMachine.currentState.UpdateState(); // Delegate update logic to the current state
    }
    private void PathFinding()
    {
        elapsed -= PathFindingUpdateTime; // Reset elapsed time
        // Calculate the path to the target position
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

        // Log the number of corners in the path
        //Debug.Log($"Path corners count: {path.corners.Length}");
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    }

    public bool IsPlayerWatchRange()
    {
        float sqrDistance = (target.position - transform.position).sqrMagnitude;
        return sqrDistance <= WatchRange * WatchRange;
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
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, WatchRange);
            float feetLevel = GroundCollider.bounds.center.y - GroundCollider.bounds.extents.y;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(GroundCollider.bounds.min.x, feetLevel, GroundCollider.bounds.min.z),
                            new Vector3(GroundCollider.bounds.max.x, feetLevel, GroundCollider.bounds.max.z));
        }
    }
}
