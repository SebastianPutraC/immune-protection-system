using System;
using UnityEngine;

public class DetectionTrigger : MonoBehaviour
{
    [SerializeField] private UnitSide unitSide;
    [SerializeField] private Collider2D trigger;
    //private AttackType attackType;

    public Action<UnitController> enemyDetected;
    public Action<UnitController> enemyOutRange;

    /*public void InitializeRange(float xRange, float yRange, AttackType attackType)
    {
        this.attackType = attackType;
        trigger.size = new Vector2(xRange, yRange);

        if (this.attackType == AttackType.MELEE || this.attackType == AttackType.RANGED)
        {
            trigger.offset = new Vector2(xRange / 2, 0);
        }
        else if (this.attackType == AttackType.ARTILERRY)
        {
            trigger.offset = new Vector2(13, 0);
        }
        else
        {
            trigger.offset = Vector2.zero;
        }
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (unitSide == UnitSide.ALLY)
        {
            if (collision.CompareTag("EnemyUnit") || collision.CompareTag("EnemyBase"))
            {
                enemyDetected?.Invoke(collision.GetComponent<UnitController>());
            }
        }
        else
        {
            if (collision.CompareTag("AllyUnit") || collision.CompareTag("AllyBase"))
            {
                enemyDetected?.Invoke(collision.GetComponent<UnitController>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (unitSide == UnitSide.ALLY)
        {
            if (collision.CompareTag("EnemyUnit"))
            {
                enemyOutRange?.Invoke(collision.GetComponent<UnitController>());
            }
        }
        else
        {
            if (collision.CompareTag("AllyUnit"))
            {
                enemyOutRange?.Invoke(collision.GetComponent<UnitController>());
            }
        }
    }
}
