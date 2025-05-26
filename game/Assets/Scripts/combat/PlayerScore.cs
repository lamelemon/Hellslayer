using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [HideInInspector] public int CurrentScore { get; private set; } = 0; // Resets when playing
    [HideInInspector] public int SumScore = 0; // Dosent reset when playing

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        SumScore += CurrentScore;
    }

    public void SubScore(int amount)
    {
        CurrentScore -= amount;
        SumScore -= amount;
    }

    public void ResetScore()
    {
        CurrentScore = 0;
    }

    public void SetScore(int amount)
    {
        CurrentScore = amount;
        SumScore = amount;
    }
}
