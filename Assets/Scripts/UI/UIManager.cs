using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : SingletonNonPersistent<UIManager>
{
    public TMP_Text scoreText;
    public Text highScoreText;
    public GameObject healthBarUI;
    public GameObject pauseMenu;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            DisplayHighScore();
        }
    }

    public void UpdateScoreUI(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = newScore.ToString();
        }
        else
        {
            Debug.Log("The score UI is null. Did you remember to assign in the inspector?");
        }
    }

    public void ScaleHealthBar(Vector3 healthBarScale)
    {
        if(healthBarUI != null)
        {
            healthBarUI.transform.localScale = healthBarScale;
        }
        else
        {
            Debug.Log("The health bar UI is null. Did you remember to assign in the inspector?");
        }
    }

    public void TogglePauseMenu(bool isPaused)
    {
        pauseMenu.SetActive(isPaused);
    }

    public void DisplayHighScore()
    {
        highScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }
}
