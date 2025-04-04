using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float forceMultiplier = 10f;
    [SerializeField] private float maxSpeed = 5f; // Maximum velocity for the enemy

    [Header("")]

    [Header("Detection range Settings")]
    [SerializeField] private float detectionRange = 20f;
    [Header("Attack range Settings")]
    [SerializeField] private float secondaryDetectionRange = 2f;
    private Transform player;
    
    [Header("")]

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing on the enemy object.");
        }

        // Freeze rotation on X and Z axes to prevent falling over
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        player = player_manager.instance.player.transform;
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set. Please assign a player Transform.");
        }
    }

    private void Update()
    {
        if (player != null && IsPlayerInSecondaryRange())
        {
            // Add non-physics-related logic here, e.g., preparing for an attack
            attackplayer();
        }
    }

    private void FixedUpdate()
    {
        if (player != null && IsPlayerInRange())
        {
            LookAtPlayer(); // Rotate the enemy to face the player
            MoveTowardsPlayer();

            // Clamp the Rigidbody's velocity to the maximum speed
            Vector3 clampedVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
            rb.linearVelocity = new Vector3(clampedVelocity.x, rb.linearVelocity.y, clampedVelocity.z);
        }
    }

    private bool IsPlayerInSecondaryRange()
    {
        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        return sqrDistance <= secondaryDetectionRange * secondaryDetectionRange;
    }

    private bool IsPlayerInRange()
    {
        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        return sqrDistance <= detectionRange * detectionRange;
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 force = direction * moveSpeed * forceMultiplier;
        rb.AddForce(new Vector3(force.x, 0, force.z), ForceMode.Force);
    }

    private void LookAtPlayer()
    {
        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y; // Keep the enemy upright by aligning only the X and Z axes
        transform.LookAt(targetPosition);
    }

    private void attackplayer()
    {
        // Implement attack logic here, e.g., dealing damage to the player
        Debug.Log("Attacking player!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, secondaryDetectionRange);
    }
}