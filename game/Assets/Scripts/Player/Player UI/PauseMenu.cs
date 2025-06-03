using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] PlayerInputManager getInput;

    public GameObject crosshair;
    public GameObject pauseMenuUI; // Assign the pause menu UI in the Inspector
    public static bool isPaused = false;

    void Start()
    {
        // Ensure the pause menu is hidden at the start of the game
        if (pauseMenuUI != null && BackButton.JustVisitedOptions == false)
        {
            Resume(); // Hide the pause menu and resume the game
        }

        else if (BackButton.JustVisitedOptions == true)
        {
            Pause();
            BackButton.JustVisitedOptions = false; // Reset the flag after using it
        }
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
        getInput.OnEnable();
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
        getInput.DisableExptMenu();
        pauseMenuUI.SetActive(true); // Show the pause menu
        crosshair.SetActive(false); // Hide the crosshair
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;

        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void MainMenu()
    {
        SceneTracker.lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Main_Menu"); // Load the main menu scene
    }

    public void optionsMenu()
    {
        SceneTracker.lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync("Options", LoadSceneMode.Additive);
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
