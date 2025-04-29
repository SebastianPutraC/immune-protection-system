using UnityEngine;

public class KillerTCellController : AllyUnitController
{
    [SerializeField] protected DetectionTrigger cellDetection;

    protected override void Awake()
    {
        base.Awake();
        cellDetection.enemyDetected += CellDetect;
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
            if (detectedCell.isInfected == true)
            {
                detectedCell.Apoptosis();
            }
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        cellDetection.enemyDetected -= CellDetect;
    }
}
