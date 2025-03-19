using System;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public int unitHp; // For Now

    public Action<UnitController> ThisUnitKilled;

    public virtual void DealDamage(int damage)
    {
        unitHp -= damage;

        if (unitHp <= 0)
        {
            Death();
        }
    }

    protected virtual void Death()
    {
        ThisUnitKilled?.Invoke(this);
        Destroy(gameObject);
    }
}
