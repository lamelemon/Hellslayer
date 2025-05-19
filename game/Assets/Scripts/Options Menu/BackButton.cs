using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static bool JustVisitedOptions = false;

    public void back()
    {
        if (SceneTracker.lastScene == "L-tests")
        {
            JustVisitedOptions = true; // Set the flag to indicate that the options menu was just visited
        }

        else
        {
            JustVisitedOptions = false; // Reset the flag for other scenes
        }

        if (!string.IsNullOrEmpty(SceneTracker.lastScene))
        {
            SceneManager.UnloadSceneAsync("Options");
        }
    }
}
