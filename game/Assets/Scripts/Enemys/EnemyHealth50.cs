using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50; // Maximum health of the enemy
    public int currentHealth;
    private hp_bar_ui hpBarUI;
    private LootDropping lootDropping; // Reference to the LootDropping script

    private void Awake()
    {
        hpBarUI = transform.parent.GetComponentInChildren<hp_bar_ui>(); // Get the hp_bar_ui component from children
        lootDropping = GetComponentInParent<LootDropping>();
    }

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health
    }

    // Method to take damage
    public void TakeDamage(int damageValue)
    {
        currentHealth -= damageValue;
        hpBarUI.hp_bar_set(currentHealth); // Update the health bar UI
        Debug.Log($"{gameObject.name} took {damageValue} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public IEnumerator TakeDotDamage(int amount, int ticks, float interval = 1f)
    {
        for (int i = 0; i < ticks; i++)
        {
            yield return new WaitForSeconds(interval);
            TakeDamage(amount);
        }
    }  

    // Method to handle enemy death
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject); // Destroy the enemy object
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
