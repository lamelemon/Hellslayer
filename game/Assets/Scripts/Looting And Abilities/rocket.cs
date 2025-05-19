using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);

        // Find all colliders on the player and ignore collisions with each one
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider rocketCollider = GetComponent<Collider>();
            if (rocketCollider != null)
            {
                Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
                foreach (Collider playerCol in playerColliders)
                {
                    Physics.IgnoreCollision(rocketCollider, playerCol);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;

        Destroy(gameObject);
    }
}
