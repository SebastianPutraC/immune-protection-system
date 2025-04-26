using System;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public float unitHp; // For Now

    public Action<UnitController> ThisUnitKilled;
    public Action<int, DeployedUnitController> UnitArrivedAtWaypoint;

    public virtual void DealDamage(float damage)
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
