using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : SingletonNonPersistent<GameStateManager>
{
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    private void Start()
    {
        LoadPlayerPrefs();
        AudioManager.Instance.StartAudioManager();
    }

    private void LoadPlayerPrefs()
    {
        int musicPref = PlayerPrefs.GetInt("Music", 0);
        //Debug.Log("Music pref = " + musicPref);
        if (musicPref == default || musicPref == 0) // There is no bool for player prefs, so I am using an int. 0 = enabled
        {
            AudioManager.Instance.musicEnabledPref = true;
        }
        else
        {
            AudioManager.Instance.musicEnabledPref = false;
        }
        UIManager.Instance.SetMusicCheckbox(AudioManager.Instance.musicEnabledPref);
    }

    public void StartGame()
	{
        if (isPaused) // unpause if paused
        {
            TogglePause();
            UIManager.Instance.ToggleDeathMenu(false);
        }
        SceneManager.LoadScene("Arena");
	}

	public void QuitGame()
	{
        ScoreManager.Instance.SaveScore(); // Save the current score
        Application.Quit();
	}

    public void RestartGame()
    {
        ScoreManager.Instance.SaveScore();
        ScoreManager.Instance.ResetScore();
        StartGame();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    public void GoToStartMenu()
    {
        if(isPaused) // unpause if paused
        {
            TogglePause();
        }
        ScoreManager.Instance.SaveScore();
        ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene("StartMenu");
    }

    public void PlayerDied()
    {
        isPaused = true;
        Time.timeScale = 0f; // Pause the game
        UIManager.Instance.ToggleDeathMenu(true);
        ScoreManager.Instance.SaveScore();
        UIManager.Instance.UpdateScoreUI(ScoreManager.Instance.score);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        UIManager.Instance.TogglePauseMenu(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f; // Pause the game
            AudioManager.Instance.StopMusic();
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
            if(AudioManager.Instance.musicEnabledPref == true)
            {
                AudioManager.Instance.PlayMusic();
            }
        }
    }
}
