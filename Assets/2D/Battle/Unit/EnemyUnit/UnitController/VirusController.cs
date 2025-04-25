using UnityEngine;

public class VirusController : EnemyUnitController
{
    [SerializeField] protected Transform projectileSpawn;

    protected override void Awake()
    {
        base.Awake();
    }
    protected void Update()
    {
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
