using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour 
{
	public float Health = 100f;
	private float healthLeft;
	private Transform healthBar;


	public void AddHealth(float amount)
	{
        if (amount < 0)
        {
            if (tag == "Player")
            {
                AudioManager.Instance.PlaySound("PlayerHit");
            }
            else
            {
                AudioManager.Instance.PlaySound("Hit");
            }
        }

		if (healthLeft <= 0) //If player/mob runs out of health
		{
			if (tag == "Player")
			{
				Debug.Log ("You died.");
                AudioManager.Instance.PlaySound("PlayerDeath");
                GameObject death = (GameObject)Instantiate(Resources.Load("prefabs/fx/DeathSprite"));
                death.transform.position = transform.position;
            } 
			else
			{
				GameObject death = (GameObject)Instantiate (Resources.Load ("prefabs/fx/DeathSprite"));
				death.transform.position = transform.position;
				Destroy (transform.gameObject);
                AudioManager.Instance.PlaySound("MobDeath");
            }
		}
        else
        {
            healthLeft += amount;
            healthBar.GetChild(0).localScale = new Vector3(healthLeft / Health, 1, 1);
        }
	}

	private Transform ChildWithTag(string tag)
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
		healthBar = ChildWithTag ("healthBar");
		healthLeft = Health;
	}
}
