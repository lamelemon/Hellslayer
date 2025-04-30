using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public GameObject resume_button;
    public GameObject options_button;
    public GameObject quit_button;
    public RectTransform healthBar; // Reference to the health bar's RectTransform
    public RectTransform itemSlot1Panel; // Reference to the first item slot panel's RectTransform
    public RectTransform itemSlot2Panel; // Reference to the second item slot panel's RectTransform

    void Update()
    {
        // Position the resume button
        Vector3 resume_position = resume_button.transform.position;
        resume_position.x = Screen.width / 2;
        resume_position.y = Screen.height / 2 + 110;
        resume_button.transform.position = resume_position;

        // Position the quit button
        Vector3 quit_position = quit_button.transform.position;
        quit_position.x = Screen.width / 2;
        quit_position.y = Screen.height / 2 - 70;
        quit_button.transform.position = quit_position;

        // Position the options button
        Vector3 options_position = options_button.transform.position;
        options_position.x = Screen.width / 2;
        options_position.y = Screen.height / 2 + 20;
        options_button.transform.position = options_position;

        // Position the health bar in the lower-left corner
        if (healthBar != null)
        {
            // Anchor the health bar to the bottom-left corner
            healthBar.anchorMin = new Vector2(0, 0); // Bottom-left corner
            healthBar.anchorMax = new Vector2(0, 0); // Bottom-left corner
            healthBar.pivot = new Vector2(0, 0);     // Pivot at the bottom-left corner

            // Offset the health bar by 20 pixels from the left and bottom
            healthBar.anchoredPosition = new Vector2(20, 10);
        }

        if (itemSlot1Panel != null)
        {
            // Anchor the first item slot panel to the bottom-right corner
            itemSlot1Panel.anchorMin = new Vector2(1, 0); // Bottom-right corner
            itemSlot1Panel.anchorMax = new Vector2(1, 0); // Bottom-right corner
            itemSlot1Panel.pivot = new Vector2(1, 0);     // Pivot at the bottom-right corner

            // Offset the first item slot panel by 20 pixels from the right and 10 pixels from the bottom
            itemSlot1Panel.anchoredPosition = new Vector2(-110, 10);
        }
        
        if (itemSlot2Panel != null)
        {
            // Anchor the second item slot panel to the bottom-right corner
            itemSlot2Panel.anchorMin = new Vector2(1, 0); // Bottom-right corner
            itemSlot2Panel.anchorMax = new Vector2(1, 0); // Bottom-right corner
            itemSlot2Panel.pivot = new Vector2(1, 0);     // Pivot at the bottom-right corner

            // Offset the second item slot panel by 20 pixels from the right and 10 pixels from the bottom
            itemSlot2Panel.anchoredPosition = new Vector2(-20, 10);
        }
    }
}