using System.Collections.Generic;
using UnityEngine;

public class NeutrophilController : AllyUnitController
{
    [SerializeField] private DetectionTrigger netDetection;
    [SerializeField] private ParticleSystem netEffect;

    private bool trapActivated = false;
    private float trapCooldown = 0.5f;
    [SerializeField] protected Dictionary<int, UnitController> detectedUnitOnTrap = new Dictionary<int, UnitController>();

    [SerializeField] protected Transform projectileSpawn;

    public GameObject TutorialArrow;

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
                PhagocytosisAttack();
                meleeCooldown = UnitSO.UnitAtkSpeed;
            }
        }
        else if (isRangedDetecting)
        {
            if (rangedCooldown > 0)
            {
                rangedCooldown -= Time.deltaTime;
            }
            else
            {
                DegranulationAttack();
                rangedCooldown = UnitSO.UnitRangedAtkSpeed;
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
    private void PhagocytosisAttack()
    {
        if (targetEnemyUnitMelee != null)
        {
            targetEnemyUnitMelee.DealDamage(unitAtk.ReturnFinalValue());
        }
    }
    protected void DegranulationAttack()
    {
        if (targetEnemyUnitRanged != null)
        {
            Projectile spawnProjectile = Instantiate(UnitSO.RangedProjectile, projectileSpawn);
            spawnProjectile.InitializeBulletProjectile(10, UnitSO.UnitRangedAtk, targetEnemyUnitRanged); // 10 is placeholder
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
