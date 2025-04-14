using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] PlayerInputManager getInput;

    public GameObject crosshair;
    public GameObject pauseMenuUI; // Assign the pause menu UI in the Inspector
    public static bool isPaused = false;

    void Start()
    {
        // Ensure the pause menu is hidden at the start of the game
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Lock and hide the cursor at the start of the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (getInput.MenuInput.WasPressedThisFrame())
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        crosshair.SetActive(true); // Show the crosshair
        Time.timeScale = 1f; // Resume game time
        isPaused = false;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        crosshair.SetActive(false); // Hide the crosshair
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;

        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); // Quit the application
    }

    public void ResumeButton()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume game time
        isPaused = false;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
