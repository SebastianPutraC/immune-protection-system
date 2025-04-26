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
}
