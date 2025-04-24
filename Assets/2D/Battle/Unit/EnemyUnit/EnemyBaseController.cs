using System;
using UnityEngine;

public class EnemyBaseController : UnitController
{
    public Action<float> enemyBaseAttacked;
    public Action enemyBaseDestroyed;

    public void InitializeBase(float baseHp)
    {
        unitHp = baseHp;
    }

    public override void DealDamage(float damage)
    {
        unitHp -= damage;
        enemyBaseAttacked?.Invoke(unitHp);

        if (unitHp <= 0)
        {
            unitHp = 0;
            enemyBaseAttacked?.Invoke(unitHp);
            Death();
        }
    }

    protected override void Death()
    {
        enemyBaseDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
