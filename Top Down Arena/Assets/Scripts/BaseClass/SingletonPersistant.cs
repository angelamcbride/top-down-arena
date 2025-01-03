using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonPersistant<T> : MonoBehaviour where T: Component 
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<T> ();
				if (instance == null)
				{
					Debug.Log ("You are missing a singletonPersistant in your scene.");
				//	var obj = new GameObject ();                  //Safety mesure off
				//	instance = obj.AddComponent<T> ();
				}
			}
			return instance;
		}
		set
		{
			instance = value;
		}
	}

	private void Awake()
	{
		DontDestroyOnLoad (this);
		if (instance == null)
		{
			instance = this as T;
		} 
		else
		{
			Destroy (gameObject);
		}
	}
}
