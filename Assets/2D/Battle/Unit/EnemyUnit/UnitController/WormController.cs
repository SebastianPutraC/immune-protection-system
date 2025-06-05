using UnityEngine;

public class WormController : EnemyUnitController
{
    protected override void Update()
    {
        base.Update();
        if (isDetecting)
        {
            if (meleeCooldown > 0)
            {
                meleeCooldown -= Time.deltaTime;
            }
            else
            {
                meleeCooldown = meleeCooldownSpeed.ReturnFinalValue();
                PhagocytosisAttack();
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
}
