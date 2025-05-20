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
    public bool isPresenting { get; set; }

    protected override void Awake()
    {
        base.Awake();
        antigenDetect.enemyDetected += GetAntigen;
        isPresenting = false;
        antigenDetect.gameObject.SetActive(false);
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

    public override void ProcessAbility(AbilityId ability)
    {
        base.ProcessAbility(ability);
        switch (ability)
        {
            case AbilityId.APC:
                if (isPresenting)
                {
                    isPresenting = false;
                    antigenDetect.gameObject.SetActive(false);
                }
                else
                {
                    isPresenting = true;
                    antigenDetect.gameObject.SetActive(true);
                }
                break;
            default:
                break;
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
        if (isPresenting)
        {
            if (carriedAntigen == null)
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

                    if (carriedAntigen != null)
                    {
                        this.carriedAntigen = carriedAntigen;
                    }
                    ReturnToBase();
                }
            }
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
