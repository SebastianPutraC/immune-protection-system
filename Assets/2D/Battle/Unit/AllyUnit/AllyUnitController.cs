using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class AllyUnitController : DeployedUnitController
{
    [SerializeField] protected GameObject selectedIndicator;

    public void UnitSelect()
    {
        selectedIndicator.SetActive(true);
    }
    public void UnitDeselect()
    {
        selectedIndicator.SetActive(false);
    }

    /*protected override void AttackAOE() // CHANGE THIS
    {
        LayerMask enemy = LayerMask.GetMask("Enemy");
        if (targetEnemyUnitMelee != null)
        {
            List<Collider2D> enemyOnAOE = Physics2D.OverlapCapsuleAll(this.transform.position, new Vector2(
                UnitSO.UnitXRange + 5, UnitSO.UnitYRange + 5), CapsuleDirection2D.Horizontal, 1, enemy).ToList();

            for (int i = 0; i < enemyOnAOE.Count; i++)
            {
                if (enemyOnAOE[i].CompareTag("EnemyUnit") || enemyOnAOE[i].CompareTag("EnemyBase"))
                {
                    enemyOnAOE[i].gameObject.GetComponent<UnitController>().DealDamage(unitAtk.ReturnFinalValue());
                }
            }
        }

        base.AttackAOE();
    }*/
}
