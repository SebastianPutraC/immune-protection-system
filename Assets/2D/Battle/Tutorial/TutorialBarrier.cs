using System;
using UnityEngine;

public class TutorialBarrier : MonoBehaviour
{
    public Action<DeployedUnitController> UnitHitBarrier;
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AllyUnit"))
        {
            DeployedUnitController unitController = collision.GetComponent<DeployedUnitController>();
            UnitHitBarrier?.Invoke(unitController);
        }
    }
}
