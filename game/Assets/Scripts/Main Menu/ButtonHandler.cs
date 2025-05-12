using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{

    public void StartGame()
    {
        Debug.Log("Start clicked!");
        SceneManager.LoadScene("L-tests");
    }

    public void OpenOptions()
    {
        SceneTracker.lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Options"); // Load the options menu scene
    }

    public void QuitGame()
    {
        Debug.Log("Quit clicked!");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
