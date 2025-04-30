using UnityEngine;

public class BackButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void back()
    {
        if (!string.IsNullOrEmpty(SceneTracker.lastScene))
        {
            SceneManager.LoadScene(SceneTracker.lastScene);
        }
    }
}
