using UnityEngine;

public class PlasmaCellController : AllyUnitController
{
    [SerializeField] protected Transform projectileSpawn;
    [SerializeField] protected AudioSource firingAudioSource;

    protected override void Update()
    {
        base.Update();
        if (isRangedDetecting)
        {
            if (rangedCooldown > 0)
            {
                rangedCooldown -= Time.deltaTime;
            }
            else
            {
                AttackArtillery();
                rangedCooldown = rangedCooldownSpeed.ReturnFinalValue();
            }
        }
    }
    protected void AttackArtillery()
    {
        if (targetEnemyUnitRanged != null)
        {
            firingAudioSource.time = 0.15f;
            firingAudioSource.Play();
            Projectile spawnProjectile = Instantiate(UnitSO.RangedProjectile, projectileSpawn);
            spawnProjectile.InitializeArtilleryProjectile(10, UnitSO.UnitRangedAtk,
                targetEnemyUnitRanged, 7); // 10 is speed, 7 is radius
        }
    }
}
