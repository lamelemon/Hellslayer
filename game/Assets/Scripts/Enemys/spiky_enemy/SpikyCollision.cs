using UnityEngine;
using UnityEngine.AI; // Import the NavMeshAgent namespace
using System.Collections.Generic; // Import the List namespace
using System.Collections; // Import the Collections namespace

public class SpikyCollision : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forceMultiplier = 10f;
    [SerializeField] private float maxSpeed = 5f; // Maximum velocity for the enemy
    [SerializeField] private float moveSpeed = 3f; // Movement speed when targeting the player
    [SerializeField] private float turnSpeed = 5f; // Speed at which the enemy turns towards the player
    private Rigidbody rb;
    private Transform player;
    //[Header("")]
    //[Header("Animator Settings")]
    //private Animator animator; // Reference to the animator
    //private float currentSpeed = 0f; // Variable to store the current speed of the enemy

    private NavMeshAgent agent;

    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();

        rb = GetComponent<Rigidbody>();
        // This will get the Animator component from the child GameObject
        //animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player object

        // Freeze rotation on all axes to limit rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    void FixedUpdate()
    {
        if (player != null)
        {
            // Calculate direction towards player position
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0f; // Keep movement in the XZ plane

            // Rotate the enemy towards the player
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // After turning towards the player, start moving
            if (Vector3.Angle(transform.forward, directionToPlayer) < 5f) // Small angle threshold to start moving
            {
                // Move the enemy in the direction it is now facing
                rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
                
                // Update the speed for the animator
                //currentSpeed = rb.linearVelocity.magnitude;
                //animator.SetFloat("speed", currentSpeed); // Update animator parameter
            }
            else
            {
                // Update the speed to 0 if the enemy isn't moving towards the player
                //currentSpeed = 0f;
                //animator.SetFloat("speed", currentSpeed); // Update animator parameter
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Example logic for collision handling
        if (collision.gameObject.CompareTag("Player"))
        {
            // React to collision with player
            Vector3 pushDirection = collision.contacts[0].point - transform.position;
            pushDirection = -pushDirection.normalized;
            rb.AddForce(pushDirection * forceMultiplier, ForceMode.Impulse);

            // Clamp the velocity to the maximum speed
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }
}
