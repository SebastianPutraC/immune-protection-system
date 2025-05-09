using System;
using System.Collections.Generic;
using UnityEngine;

public class EosinophilController : AllyUnitController, IPresenter
{
    [SerializeField] protected ParticleSystem AoeEffect;

    [SerializeField] private DetectionTrigger antigenDetect;
    [SerializeField] private Transform antigenCarryPosition;
    private AntigenController carriedAntigen;

    [SerializeField] private DetectionTrigger netDetection;
    [SerializeField] private ParticleSystem netEffect;
    private bool trapActivated = false;
    private float trapCooldown = 0.5f;
    [SerializeField] protected Dictionary<int, UnitController> detectedUnitOnTrap = new Dictionary<int, UnitController>();

    public Action<int> AntigenPresented { get; set; }
    public bool isPresenting { get; set; }

    protected override void Awake()
    {
        base.Awake();
        antigenDetect.enemyDetected += GetAntigen;
        netDetection.enemyDetected += NetDetect;
    }

    protected override void Update()
    {
        base.Update();
        if (trapActivated)
        {
            trapCooldown -= Time.deltaTime;
            if (trapCooldown <= 0)
            {
                NetAttack();
            }
        }

        if (isDetecting)
        {
            if (meleeCooldown > 0)
            {
                meleeCooldown -= Time.deltaTime;
            }
            else
            {
                ToxinAttack();
                meleeCooldown = UnitSO.UnitAtkSpeed;
            }
        }
    }
    public override void ProcessAbility(AbilityId ability)
    {
        base.ProcessAbility(ability);
        switch (ability)
        {
            case AbilityId.TRAP:
                if (!trapActivated)
                {
                    ActivateNet();
                }
                break;
            default:
                break;
        }
    }

    protected void ToxinAttack()
    {
        if (targetEnemyUnitMelee != null)
        {
            for (int i = 0; i < detectedUnitMelee.Count; i++)
            {
                detectedUnitMelee[i].DealDamage(unitAtk.ReturnFinalValue());
            }
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

    protected void ActivateNet()
    {
        trapActivated = true;
        netEffect.Play();
        netDetection.gameObject.SetActive(true);
    }
    protected void NetDetect(UnitController enemyUnit)
    {
        if (!detectedUnitOnTrap.ContainsKey(enemyUnit.GetInstanceID()))
        {
            detectedUnitOnTrap.Add(enemyUnit.GetInstanceID(), enemyUnit);
        }
    }
    protected void NetAttack()
    {
        foreach (UnitController unit in detectedUnitOnTrap.Values)
        {
            if (unit != null)
            {
                unit.DealDamage(100);
            }
        }

        DealDamage(9999);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        antigenDetect.enemyDetected -= GetAntigen;
        netDetection.enemyDetected -= NetDetect;
    }
}
