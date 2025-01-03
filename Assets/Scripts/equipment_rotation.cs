using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class equipment_rotation : MonoBehaviour {

	public float rotationOffset;

	private SpriteRenderer sprite;

	void lookAtDir(float rotation) //Determine the direction char/arm is aiming. Flop/layer the arm accordiningly.
	{
		if (rotation < 45 && rotation > -45)//1:30 -> 4:30, char looking right. 
		{
			sprite.sortingOrder = 3;
		} 
		else if (rotation < -45 && rotation > -100)//4:30 -> 7:30, char looking down. 
		{
			sprite.sortingOrder = 3;
		} 
		else if (rotation < 135 && rotation > 45) //char looking up
		{
			sprite.sortingOrder = 0;
		} 
		else //char looking left
		{
			sprite.sortingOrder = 1;
		}
	}

	void Start()
	{
		sprite = GetComponent<SpriteRenderer> ();
	}

	void setRotation() //Based on mouse position, set the rotation of the arm (aim at mouse).
	{
		Vector3 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.root.position; //difference between mouse pos and character.
		difference.Normalize(); //make 0-1
		float rotationZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg; //find angle in degrees
		lookAtDir (rotationZ);
	}

	void Update () 
	{
		setRotation();
	}
}
