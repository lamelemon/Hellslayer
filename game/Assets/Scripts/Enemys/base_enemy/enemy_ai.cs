using UnityEngine;

// this enemy is basic its get stuck to the "walls" and thats why I recomend use this as base or large groups of enemies..
public class Enemy_AI : MonoBehaviour
{
    public Transform player;
    private hp_system hp_System;
    public float detectRange = 300f;
    public float moveSpeed = 3f;
    private Rigidbody rb;
    private bool playerDetected = false;
    private bool isTouchingPlayer = false;
    public float damageInterval = 1f; // Time in seconds between damage ticks
    private float nextDamageTime = 0f;

    void Awake()
    {
        // Automatically assign components
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

        hp_System = GetComponent<hp_system>();
        if (hp_System == null)
        {
            Debug.LogWarning("hp_system component is missing on this enemy!");
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player object with tag 'Player' not found in the scene! -enemy");
            }
        }
    }

    void Update()
    {
        DetectPlayer();
        if (playerDetected)
        {
            LookAtPlayer();
        }

        if (isTouchingPlayer && Time.time >= nextDamageTime)
        {
            DealDamageToPlayer();
            nextDamageTime = Time.time + damageInterval;
        }
    }

    void FixedUpdate()
    {
        if (playerDetected)
        {
            MoveTowardsPlayer();
        }
    }

    void DetectPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectRange)
        {
            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    void LookAtPlayer()
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0; // Keep the enemy upright
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }

    void DealDamageToPlayer()
    {
        if (player != null)
        {
            hp_system player_hp = player.GetComponent<hp_system>();
            if (player_hp != null)
            {
                player_hp.take_damage(1);
                Debug.Log("Player took damage!");
            }
        }
    }
}