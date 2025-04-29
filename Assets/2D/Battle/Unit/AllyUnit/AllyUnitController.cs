using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class AllyUnitController : DeployedUnitController
{
    [SerializeField] protected GameObject selectedIndicator;

    public override void InitializeUnit(UnitData unitSO, List<StatModifier> modifiers, Route initialRoute)
    {
        currentWaypointIndex = 0;
        base.InitializeUnit(unitSO, modifiers, initialRoute);
    }
    protected override void GetNewWaypoint(bool isStart, bool isReturning = false)
    {
        if (!isStart)
        {
            if (isReturning)
            {
                currentWaypointIndex--;
            }
            else
            {
                currentWaypointIndex++;
            }
        }

        base.GetNewWaypoint(isStart, isReturning);
    }
    public override void SetNewRoute(Route route)
    {
        base.SetNewRoute(route);
        currentWaypointIndex = 0;
    }
    public void UnitSelect()
    {
        selectedIndicator.SetActive(true);
    }
    public void UnitDeselect()
    {
        selectedIndicator.SetActive(false);
    }
}
