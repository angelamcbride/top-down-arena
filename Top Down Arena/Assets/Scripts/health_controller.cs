using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health_controller : MonoBehaviour 
{
	public float Health = 100f;
	private float healthLeft;
	private Transform healthBar;


	public void addHealth(float amount)
	{
		healthLeft += amount;
		healthBar.GetChild(0).localScale = new Vector3 (healthLeft / Health,1,1);

		if (healthLeft <= 0) //If player/mob runs out of health
		{
			if (this.tag == "Player")
			{
				Debug.Log ("You died.");
			} 
			else
			{
				GameObject death = (GameObject)Instantiate (Resources.Load ("prefabs/fx/DeathSprite"));
				death.transform.position = transform.position;
				Object.Destroy (transform.gameObject);
			}
		}
	}

	private Transform childWithTag(string tag)
	{
		Transform childFound = null;
		foreach (Transform child in transform)
			if (child.CompareTag (tag))
			{
				childFound = child;
			} 
		return childFound;
	}

	void Start()
	{
		healthBar = childWithTag ("healthBar");
		healthLeft = Health;
	}
}
