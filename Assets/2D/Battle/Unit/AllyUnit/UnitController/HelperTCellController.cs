using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HelperTCellController : AllyUnitController, IInflammator
{
    private int resourcePointWhenDamaged = 20;

    [SerializeField] private DetectionTrigger allyDetect;
    [SerializeField] protected Dictionary<int, AllyUnitController> detectedUnitBuff = new Dictionary<int, AllyUnitController>();
    private StatModifier meleeModifier;
    private StatModifier rangedModifier;

    private bool isBuffing = false;

    [SerializeField] private ParticleSystem inflammationEffect;

    public Action<int> InflammatoryActivated { get; set; }

    protected override void Awake()
    {
        base.Awake();
        allyDetect.enemyDetected += AllyBuffDetected;
        allyDetect.enemyOutRange += AllyBuffLost;
        meleeModifier = new StatModifier(0.5f, StatModType.PERCENTMULT, StatType.ATK, 1, this.gameObject.GetInstanceID());
        rangedModifier = new StatModifier(-0.4f, StatModType.PERCENTMULT, StatType.ATKCOOLDOWN, 1, this.gameObject.GetInstanceID());
    }

    protected override void Update()
    {
        base.Update();
        if (isBuffing)
        {
            BuffUnit();
        }
    }

    protected void AllyBuffDetected(UnitController unit) // Convert to AllyUnitController
    {
        if(unit is not AllyUnitController)
        {
            return;
        }

        isBuffing = true;
        AllyUnitController allyUnit = (AllyUnitController)unit;

        if (!detectedUnitBuff.ContainsKey(allyUnit.GetInstanceID()))
        {
            detectedUnitBuff.Add(allyUnit.GetInstanceID(), allyUnit);
            allyUnit.ThisUnitKilled += AllyDead;
        }
    }
    protected void AllyBuffLost(UnitController unit)
    {
        if (unit is not AllyUnitController)
        {
            return;
        }

        AllyUnitController allyUnit = (AllyUnitController)unit;
        if (detectedUnitBuff.ContainsKey(allyUnit.GetInstanceID()))
        {
            if (allyUnit is MacrophageController || unit is NeutrophilController)
            {
                allyUnit.RemoveBuff(StatType.ATK, this.gameObject.GetInstanceID());
            }
            else if (unit is PlasmaCellController)
            {
                allyUnit.RemoveBuff(StatType.ATKCOOLDOWN, this.gameObject.GetInstanceID());
            }
            detectedUnitBuff.Remove(allyUnit.GetInstanceID());
        }

        if (detectedUnitBuff.Count <= 0)
        {
            isBuffing = false;
        }
    }
    protected void BuffUnit()
    {
        foreach (AllyUnitController unit in detectedUnitBuff.Values)
        {
            if (unit != null)
            {
                if (unit is MacrophageController || unit is NeutrophilController)
                {
                    unit.AddBuff(meleeModifier);
                }
                else if (unit is PlasmaCellController)
                {
                    unit.AddBuff(rangedModifier);
                }
            }
        }
    }
    protected void AllyDead(UnitController allyUnit)
    {
        allyUnit.ThisUnitKilled -= AllyDead;
        AllyBuffLost(allyUnit);
    }

    public override void DealDamage(float damage)
    {
        base.DealDamage(damage);
        InflammatoryActivated?.Invoke(resourcePointWhenDamaged);
        inflammationEffect.Play();
    }

    protected override void OnDestroy()
    {
        for (int i = 0; i < detectedUnitBuff.Count; i++)
        {
            if (detectedUnitBuff[i] is MacrophageController || detectedUnitBuff[i] is NeutrophilController)
            {
                detectedUnitBuff[i].RemoveBuff(StatType.ATK, this.gameObject.GetInstanceID());
            }
            else if (detectedUnitBuff[i] is PlasmaCellController)
            {
                detectedUnitBuff[i].RemoveBuff(StatType.ATKCOOLDOWN, this.gameObject.GetInstanceID());
            }
        }
        base.OnDestroy();
        allyDetect.enemyDetected -= AllyBuffDetected;
        allyDetect.enemyOutRange -= AllyBuffLost;
    }
}
