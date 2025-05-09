using System;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public UnitSide baseSide;

    public Action<UnitSide> baseAttacked;

    public void InitializeBase(UnitSide baseSide)
    {
        this.baseSide = baseSide;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (baseSide == UnitSide.ALLY)
        {
            if (collision.CompareTag("EnemyUnit"))
            {
                baseAttacked?.Invoke(baseSide);
                collision.GetComponent<EnemyUnitController>().DealDamage(9999);
            }
        }
        else
        {
            if (collision.CompareTag("AllyUnit"))
            {
                baseAttacked?.Invoke(baseSide);
                collision.GetComponent<AllyUnitController>().DealDamage(9999);
            }
        }
    }
}
