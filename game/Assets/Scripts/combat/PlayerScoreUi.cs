using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScoreUi : MonoBehaviour
{
    [Header("Wave UI Settings")]
    public string ScoreTextFormat = "Player Score "; // Format for displaying the wave number
    public TextMeshProUGUI ScoreText;

    public PlayerScore PlayerScore; // Drag in the inspector

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = ScoreTextFormat + PlayerScore.SumScore.ToString();
    }
}
