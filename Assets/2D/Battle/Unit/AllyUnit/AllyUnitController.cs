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
        unitAudioSource.time = 0.5f;
        unitAudioSource.clip = spawnSound;
        unitAudioSource.Play();
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
        if (isReturning)
        {
            currentWaypointIndex = route.GetWaypointsLength();
        }
        else
        {
            currentWaypointIndex = 0;
        }
        currentRouteTaken = route;
        GetNewWaypoint(true, isReturning);
    }

    public void AddBuff(StatModifier modifier)
    {
        switch (modifier.StatToMod)
        {
            case StatType.ATK:
                if (unitAtk.GetStatFromSource(modifier.Source) == null)
                {
                    unitAtk.AddModifier(modifier);
                }
                break;
            case StatType.RANGEDATK:
                if (unitRangedAtk.GetStatFromSource(modifier.Source) == null)
                {
                    unitRangedAtk.AddModifier(modifier);
                }
                break;
        }
    }
    public void RemoveBuff(StatType type, int bufferId)
    {
        switch (type)
        {
            case StatType.ATK:
                unitAtk.RemoveModifersFromSource(bufferId);
                break;
            case StatType.RANGEDATK:
                unitRangedAtk.RemoveModifersFromSource(bufferId);
                break;
        }
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
