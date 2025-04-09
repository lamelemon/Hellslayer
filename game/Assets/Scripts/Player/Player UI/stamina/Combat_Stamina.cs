using UnityEngine;

public class Combat_Stamina : MonoBehaviour
{
    [SerializeField] private combat_stamina_bar_ui staminaBarUI; // Reference to the UI script
    private float maxStamina = 100f; // Maximum stamina
    private float currentStamina;   // Current stamina
    private float staminaDrain = 10f; // Stamina drained per punch
    private float staminaRegenRate = 5f; // Stamina regeneration rate per second
    private float regenDelay = 2f; // Delay before stamina starts regenerating

    private float lastPunchTime; // Tracks the time of the last punch

    [SerializeField] private PlayerInputManager getInput; // Reference to PlayerInputManager for input handling

    // Property to check if the player can punch
    public bool CanPunch => currentStamina >= staminaDrain;

    void Start()
    {
        // Initialize current stamina to max stamina
        currentStamina = maxStamina;

        // Initialize the stamina bar UI
        staminaBarUI.combat_stamina_bar_max((int)maxStamina);
    }

    void Update()
    {
        // Check for punch input
        if (getInput.AttackInput.WasPressedThisFrame() && CanPunch)
        {
            Punch();
        }

        // Regenerate stamina if enough time has passed since the last punch
        if (Time.time - lastPunchTime >= regenDelay)
        {
            RegenerateStamina();
        }

        // Update the stamina bar UI
        staminaBarUI.combat_stamina_bar_set((int)currentStamina);
    }

    void Punch()
    {
        if (CanPunch)
        {
            // Perform punch logic here
            Debug.Log("Player punched!");

            // Reduce stamina
            currentStamina -= staminaDrain;

            // Update the last punch time
            lastPunchTime = Time.time;
        }
        else
        {
            Debug.Log("Not enough stamina to punch!");
        }
    }

    void RegenerateStamina()
    {
        // Gradually regenerate stamina
        currentStamina += staminaRegenRate * Time.deltaTime;

        // Clamp stamina to max value
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }
}
