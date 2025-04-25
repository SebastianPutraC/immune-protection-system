using System.Collections.Generic;
using UnityEngine;

public class EosinophilController : AllyUnitController
{
    [SerializeField] protected ParticleSystem AoeEffect;

    [SerializeField] private DetectionTrigger antigenDetect;
    [SerializeField] private Transform antigenCarryPosition;

    [SerializeField] private DetectionTrigger netDetection;
    [SerializeField] private ParticleSystem netEffect;
    private bool trapActivated = false;
    private float trapCooldown = 0.5f;
    [SerializeField] protected Dictionary<int, UnitController> detectedUnitOnTrap = new Dictionary<int, UnitController>();

    protected override void Awake()
    {
        base.Awake();
        antigenDetect.enemyDetected += GetAntigen;
        netDetection.enemyDetected += NetDetect;
    }

    protected void Update()
    {
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
            if (isMoveOrder)
            {
                Move();
            }
            else
            {
                Stop();
            }
        }
    }
    public override void ProcessAbility(AbilityType ability)
    {
        base.ProcessAbility(ability);
        switch (ability)
        {
            case AbilityType.NET:
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

    private void GetAntigen(UnitController antigen)
    {

    }
    private void ReturnToBase()
    {

    }
    private void ReturnToNormal()
    {

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
