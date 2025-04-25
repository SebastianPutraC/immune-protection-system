using UnityEngine;

public class PlasmaCellController : AllyUnitController
{
    [SerializeField] protected Transform projectileSpawn;

    protected void Update()
    {
        if (isRangedDetecting)
        {
            if (rangedCooldown > 0)
            {
                rangedCooldown -= Time.deltaTime;
            }
            else
            {
                AttackArtillery();
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
    protected void AttackArtillery()
    {
        if (targetEnemyUnitRanged != null)
        {
            Projectile spawnProjectile = Instantiate(UnitSO.RangedProjectile, projectileSpawn);
            spawnProjectile.InitializeArtilleryProjectile(10, UnitSO.UnitRangedAtk,
                targetEnemyUnitRanged, 7); // 10 is speed, 7 is radius
        }
    }
}
