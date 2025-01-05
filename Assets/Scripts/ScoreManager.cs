using UnityEngine;

public class ScoreManager : SingletonNonPersistent<ScoreManager>
{
    private int score = 0;

    public int GetScore()
    {
        return score;
    }

    public void AddScore(int amount)
    {
        score += amount;
        UIManager.Instance.UpdateScoreUI(score); // Notify the UI Manager
        SaveScore();
    }

    public void SaveScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0); // Retrieve the saved high score
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score); // Save the new high score
            PlayerPrefs.Save();
        }
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void ResetHighScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
        UIManager.Instance.DisplayHighScore();
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0); // Retrieve the saved high score
    }
}
