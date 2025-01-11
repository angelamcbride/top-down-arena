using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : SingletonNonPersistent<UIManager>
{
    [SerializeField] private List<TMP_Text> scoreTexts = new List<TMP_Text>();
    [SerializeField] private Text highScoreText;
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject startMenu;
    [SerializeField] public GameObject musicToggle;
    [SerializeField] private GameObject musicButton;

    private Button musicButtonComponent;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            DisplayHighScore();
        }
        if (SceneManager.GetActiveScene().name == "Arena")
        {
            TogglePauseMenu(false);
            ToggleDeathMenu(false);
        }

        // Manually set up the button's click listener
        if (musicButton != null)
        {
            musicButtonComponent = musicButton.GetComponent<Button>();
            if (musicButtonComponent != null)
            {
                musicButtonComponent.onClick.AddListener(OnMusicButtonClicked);
            }
            else
            {
                Debug.LogError("Button component not found on settingsButton GameObject.");
            }
        }
        else
        {
            Debug.LogError("settingsButton GameObject is not assigned in the inspector.");
        }
    }

    private void OnMusicButtonClicked()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void UpdateScoreUI(int newScore)
    {
        if (scoreTexts.Count > 0)
        {
            foreach (TMP_Text textToUpdate in scoreTexts)
            {
                if (textToUpdate != null)
                    textToUpdate.text = newScore.ToString();
            }
        }
        else
        {
            Debug.LogError("The score UI is null. Did you remember to assign in the inspector?");
        }
    }

    public void ScaleHealthBar(Vector3 healthBarScale)
    {
        if (healthBarUI != null)
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

    public void ToggleDeathMenu(bool show)
    {
        deathMenu.SetActive(show);
    }

    public void SetMusicCheckbox(bool musicOn)
    {
        musicToggle.GetComponent<Toggle>().isOn = musicOn;
        //AudioManager.Instance.ToggleMusic();
    }

    public void DisplayHighScore()
    {
        highScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    public void ToggleStartMenu()
    {
        bool startMenuVisible = startMenu.activeInHierarchy;
        startMenu.SetActive(!startMenuVisible);
    }

    public void ToggleSettingsMenu()
    {
        bool settingsMenuVisible = settingsMenu.activeInHierarchy;
        settingsMenu.SetActive(!settingsMenuVisible);
    }

    public void TogglePauseMenu()
    {
        bool pauseMenuVisible = pauseMenu.activeInHierarchy;
        pauseMenu.SetActive(!pauseMenuVisible);
    }
}