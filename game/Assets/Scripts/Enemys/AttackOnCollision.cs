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
    [SerializeField] private GameObject Player ; // Drag the desired collider here in the inspector

    private bool attacked = false;

    void Start()
    {
        if (Player == null)
        {
            //Debug.LogWarning("Player GameObject not assigned. Finding by tag instead.");
            Player = GameObject.FindGameObjectWithTag("Player");
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Collided with: " + collision.gameObject.name);
            AttackPlayer(collision.gameObject);
        }
    }

    private void AttackPlayer(GameObject playerObject)
    {
        if (!attacked)
        {
            var hpSystem = playerObject.GetComponent<hp_system>();
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
}