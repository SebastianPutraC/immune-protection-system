using System;
using UnityEngine;

public class EnemyBaseController : UnitController
{
    public Action<int> enemyBaseAttacked;
    public Action enemyBaseDestroyed;

    public void InitializeBase(int baseHp)
    {
        unitHp = baseHp;
    }

    public override void DealDamage(int damage)
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
