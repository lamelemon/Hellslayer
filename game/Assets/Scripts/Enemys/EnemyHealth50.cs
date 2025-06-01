using UnityEngine;
using UnityEngine.Rendering;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50; // Maximum health of the enemy
    public int currentHealth;
    public hp_bar_ui hpBarUI;

    private void Awake()
    {
        if (hpBarUI == null) // if hasent been set on inpector get hpBarUi component
        {
            hpBarUI = GetComponentInChildren<hp_bar_ui>(); // Get the hp_bar_ui component from children

            if (hpBarUI == null) // get from not clidren if is still null
            {
                hpBarUI = GetComponent<hp_bar_ui>();
            }
        }
    }

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health
        hpBarUI.hp_bar_max(maxHealth); // set slider right way to max x-0
    }

    // Method to take damage
    public void TakeDamage(int damageValue)
    {
        currentHealth -= damageValue;
        hpBarUI.hp_bar_set(currentHealth);
        //Debug.Log($"{gameObject.name} took {damageValue} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle enemy death
    private void Die()
    {
        //Debug.Log($"{gameObject.name} has died.");

        if (gameObject.CompareTag("spiky") || gameObject.CompareTag("bird")) // check if its spiky or brid enemy because they have own destroy logic
        {
            return; // Do nothing
        }
        else
        {
            Destroy(gameObject); // Destroy the enemy object
        }
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
