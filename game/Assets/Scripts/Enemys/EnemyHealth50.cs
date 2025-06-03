using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50; // Maximum health of the enemy
    public int currentHealth;
    public hp_bar_ui hpBarUI;
    private LootDropping lootDropping; // Reference to the LootDropping script
    public bool endGameOnDeath = false; // Flag to end the game on death
    private GameObject pausemenu;

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
        lootDropping = GetComponentInParent<LootDropping>();
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
        //Debug.Log($"{gameObject.name} has died.");

        if (endGameOnDeath) PauseMenu.MainMenu(); // If endGameOnDeath is true, call the MainMenu method from PauseMenu

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
