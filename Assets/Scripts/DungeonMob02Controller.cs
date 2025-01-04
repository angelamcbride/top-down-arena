using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMob02Controller : BaseMobController
{
	
	private void Start () 
	{
		player = GameObject.FindGameObjectsWithTag("Player1")[0];
		rb2d = transform.parent.GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		currentSpeed = mySpeed;
		state = "chasePlayer";
	}

    protected override void MobSpecificStart()
    {
        state = "chasePlayer";
    }

    protected override void MoveRandom() //randomly move a direction
    {
        if (Time.time > randomMoveTimer) //timer is up, go back to chasing.
        {
            state = "chasePlayer";
        }
        else
        {
            rb2d.MovePosition(rb2d.position + (Vector2.ClampMagnitude(movement_vector, 1) * Time.deltaTime * currentSpeed));
        }
    }

    protected override void WhenMobHitsPlayer()
    {
        state = "moveRandom";
        randomMoveTimer = Time.time + 0.25f; //add time to timer
        movement_vector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); //pick random x,y coordinates
        movement_vector.Normalize(); //make 0-1
    }

    protected override void WhenMobHitsWall()
    {
        state = "moveRandom";
        randomMoveTimer = Time.time + 1f; //add time to timer
        movement_vector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); //pick random x,y coordinates
        movement_vector.Normalize(); //make 0-1
    }
}
