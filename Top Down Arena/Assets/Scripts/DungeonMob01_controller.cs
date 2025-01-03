﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMob01_controller : MonoBehaviour {

	public float mySpeed = 2f;
	public float jumpSpeed = 3f;
	public float collideDamage = -6f;
	public float DamageMultiplier = 5f;
	public string AmmoType = "greenMagic";
	public float AmmoSpeed = 8f;
	public float FireRate = .5f;
	public float TimeBetweenRandomStates = 2f;

	private GameObject player;
	private GameObject obsticle;
	private Rigidbody2D rb2d;
	private Animator anim;
	private float currentSpeed;
	private string state;
	private float shootTimer = 0;
	private float stateTimer = 0;
	private float randomMoveTimer = 0;
	private Vector2 movement_vector;
	private List<string> randomActions = new List<string>(new string[] { "chasePlayer", "runAway", "moveRandom" });

	private Transform childWithTag(string tag)
	{
		Transform childFound = null;
		foreach (Transform child in transform)
			if (child.CompareTag (tag))
			{
				childFound = child;
			} 
		if (childFound == null)
		{
			Debug.Log("no child found");
		}
		return childFound;
	}

	Vector2 lookAtPlayer()
	{
		Vector2 difference = player.transform.position - transform.root.position - new Vector3(0,.5f,0); //difference between mouse pos and character.
		difference.Normalize(); //make 0-1
		return new Vector2 (difference.x,difference.y);
	}

	float DistanceFromPlayer()
	{
		return Vector2.Distance(player.transform.position, transform.root.position);
	}

	private void chasePlayer()
	{
		if (DistanceFromPlayer() < 2f && DistanceFromPlayer() > 0f ) //slow down during approach
		{
			movement_vector = lookAtPlayer ();
			rb2d.MovePosition (rb2d.position + (Vector2.ClampMagnitude (movement_vector, 1) * Time.deltaTime * currentSpeed/1.7f));
		}

		if (DistanceFromPlayer() > 1f && DistanceFromPlayer() > 2f)
		{
			movement_vector = lookAtPlayer ();
			rb2d.MovePosition (rb2d.position + (Vector2.ClampMagnitude (movement_vector, 1) * Time.deltaTime * currentSpeed));
		}
	}
	private IEnumerator jump()
	{
		anim.Play ("mob_jump_anim");
		currentSpeed = jumpSpeed;
		this.gameObject.layer = 12; //Jumping layer
		yield return new WaitForSeconds(.9f);
		currentSpeed = mySpeed;
		this.gameObject.layer = 8; //Normal player layer
	}

	private void shootPlayer()
	{
		if (Time.time > shootTimer) //timer is up
		{
			shootTimer = Time.time + FireRate; //add time to timer
			Transform shootFrom = childWithTag ("ShootFrom");
			GameObject bullet = (GameObject)Instantiate (Resources.Load ("prefabs/ammo/ammo_" + AmmoType));
			Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), GetComponent<Collider2D> (), true);
			bullet.transform.position = shootFrom.position;
			bullet.GetComponent<ammo_controller> ().shootAmmo (DamageMultiplier, AmmoSpeed, AmmoType, lookAtPlayer ());
		}
	}
	private void moveRandom() //randomly move a direction
	{
		if (Time.time > randomMoveTimer) //timer is up, change directions.
		{
			randomMoveTimer = Time.time + 1f; //add time to timer
			movement_vector = new Vector2 (Random.Range (-1f, 1f),Random.Range (-1f, 1f)); //pick random x,y coordinates
			movement_vector.Normalize(); //make 0-1
		}
		rb2d.MovePosition (rb2d.position + (Vector2.ClampMagnitude (movement_vector, 1) * Time.deltaTime * currentSpeed));
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.layer == 11 || other.gameObject.layer == 9) //if on jumpable layer
		{
			StartCoroutine(jump ());
		}
		if (other.gameObject.tag == "Player")
		{
			state = "runAway";
		}
		if (other.gameObject.tag == "Player")
		{
			health_controller healthScript = other.gameObject.GetComponent<health_controller> ();
			if (healthScript != null) //Sometimes health script is null because object is in the process of dying. If it isn't dead/null we can access its health script.
			{
				healthScript.addHealth (collideDamage);
			} else
			{
				Debug.Log ("health script null");
			}
		}
		else //if mob hits a wall, pick a random direction to move in.
		{
			state = "moveRandom";
			randomMoveTimer = 0; //reset timer
		}
	}

	private void runAway()
	{
		
		Vector2 movement_vector = lookAtPlayer ();
		rb2d.MovePosition (rb2d.position - (Vector2.ClampMagnitude (movement_vector, 1) * Time.deltaTime * currentSpeed));
	}

	private void actionPattern() //randomize state the mob is in
	{
		if (Time.time > stateTimer) //timer is up
		{
			stateTimer = Time.time + TimeBetweenRandomStates; //add time to timer
			state = randomActions [Random.Range (0, randomActions.Count)];
		}
	}
		
	private void Start () 
	{
		player = GameObject.FindGameObjectsWithTag("Player1")[0];
		rb2d = transform.parent.GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		currentSpeed = mySpeed;
		stateTimer = Time.time - TimeBetweenRandomStates;
	}

	private void FixedUpdate()  
	{
		switch(state)
		{
		case "chasePlayer":
			chasePlayer ();
			break;
		case "runAway":
			runAway();
			break;
		case "moveRandom":
			moveRandom();
			break;
		default:
			chasePlayer ();
			break;
		}
			
	}

	private void Update () 
	{
		actionPattern ();
		shootPlayer ();
	}
}