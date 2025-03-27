using System;
using UnityEngine;

public class GameScore
{
    public int baseScore = 100;
    public int comboMultipier = 2;
    private int currentScore;
    private int comboCount;

    public Action<int> onScoreUpdated;

    public void UpdateScore()
    {
        currentScore += baseScore;
        onScoreUpdated?.Invoke(currentScore);
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void SetCurrentScore(int score)
    {
        currentScore = score;
    }

    public void ResetScore()
    {
        currentScore = 0;
        comboCount = 0;
        onScoreUpdated?.Invoke(currentScore);
    }

    public bool IsCombo()
    {
        return comboCount >= 2;
    }
}
