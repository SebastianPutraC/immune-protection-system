using System.Collections.Generic;
using UnityEngine;

public class EnemyAiManager : MonoBehaviour
{
    private int currentResourcePoints;
    private int maxResourcePoints;
    private float resourcePointIncome;

    public List<LaneData> lanes;
    public List<UnitData> units;

    private float spawnCooldown = 0.5f;

    public void SpawnUnit(UnitData unit, int lane)
    {

    }
    public void SpawnRandom()
    {

    }
    public void SpawnEarly()
    {

    }
    public void SpawnBoom()
    {

    }

    public void UpdateEnemyScore(int score, int lane)
    {

    }
    public void UpdateAllyScore(int score, int lane)
    {

    }
}
