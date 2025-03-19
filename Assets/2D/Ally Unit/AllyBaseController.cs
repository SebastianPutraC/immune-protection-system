using System;
using UnityEngine;

public class AllyBaseController : UnitController
{
    public Action<int> allyBaseAttacked;
    public Action allyBaseDestroyed;

    public void InitializeBase(int baseHp)
    {
        unitHp = baseHp;
    }

    public override void DealDamage(int damage)
    {
        unitHp -= damage;
        allyBaseAttacked?.Invoke(unitHp);

        if (unitHp <= 0)
        {
            unitHp = 0;
            allyBaseAttacked?.Invoke(unitHp);
            Death();
        }
    }

    protected override void Death()
    {
        allyBaseDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
