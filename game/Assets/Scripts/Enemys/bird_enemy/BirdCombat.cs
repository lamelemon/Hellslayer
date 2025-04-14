using UnityEngine;
using System.Collections;


// Summary:
// This script handles the combat behavior of an enemy in a Unity game. 
// It checks if the player is within attack range and, if so, deals damage to the player while respecting a cooldown period.
// Key functionalities include:
// - Detecting the player's presence within a specified attack range.
// - Dealing damage to the player's health system.
// - Implementing an attack cooldown to prevent continuous attacks.
// - Visualizing the attack range in the Unity editor using Gizmos.


public class BirdCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float AttackRange = 3f;
    [SerializeField] private float attackCooldown = 1f; // Cooldown duration in seconds
    [SerializeField] private int attackDamage = 5; // Damage dealt to the player
    private bool attacked = false; // Flag to track if the enemy has attacked
    private Transform player; // Reference to the player's transform

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = player_manager.instance.player.transform;
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set. Please assign a player Transform.");
        }
    }

    // Update is called once per frame
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
        return sqrDistance <= AttackRange * AttackRange;
    }

    private void AttackPlayer()
    {
        if (!attacked)
        {
            hp_system hpSystem = player.GetComponent<hp_system>();
            if (hpSystem != null)
            {
                hpSystem.take_damage(attackDamage);
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}