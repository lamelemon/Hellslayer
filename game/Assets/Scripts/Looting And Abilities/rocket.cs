using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float rocketSpeed = 50f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damageValue = 20; // Match TestItem
    [SerializeField] private LayerMask enemyLayer; // Must be set to "enemyLayer" in Inspector
    [SerializeField] private GameObject explosionEffectPrefab;

    private Transform target;
    private Rigidbody rb;

    public void AssignTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = transform.forward * rocketSpeed;
        }

        // Ignore player collision
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

    private void FixedUpdate()
    {
        if (target == null || rb == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, turnSpeed * Time.fixedDeltaTime, 0f);
        rb.linearVelocity = newDirection * rocketSpeed;
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        Explode();
    }

    private void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Check for enemies in AoE using the specified enemyLayer
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);
        foreach (Collider hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageValue);
                Debug.Log($"Rocket hit {hit.name}, dealing {damageValue} damage.");
            }
        }

        Destroy(gameObject);
    }
}
