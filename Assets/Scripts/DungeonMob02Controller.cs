using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMob02Controller : BaseMobController
{
    private float runAwayTimer = 0;

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

    protected override void RunAway()
    {
        if (Time.time > runAwayTimer) //timer is up, go back to chasing.
        {
            state = "chasePlayer";
        }
        else
        {
            movement_vector = LookAtPlayer(transform.root.position);
            rb2d.AddForce(-Vector2.ClampMagnitude(movement_vector, 1) * currentSpeed * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    protected override void WhenMobHitsPlayer(GameObject player)
    {
        StartCoroutine(Jump());
        state = "runAway";
        runAwayTimer = Time.time + 0.25f; // Run away for 0.25 seconds
        HealthController healthScript = player.transform.ChildWithTag("healthBar").GetComponent<HealthController>();
        if (healthScript != null) //Sometimes health script is null because object is in the process of dying. If it isn't dead/null we can access its health script.
        {
            healthScript.ModifyHealth(collideDamage);
        }
        else
        {
            Debug.Log("health script null");
        }
    }

    protected override void WhenMobHitsWall()
    {
        state = "moveRandom";
        randomMoveTimer = Time.time + 1f; // Move in a random direction for 1 second.
        movement_vector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); // pick random x,y coordinates
        movement_vector.Normalize(); //make 0-1
    }
}
