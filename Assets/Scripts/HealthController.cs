using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour 
{
	public float Health = 100f;
	private float healthLeft;

    private IEnumerator FlashRed(GameObject Entity, float flashDuration = 0.1f, int flashCount = 2)
    {
        SpriteRenderer sprite = Entity.GetComponent<SpriteRenderer>();
        for (int i = 0; i < flashCount; i++)
        {
            sprite.color = Color.red;
            yield return new WaitForSeconds(flashDuration);

            sprite.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    public void ModifyHealth(float amount)
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
            StartCoroutine(FlashRed(transform.parent.gameObject));
        }

		if (healthLeft <= 0) // If player/mob runs out of health
		{
			if (transform.parent.tag == "Player")
			{
				Debug.Log ("You died.");
                AudioManager.Instance.PlaySound("PlayerDeath");
                GameObject death = (GameObject)Instantiate(Resources.Load("prefabs/fx/DeathSprite"));
                death.transform.position = transform.parent.position;
                GameStateManager.Instance.PlayerDied();
            } 
			else
			{
				GameObject death = (GameObject)Instantiate (Resources.Load ("prefabs/fx/DeathSprite"));
				death.transform.position = transform.parent.position;

                BaseMobController mobController = GetComponentInParent<BaseMobController>();
                if (mobController != null)
                {
                    mobController.Die(); // Tells the mob to handle its own death
                }

                AudioManager.Instance.PlaySound("MobDeath");
                ScoreManager.Instance.AddScore(1);
            }
		}
        else
        {
            healthLeft = Mathf.Clamp(healthLeft + amount, 0, 100);
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
