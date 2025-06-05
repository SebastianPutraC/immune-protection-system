using UnityEngine;

public class BacteriaController : EnemyUnitController
{
    protected override void Awake()
    {
        base.Awake();
    }

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

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
