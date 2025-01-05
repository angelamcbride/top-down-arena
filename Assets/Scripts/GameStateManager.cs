using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : SingletonNonPersistent<GameStateManager>
{
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    public void StartGame()
	{
        if (isPaused) // unpause if paused
        {
            TogglePause();
        }
        SceneManager.LoadScene("Arena");
	}

	public void QuitGame()
	{
        ScoreManager.Instance.SaveScore(); // Save the current score
        Application.Quit();
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
            AudioManager.Instance.PlayMusic("BackgroundMusic");
        }
    }
}
