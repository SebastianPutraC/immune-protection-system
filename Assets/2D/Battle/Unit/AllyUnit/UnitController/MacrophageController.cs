using System;
using System.Collections.Generic;
using UnityEngine;

public class MacrophageController : AllyUnitController, IInflammator, IPresenter
{
    private int resourcePointWhenDamaged = 20;

    [SerializeField] private DetectionTrigger antigenDetect;
    [SerializeField] private Transform antigenCarryPosition;
    private AntigenController carriedAntigen;

    [SerializeField] private ParticleSystem inflammationEffect;

    public Action<int> InflammatoryActivated { get; set; }
    public Action<int> AntigenPresented { get; set; }

    protected override void Awake()
    {
        base.Awake();
        antigenDetect.enemyDetected += GetAntigen;
    }
    protected override void Update()
    {
        if (noMoreRoute && isReturning && carriedAntigen != null)
        {
            ArrivedAtBase();
        }
        base.Update();
        if (isDetecting)
        {
            if (meleeCooldown > 0)
            {
                meleeCooldown -= Time.deltaTime;
            }
            else
            {
                PhagocytosisAttack();
                meleeCooldown = UnitSO.UnitAtkSpeed;
            }
        }
    }

    private void PhagocytosisAttack()
    {
        if (targetEnemyUnitMelee != null)
        {
            targetEnemyUnitMelee.DealDamage(unitAtk.ReturnFinalValue());
        }
    }

    public void GetAntigen(UnitController antigen)
    {
        if (antigen is not AntigenController)
        {
            return;
        }
        else
        {
            AntigenController carriedAntigen = (AntigenController)antigen;
            carriedAntigen.ObjectCarried();
            carriedAntigen.transform.parent = antigenCarryPosition;
            carriedAntigen.transform.position = antigenCarryPosition.position;

            ReturnToBase();
        }
    }
    public void ReturnToBase()
    {
        isReturning = true;
        GetNewWaypoint(false, isReturning);
    }
    public void ArrivedAtBase()
    {
        AntigenPresented?.Invoke(carriedAntigen.intelAmount);
        Destroy(this.gameObject);
    }

    public override void DealDamage(float damage)
    {
        base.DealDamage(damage);
        InflammatoryActivated?.Invoke(resourcePointWhenDamaged);
        inflammationEffect.Play();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        antigenDetect.enemyDetected -= GetAntigen;
    }
}
