using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Katana : MonoBehaviour, IWeapon, ISpecialAbility
{
    public int Damage { get; set; } = 20; // Damage dealt by the katana
    public float AttackRange { get; set; } = 2f; // Range of the katana attack
    public float AttackCooldown { get; set; } = 1f; // Time between attacks
    private bool isUsingSpecial = false; // Time until the special ability can be used again
    public float specialDuration = 6f;
    public float specialAbilityRadius = 5f;
    public int maxSpecialAbilityHits = 3;
    public int SpecialDotDamage { get; set; } = 6; // Damage dealt by the special ability over time
    public int SpecialDotTicks { get; set; } = 2; // Duration of the special ability's damage over time
    public float SpecialCooldown { get; set; } = 10f; // Cooldown time for the special ability
    private GameObject[] hitEnemies; // Array to store enemies hit by the special ability

    public void SpecialAbility()
    {
        StartCoroutine(SpecialDuration(specialDuration));
    }

    private void SpecialAbilityAttack(GameObject enemy)
    {
        if (hitEnemies.Count() >= maxSpecialAbilityHits)
        {
            hitEnemies = null; // Clear the hit enemies list if max hits reached
            return;
        }

        try {hitEnemies.Append(enemy);} // Add the enemy to the hit enemies list  
        catch { return; } // If the list is full, return

        if (enemy.TryGetComponent(out hp_system enemyHp))
        {
            enemyHp.take_damage(Damage); // Deal damage to the enemy and apply dot
            StartCoroutine(enemyHp.TakeDotDamage(SpecialDotDamage, SpecialDotTicks));
        }

        else
        {
            Debug.LogWarning($"Enemy {enemy.transform.parent.name} does not have hp_system component.");
            return;
        }

        Collider[] enemiesHit = Physics.OverlapSphere(enemyHp.transform.position, specialAbilityRadius, LayerMask.GetMask("enemy_1", "enemyLayer"));

        foreach (Collider enemyCollider in enemiesHit)
        {
            if (hitEnemies.Contains(enemyCollider.gameObject)) continue; // skip if the enemy is already hit

            SpecialAbilityAttack(enemyCollider.gameObject); // Call the special ability attack method
            break;
        }
    }

    public void Attack()
    {
        // Perform a raycast to check for enemies in range
        if (Physics.SphereCast(transform.position, AttackRange, GameObject.FindGameObjectWithTag("playerCamera").transform.forward, out RaycastHit hit, AttackRange, LayerMask.GetMask("enemy_1", "enemyLayer")) && hit.collider.TryGetComponent(out hp_system enemyHp))
        {
            if (isUsingSpecial)
            {
                hitEnemies = new GameObject[maxSpecialAbilityHits]; // Reset the hit enemies array
                SpecialAbilityAttack(hit.collider.gameObject); // Call the special ability attack method
            }

            else
            {
                hit.collider.gameObject.GetComponent<hp_system>().take_damage(Damage); // Normal damage
            }
        }
    }


    private IEnumerator SpecialDuration(float duration)
    {
        isUsingSpecial = true;
        yield return new WaitForSeconds(duration);
        isUsingSpecial = false;
    }
}