using UnityEngine;

public class BacteriaController : EnemyUnitController
{
    [SerializeField] protected DetectionTrigger cellDetection;

    protected override void Awake()
    {
        base.Awake();
        cellDetection.enemyDetected += CellDetect;
    }

    protected override void Update()
    {
        base.Update();
        if (isDetecting)
        {
            if (meleeCooldown > 0)
            {
                meleeCooldown -= Time.deltaTime;
            }
            else
            {
                PhagocytosisAttack();
                meleeCooldown = UnitSO.UnitAtkSpeed;
            }
        }
    }
    private void PhagocytosisAttack()
    {
        if (targetEnemyUnitMelee != null)
        {
            targetEnemyUnitMelee.DealDamage(unitAtk.ReturnFinalValue());
        }
    }
    private void CellDetect(UnitController infectedCell)
    {
        if (infectedCell is not BodyCellController)
        {
            return;
        }
        else
        {
            BodyCellController detectedCell = (BodyCellController)infectedCell;
            if (detectedCell.isInfected == false)
            {
                detectedCell.InfectCell((EnemyUnit)UnitSO);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        cellDetection.enemyDetected -= CellDetect;
    }
}
