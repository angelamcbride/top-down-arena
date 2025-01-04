using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : SingletonPersistant<GameStateManager>
{
	public void StartGame()
	{
		SceneManager.LoadScene("Arena");
	}
	public void QuitGame()
	{
		Application.Quit();
	}
	private void Start()
	{

    }
}
