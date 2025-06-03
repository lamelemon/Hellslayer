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
    public PlayerScore playerScore; // Reference to the PlayerScore script
    public int RewardScoreOnDeath = 5; // Score to reward the player on enemy death

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

        playerScore = FindObjectOfType<PlayerScore>(); // Find the PlayerScore component in the scene
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
        playerScore.AddScore(RewardScoreOnDeath);

        if (endGameOnDeath) PauseMenu.MainMenu(); // If endGameOnDeath is true, call the MainMenu method from PauseMenu

        if (lootDropping != null && lootDropping.lootItems.Length > 0) // Check if loot items are available
        {
            lootDropping.DropLoot(); // Call the DropLoot method from the LootDropping script
            return;
        }
        else
        {
            Destroy(transform.parent.gameObject); // Destroy the enemy object
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
