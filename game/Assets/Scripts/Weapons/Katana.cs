using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float specialCooldown { get; set; } = 10f; // Cooldown time for the special ability
    private List<GameObject> hitEnemies = new(); // Array to store enemies hit by the special ability

    public void SpecialAbility()
    {
        // Implement the special ability logic here
        Debug.Log("Special ability activated!" + gameObject.name);
        StartCoroutine(SpecialDuration(specialDuration));
    }

    private void SpecialAbilityAttack(hp_system enemyHp)
    {
        if (hitEnemies.Count >= maxSpecialAbilityHits)
        {
            hitEnemies.Clear(); // Clear the hit enemies list if max hits reached
            return;
        }

        if (!hitEnemies.Contains(enemyHp.gameObject))
        {
            hitEnemies.Add(enemyHp.gameObject); // Add the enemy to the hit enemies list
        }
        
        enemyHp.take_damage(Damage);
        StartCoroutine(enemyHp.TakeDotDamage(6, 2, 1f));
        Collider[] enemiesHit = Physics.OverlapSphere(enemyHp.transform.position, specialAbilityRadius, LayerMask.GetMask("enemy_1", "enemyLayer"));

        foreach (Collider enemy in enemiesHit)
        {
            if (hitEnemies.Contains(enemy.gameObject)) continue; // Skip the original hit enemy

            else if (enemy.gameObject.TryGetComponent(out hp_system hitEnemyHpSystem))
            {
                print("hit enemy: " + enemy.gameObject.name);
                SpecialAbilityAttack(hitEnemyHpSystem); // Call the special ability attack method
                break;
            }
        }
    }

    public void Attack()
    {
        // Implement the attack logic here
        Debug.Log("Katana attack performed!");
        // Perform a raycast to check for enemies in range
        if (Physics.SphereCast(transform.position, AttackRange, GameObject.FindGameObjectWithTag("playerCamera").transform.forward, out RaycastHit hit, AttackRange, LayerMask.GetMask("enemy_1", "enemyLayer")) && hit.collider.gameObject.TryGetComponent(out hp_system enemyHp))
        {
            if (isUsingSpecial)
            {
                SpecialAbilityAttack(enemyHp); // Call the special ability attack method
            }
            else
            {
                enemyHp.take_damage(Damage); // Normal damage
            }
        }
    }


    IEnumerator SpecialDuration(float duration)
    {
        isUsingSpecial = true; // Set the special ability state
        yield return new WaitForSeconds(duration);
        isUsingSpecial = false; // Reset the special ability state
    }
}
