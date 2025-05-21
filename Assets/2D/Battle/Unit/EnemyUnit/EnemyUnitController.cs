using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyUnitController : DeployedUnitController
{
    [SerializeField] protected AntigenController antigenPrefab;
    [SerializeField] protected Transform antigenDroppedLocation;
    
    public override void InitializeUnit(UnitData unitSO, List<StatModifier> modifiers, Route initialRoute)
    {
        currentWaypointIndex = initialRoute.GetWaypointsLength();
        base.InitializeUnit(unitSO, modifiers, initialRoute);
        this.transform.localEulerAngles = new Vector3(0, 180, 0); // rotate for enemy
    }
    protected override void GetNewWaypoint(bool isStart, bool isReturning = false)
    {
        if (!isStart)
        {
            currentWaypointIndex--;
        }

        base.GetNewWaypoint(isStart, isReturning);
    }
    public override void SetNewRoute(Route route)
    {
        currentWaypointIndex = route.GetWaypointsLength();
        currentRouteTaken = route;
        GetNewWaypoint(true, isReturning);
    }

    protected override void Death()
    {
        AntigenController droppedAntigen = Instantiate(antigenPrefab, antigenDroppedLocation);
        droppedAntigen.InitializeAntigen(25);
        droppedAntigen.transform.parent = null;

        base.Death();
    }
}
