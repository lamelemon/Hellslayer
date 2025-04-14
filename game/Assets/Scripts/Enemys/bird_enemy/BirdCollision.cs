using UnityEngine;


// This script handles enemy collision behavior in Unity.
// - Pushes the enemy away from the player upon collision using a force.
// - Limits the enemy's velocity to a maximum speed.
// - Freezes rotation to prevent unwanted rotations during collisions.


public class BirdCollision : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forceMultiplier = 10f;
    [SerializeField] private float maxSpeed = 5f; // Maximum velocity for the enemy
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Freeze rotation on all axes to limit rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;
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