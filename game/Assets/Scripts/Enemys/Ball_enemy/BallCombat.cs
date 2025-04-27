using NUnit.Framework.Interfaces;
using UnityEngine;
using System.Collections;

public class BallCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 1f; // Cooldown duration in seconds
    [SerializeField] private float damageMultiplier = 0.5f; // Damage multiplier based on the enemy's speed
    [SerializeField] private float attackRange = 4f; // Attack range for the enemy
    private bool attacked = false; // Flag to check if the enemy has attacked
    private hp_system playerHpSystem; // Reference to the player's hp_system component
    private Rigidbody rb; // Reference to the Rigidbody component
    private Transform player; // Reference to the player's transform

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        player = player_manager.instance.player.transform;
        playerHpSystem = player.GetComponent<hp_system>();
    }

    void Update()
    {
        if (player != null && IsPlayerInSecondaryRange())
        {
            // Add non-physics-related logic here, e.g., preparing for an attack
            AttackPlayer();
        }
    }

    private bool IsPlayerInSecondaryRange()
    {
        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        return sqrDistance <= attackRange * attackRange;
    }
    private void AttackPlayer()
    {
        if (!attacked)
        {
            if (playerHpSystem != null)
            {
                playerHpSystem.take_damage(Mathf.RoundToInt(rb.linearVelocity.magnitude * damageMultiplier));
                attacked = true;
                StartCoroutine(AttackCooldown());
            }
        }
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown); // Cooldown duration in seconds
        attacked = false;
    }
}
