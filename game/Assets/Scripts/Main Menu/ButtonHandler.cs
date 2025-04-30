using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public GameObject optionsPanel;

    public void StartGame()
    {
        Debug.Log("Start clicked!");
        SceneManager.LoadScene("L-tests");
    }

    public void OpenOptions()
    {
        Debug.Log("Options clicked!");
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
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
