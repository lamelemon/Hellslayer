using UnityEngine;
using UnityEngine.UI;

public class Stamina_System : MonoBehaviour
{
    [SerializeField] PlayerInputManager getInput;
    [SerializeField] PlayerController playerController; // Reference to PlayerController for IsOnFloor
    public Slider staminaBar; // Reference to the UI Slider for the stamina bar
    private float maxStamina = 100f; // Maximum stamina value
    public float currentStamina; // Current stamina value
    private float staminaRegenRate = 16f; // Stamina regeneration rate per second
    private float walkingRegenRate = 11f; // Stamina regeneration rate while walking
    private float staminaDepletionRate = 16f; // Stamina depletion rate per second
    private float staminaRegenThreshold = 20f; // Threshold to allow sprinting again
    public float jumpStaminaCost = 20f; // Stamina cost for jumping

    private bool isDepleting = false; // Whether stamina is currently being depleted
    private bool canSprintAgain = true; // Whether the player is allowed to sprint

    // Static property to check if the player can sprint
    public bool CanSprint = true;

    // Property to check if the player can jump
    public bool CanJump => 0 < currentStamina && currentStamina >= staminaRegenThreshold;

    void Start()
    {
        // Initialize stamina
        currentStamina = maxStamina;

        // Set the stamina bar's max value and initial value
        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }
    }

    void Update()
    {
        // Handle sprint input
        if (playerController.IsRunning && canSprintAgain && getInput.MoveInput.IsPressed())
        {
            if (currentStamina > 0)
            {
                CanSprint = true;
                DepleteStamina(); // Deplete stamina if the player can sprint
            }
            else
            {
                CanSprint = false;
                StopDepleting(); // Stop depleting if the player can't sprint
                canSprintAgain = false; // Prevent sprinting until stamina regenerates
            }
        }
        else
        {
            StopDepleting(); // Stop depleting when sprint input is released or not moving
        }

        // Regenerate stamina if not depleting
        if (!isDepleting && currentStamina < maxStamina)
        {
            float regenRate;
            if (getInput.MoveInput.IsPressed())
            {
                regenRate = walkingRegenRate;
            }
            else
            {
                regenRate = staminaRegenRate;
            }
            currentStamina += regenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Ensure stamina doesn't exceed max

            // Allow sprinting again if stamina reaches the threshold
            if (currentStamina >= staminaRegenThreshold)
            {
                canSprintAgain = true;
            }
        }

        // Update the stamina bar
        if (staminaBar != null)
        {
            staminaBar.value = currentStamina;
        }

        // Update CanSprint based on current stamina
        CanSprint = currentStamina > 0 && canSprintAgain;
    }

    // Call this method to deplete stamina (e.g., when sprinting)
    public void DepleteStamina()
    {
        if (currentStamina > 0)
        {
            isDepleting = true;
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Ensure stamina doesn't go below 0
        }
        else
        {
            isDepleting = false; // Stop depleting if stamina is empty
        }
    }

    // Call this method to stop depleting stamina (e.g., when stopping sprinting)
    public void StopDepleting()
    {
        isDepleting = false;
    }

    // Call this method to consume stamina for jumping
    public void ConsumeStaminaForJump()
    {
        currentStamina -= jumpStaminaCost;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Ensure stamina doesn't go below 0
    }
}
