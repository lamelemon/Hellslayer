using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [HideInInspector] public int CurrentScore { get; private set; } = 0;
    [HideInInspector] public int SumScore = 0;

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        SumScore += amount;
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
        SumScore += amount; // optional, depends on your logic
    }
}

