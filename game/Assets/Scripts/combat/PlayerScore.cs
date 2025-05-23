using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int CurrentScore { get; private set; } = 0;

    public void AddScore(int amount)
    {
        CurrentScore += amount;
    }

    public void SubScore(int amount)
    {
        CurrentScore -= amount;
    }

    public void ResetScore()
    {
        CurrentScore = 0;
    }

    public void SetScore(int amount)
    {
        CurrentScore = amount;
    }
}
