using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyUnitController : DeployedUnitController
{
    public override void InitializeUnit(UnitData unitSO)
    {
        base.InitializeUnit(unitSO);
        this.transform.localEulerAngles = new Vector3(0, 180, 0); // rotate for enemy
    }

    protected override void Move()
    {
        unitRb.linearVelocityX = -UnitSO.UnitSpeed;
    }
    protected override void AttackAOE() // CHANGE THIS
    {
        LayerMask enemy = LayerMask.GetMask("Ally");
        if (targetEnemyUnitRanged != null)
        {
            List<Collider2D> enemyOnAOE = Physics2D.OverlapCapsuleAll(this.transform.position, new Vector2(
                UnitSO.UnitXRange, UnitSO.UnitYRange), CapsuleDirection2D.Horizontal, 0, enemy).ToList();

            for (int i = 0; i < enemyOnAOE.Count; i++)
            {
                if (enemyOnAOE[i].CompareTag("AllyUnit"))
                {
                    enemyOnAOE[i].gameObject.GetComponent<UnitController>().DealDamage(unitAtk);
                }
            }
        }

        base.AttackAOE();
    }
}
