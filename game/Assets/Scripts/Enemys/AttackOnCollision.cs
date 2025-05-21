using UnityEngine;
using System.Collections;

// Applies damage to objects with an hp_system and the "Player" tag when colliding,
// with a cooldown to prevent instant repeated damage. 
// You can set which collider triggers the attack.

public class AttackOnCollision : MonoBehaviour
{
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float attackCooldown = 0.2f;
    [SerializeField] private Collider attackCollider; // Drag the desired collider here in the inspector
    [SerializeField] private GameObject Player; // Drag the desired collider here in the inspector

    private bool attacked = false;


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided with: " + collision.gameObject.name);
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (!attacked)
        {
            hp_system hpSystem = Player.GetComponent<hp_system>();
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
    /*private void OnCollisionStay(Collision collision)
    {
        // Only react if the collision involves the specified attackCollider
        if (attackCollider != null &&
            collision.contacts.Length > 0 &&
            collision.contacts[0].thisCollider != attackCollider)
        {
            return;
        }

        if (!canAttack) return;

        // Check for both hp_system and "Player" tag
        var hp = collision.gameObject.GetComponent<hp_system>();
        if (hp != null && collision.gameObject.CompareTag("Player"))
        {
            hp.take_damage(damage);
            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }*/
}