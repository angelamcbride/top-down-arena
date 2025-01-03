using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Component 
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
					Debug.Log ("You are missing a singleton in your scene.");
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
}
