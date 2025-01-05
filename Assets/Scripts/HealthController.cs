﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour 
{
	public float Health = 100f;
	private float healthLeft;

	public void AddHealth(float amount)
	{
        if (amount < 0)
        {
            if (transform.parent.tag == "Player")
            {
                AudioManager.Instance.PlaySound("PlayerHit");
            }
            else
            {
                AudioManager.Instance.PlaySound("Hit");
            }
        }

		if (healthLeft <= 0) // If player/mob runs out of health
		{
			if (transform.parent.tag == "Player")
			{
				Debug.Log ("You died.");
                AudioManager.Instance.PlaySound("PlayerDeath");
                GameObject death = (GameObject)Instantiate(Resources.Load("prefabs/fx/DeathSprite"));
                death.transform.position = transform.parent.position;
            } 
			else
			{
				GameObject death = (GameObject)Instantiate (Resources.Load ("prefabs/fx/DeathSprite"));
				death.transform.position = transform.parent.position;
				Destroy(transform.parent.parent.gameObject); // Not great if Hierarchy changes..
                AudioManager.Instance.PlaySound("MobDeath");
                ScoreManager.Instance.AddScore(1);
            }
		}
        else
        {
            healthLeft += amount;
            Vector3 healthBarScale = new Vector3(healthLeft / Health, 1, 1);
            transform.GetChild(0).localScale = healthBarScale;
            if (transform.parent.tag == "Player")
            {
                UIManager.Instance.ScaleHealthBar(healthBarScale);
            }
        }
	}

    void Start()
	{
		healthLeft = Health;
	}
}
