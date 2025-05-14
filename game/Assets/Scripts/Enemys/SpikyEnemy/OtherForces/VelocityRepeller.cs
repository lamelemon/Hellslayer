using UnityEngine;

public class VelocityRepeller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a Rigidbody
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            // Reverse the velocity of the Rigidbody
            rb.linearVelocity = -rb.linearVelocity;
        }
    }
}
