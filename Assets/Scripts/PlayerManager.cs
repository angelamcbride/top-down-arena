using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int PlayerID { get; private set; } // Unique ID for each player
    public int Score { get; private set; } // Player's score

    public void InitializePlayer(int id)
    {
        PlayerID = id;
        Score = 0;
    }

    public void AddScore(int amount)
    {
        Score += amount;
        Debug.Log($"Player {PlayerID}'s Score: {Score}");
    }
}
