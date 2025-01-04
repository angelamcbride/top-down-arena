using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMob01Controller : BaseMobController
{
    protected override void MobSpecificStart()
    {
        stateTimer = Time.time - TimeBetweenRandomStates;
    }

    protected override void Update()
    {
        ActionPattern();
        ShootPlayer();
    }
}