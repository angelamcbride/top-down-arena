using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun_shoot : MonoBehaviour {

	public float speed = 10;
	public float damageMultiplier = 5;
	public string ammoType = "silverBullet";

	Vector2 playerLookVector()
	{
		Vector2 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.root.position; //difference between mouse pos and character.
		difference.Normalize(); //make 0-1
		return new Vector2 (difference.x,difference.y);
	}

	void playerInputs()
	{
		if (Input.GetMouseButtonDown (0))
		{
			Transform shootFrom = this.gameObject.transform.GetChild(0);
			GameObject bullet = (GameObject)Instantiate (Resources.Load ("prefabs/ammo/ammo_" + ammoType));
			bullet.transform.position = shootFrom.position;
			bullet.GetComponent<ammo_controller>().shootAmmo(damageMultiplier, speed, ammoType, playerLookVector());
		}
	}

	void Update () 
	{
		playerInputs ();
	}
}
