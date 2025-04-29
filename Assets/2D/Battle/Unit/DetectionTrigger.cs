using System;
using UnityEngine;

public class DetectionTrigger : MonoBehaviour //Might make class to detect specific item
{
    [SerializeField] private UnitSide unitSide;
    [SerializeField] private Collider2D trigger;
    [SerializeField] private bool isPresenter; //Placeholder
    [SerializeField] private bool isCheckCell; //Placeholder

    public Action<UnitController> enemyDetected;
    public Action<UnitController> enemyOutRange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPresenter && !isCheckCell)
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
        else if (isPresenter)
        {
            if (unitSide == UnitSide.ALLY)
            {
                if (collision.CompareTag("Antigen"))
                {
                    enemyDetected?.Invoke(collision.GetComponent<UnitController>());
                }
            }
        }
        else if (isCheckCell)
        {
            if (collision.CompareTag("BodyCell"))
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
