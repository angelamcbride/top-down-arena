using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm_rotation : MonoBehaviour {

	//private string direction = "down";
	private float pxWidth = 0.0625f; //pixel size
	private int armOffsetNormalY = -0; //in pixels
	private int armOffsetflopY = -0; //in pixels
	private int armOffsetNormalX = -0; //in pixels
	private float armOffsetFlopX = 6; //in pixels

	private bool armRotateOn = true;

	private SpriteRenderer sprite;

	void lookAtDir(float rotation) //Determine the direction char/arm is aiming. Flop/layer the arm accordiningly.
	{
		if (rotation < 45 && rotation > -45)//1:30 -> 4:30, char looking right. 
		{
			transform.localScale = new Vector3(1, 1, 1);
			sprite.sortingOrder = 4;
			transform.localPosition = new Vector2 (armOffsetNormalX, armOffsetNormalY)*pxWidth;
		} 
		else if (rotation < -45 && rotation > -100)//4:30 -> 7:30, char looking down. 
		{
			transform.localScale = new Vector3(1, 1, 1);
			sprite.sortingOrder = 4;
			transform.localPosition = new Vector2 (armOffsetNormalX, armOffsetNormalY)*pxWidth;
		} 
		else if (rotation < 135 && rotation > 45) //char looking up
		{
			transform.localScale = new Vector3(1, -1, 1);
			sprite.sortingOrder = 1;
			transform.localPosition = new Vector2 (armOffsetFlopX, armOffsetflopY)*pxWidth;
		} 
		else //char looking left
		{
			transform.localScale = new Vector3(1, -1, 1);
			sprite.sortingOrder = 0;
			transform.localPosition = new Vector2 (armOffsetNormalX, armOffsetNormalY)*pxWidth;
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
		transform.rotation = Quaternion.Euler (0f, 0f, rotationZ);
		lookAtDir (rotationZ);
	}

	void Update () 
	{
		if (armRotateOn)
		{
			setRotation ();
		} 
		else
		{
			transform.rotation = Quaternion.Euler (0f, 0f, 0f); //dont really need this set every frame...
		}
	}
}
