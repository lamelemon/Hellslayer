using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public GameObject resume_button;
    public GameObject quit_button;
    public RectTransform healthBar; // Reference to the health bar's RectTransform
    public RectTransform staminaBar; // Reference to the stamina bar's RectTransform

    public RectTransform combatstaminaBar; // Reference to the combat stamina bar's RectTransform

    void Update()
    {
        // Position the resume button
        Vector3 resume_position = resume_button.transform.position;
        resume_position.x = Screen.width / 2;
        resume_position.y = Screen.height / 2 + 40;
        resume_button.transform.position = resume_position;

        // Position the quit button
        Vector3 quit_position = quit_button.transform.position;
        quit_position.x = Screen.width / 2;
        quit_position.y = Screen.height / 2 - 40;
        quit_button.transform.position = quit_position;

        // Position the health bar in the lower-left corner
        if (healthBar != null)
        {
            // Anchor the health bar to the bottom-left corner
            healthBar.anchorMin = new Vector2(0, 0); // Bottom-left corner
            healthBar.anchorMax = new Vector2(0, 0); // Bottom-left corner
            healthBar.pivot = new Vector2(0, 0);     // Pivot at the bottom-left corner

            // Offset the health bar by 20 pixels from the left and bottom
            healthBar.anchoredPosition = new Vector2(20, 90);
        }

        // Position the stamina bar in the lower-left corner, above the health bar
        if (staminaBar != null)
        {
            // Anchor the stamina bar to the bottom-left corner
            staminaBar.anchorMin = new Vector2(0, 0); // Bottom-left corner
            staminaBar.anchorMax = new Vector2(0, 0); // Bottom-left corner
            staminaBar.pivot = new Vector2(0, 0);     // Pivot at the bottom-left corner

            // Offset the stamina bar by 20 pixels from the left and 100 pixels from the bottom
            staminaBar.anchoredPosition = new Vector2(20, 50);
        }


        if (combatstaminaBar != null)
        {
            // Anchor the stamina bar to the bottom-left corner
            combatstaminaBar.anchorMin = new Vector2(0, 0); // Bottom-left corner
            combatstaminaBar.anchorMax = new Vector2(0, 0); // Bottom-left corner
            combatstaminaBar.pivot = new Vector2(0, 0);     // Pivot at the bottom-left corner

            // Offset the stamina bar by 20 pixels from the left and 100 pixels from the bottom
            combatstaminaBar.anchoredPosition = new Vector2(20, 10);
        }
    }
}