using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyUnitController : DeployedUnitController
{
    public override void InitializeUnit(UnitData unitSO, List<StatModifier> modifiers, Route initialRoute)
    {
        currentWaypointIndex = initialRoute.GetWaypointsLength();
        base.InitializeUnit(unitSO, modifiers, initialRoute);
        this.transform.localEulerAngles = new Vector3(0, 180, 0); // rotate for enemy
    }
    protected override void ArrivedAtWaypoint(bool isStart)
    {
        if (!isStart)
        {
            currentWaypointIndex--;
        }

        base.ArrivedAtWaypoint(isStart);
    }
    public override void SetNewRoute(Route route)
    {
        base.SetNewRoute(route);
        currentWaypointIndex = currentRouteTaken.GetWaypointsLength();
    }
}
