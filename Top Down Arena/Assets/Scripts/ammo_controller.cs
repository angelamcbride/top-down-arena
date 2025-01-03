using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammo_controller : MonoBehaviour 
{
	public float damage = 5f;
	public float distanceMax = 10f;
	public string myAmmoType = "silverBullet";

	private bool shoot;
	private float CalculatedDamage;
	private float mySpeed;
	private Vector2 startPos;
	private Vector2 shootVector;

	private Rigidbody2D rb2d;

	public void shootAmmo(float damageMultiplier, float speed, string ammoType, Vector2 playerLookVector)
	{
		if (ammoType == myAmmoType && shoot == false)
		{
			CalculatedDamage = damage * damageMultiplier;
			mySpeed = speed;
			startPos = this.transform.position;
			shootVector = playerLookVector;
			shoot = true;
		}
	}

	void FixedUpdate()
	{
		if (shoot == true)
		{
			float distance = Vector2.Distance(startPos,this.transform.position);
			if (distance > distanceMax)
			{
				Object.Destroy (this.gameObject);
			} 
			else
			{
				rb2d.MovePosition (rb2d.position + (Vector2.ClampMagnitude (shootVector, 1) * Time.deltaTime * mySpeed));
			}
		}
	}

	void Start () 
	{
		rb2d = this.GetComponent<Rigidbody2D>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
		{
			health_controller healthScript = other.transform.root.GetComponent<health_controller> ();
			if (healthScript != null) //Sometimes health script is null because object is in the process of dying. If it isn't dead/null we can access its health script.
			{
				healthScript.addHealth (CalculatedDamage);
			}
		}
		Object.Destroy (this.gameObject);
	}
}
