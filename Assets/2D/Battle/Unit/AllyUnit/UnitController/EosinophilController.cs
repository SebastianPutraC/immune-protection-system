using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EosinophilController : AllyUnitController
{
    [SerializeField] protected ParticleSystem AoeEffect;

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
        else
        {
            AoeEffect.Stop();
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
        AoeEffect.Play();
        if (targetEnemyUnitMelee != null)
        {
            foreach(DeployedUnitController enemyUnit in detectedUnitMelee.Values.ToList())
            {
                enemyUnit.DealDamage(unitAtk.ReturnFinalValue());
            }
        }
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
        netDetection.enemyDetected -= NetDetect;
    }
}
