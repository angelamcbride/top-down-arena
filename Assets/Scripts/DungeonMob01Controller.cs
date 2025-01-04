using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMob01Controller : BaseMobController
{
    protected override void MobSpecificStart()
    {
        stateTimer = Time.time - TimeBetweenRandomStates;
    }

    protected override void ChasePlayer()
    {
        movement_vector = LookAtPlayer();
        rb2d.MovePosition(rb2d.position + (Vector2.ClampMagnitude(movement_vector, 1) * Time.deltaTime * currentSpeed));
    }

    protected override void Update()
    {
        if (DistanceFromPlayer() < 2f && DistanceFromPlayer() > 0f) // Run away if too close
        {
            state = "runAway";
        }
        ActionPattern();
        ShootPlayer();
    }
}