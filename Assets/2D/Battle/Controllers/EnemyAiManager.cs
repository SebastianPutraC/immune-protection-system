using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAiManager : MonoBehaviour
{
    public bool usingAI = false;

    public int currentResourcePoints;
    public int maxResourcePoints = 800;
    public int resourcePointIncome;

    public int minimalPoints = 200;
    public int defaultMinimalPoints = 200;

    public float startingTimer = 3f;

    public float randomTimer;
    public float maxRandomTimer = 2.5f;
    public float bossTimer;
    public float maxBossTimer = 5f;
    public float boomTimer;
    public float maxBoomTimer = 10f;

    public float defendSpawn;
    public bool isDefending;
    public List<EnemySpawnController> spawners;

    public bool isBoom = true;

    public UnitData bossUnit;
    public UnitData boomUnit;
    public List<UnitData> randomUnits;

    public Action<EnemyUnit, int> EnemySpawn;

    public void InitializeAI(int startingResource, int income, UnitData bossUnit, UnitData boomUnit, List<UnitData> randomUnits, List<EnemySpawnController> enemySpawns, bool usingAI)
    {
        currentResourcePoints = startingResource;
        resourcePointIncome = income;

        this.bossUnit = bossUnit;
        this.boomUnit = boomUnit;
        this.randomUnits = randomUnits;
        this.usingAI = usingAI;

        spawners = enemySpawns;

        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].DefendBase += DefensiveSpawn;
        }
    }

    private void Update()
    {
        if (usingAI)
        {
            if (startingTimer > 0)
            {
                startingTimer -= Time.deltaTime;
                return;
            }
            else
            {
                if (boomTimer > 0)
                {
                    boomTimer -= Time.deltaTime;
                }
                else
                {
                    boomTimer = maxBoomTimer;
                    CheckBoom();
                }
                if (boomTimer > 0)
                {
                    boomTimer -= Time.deltaTime;
                }
                else
                {
                    boomTimer = maxBossTimer;
                    CheckBoss();
                }
                if (randomTimer > 0)
                {
                    randomTimer -= Time.deltaTime;
                }
                else
                {
                    randomTimer = maxRandomTimer;
                    CheckRandom();
                }
            }
        }
    }

    public void IncreaseResourcePoint()
    {
        currentResourcePoints += resourcePointIncome;
        if (currentResourcePoints > maxResourcePoints)
        {
            currentResourcePoints = maxResourcePoints;
        }
    }


    public bool PayUnit(int cost)
    {
        if (cost > currentResourcePoints)
        {
            return false;
        }

        currentResourcePoints -= cost;
        return true;
    }
    public void SpawnUnit(EnemyUnit unit, int lane)
    {
        EnemySpawn?.Invoke(unit, lane);
    }
    public void CheckRandom()
    {
        int availableResource = currentResourcePoints - minimalPoints;
        int mostExpensive = randomUnits.Max(x => x.UnitCost);
        availableResource -= mostExpensive;

        if (availableResource < 0)
        {
            randomTimer = maxRandomTimer;
            return;
        }

        int randomRow = UnityEngine.Random.Range(0, spawners.Count);
        int randomUnit = UnityEngine.Random.Range(0, randomUnits.Count);
        Debug.Log(randomUnit);
        EnemyUnit spawnEnemy = (EnemyUnit)randomUnits[randomUnit];

        bool paymentComplete = PayUnit(spawnEnemy.UnitCost);
        if (paymentComplete)
        {
            randomTimer = maxRandomTimer;
            minimalPoints += 50;

            SpawnUnit(spawnEnemy, randomRow);
        }
    }
    public void CheckBoom()
    {
        if (!isBoom || boomUnit == null)
        {
            return;
        }
        int availableResource = currentResourcePoints - defaultMinimalPoints;
        if (availableResource <= 0)
        {
            boomTimer = randomTimer;
            return;
        }

        EnemyUnit spawnSwarm = (EnemyUnit)boomUnit;
        bool paymentComplete = PayUnit(spawnSwarm.boomCost);
        if (paymentComplete)
        {
            boomTimer = maxBoomTimer;
            minimalPoints = defaultMinimalPoints;
            for (int i = 0; i < spawners.Count; i++)
            {
                SpawnUnit(spawnSwarm, i);
            }

            if (bossUnit != null)
            {
                isBoom = false;
            }
        }
    }
    public void CheckBoss()
    {
        if (isBoom || bossUnit == null)
        {
            return;
        }
        int availableResource = currentResourcePoints - defaultMinimalPoints;
        if (availableResource <= 0)
        {
            bossTimer = randomTimer;
            return;
        }

        int randomRow = UnityEngine.Random.Range(0, spawners.Count);
        EnemyUnit spawnBoss = (EnemyUnit)bossUnit;

        bool paymentComplete = PayUnit(spawnBoss.bossCost);
        if (paymentComplete)
        {
            isBoom = true;
            bossTimer = maxBossTimer;
            minimalPoints = defaultMinimalPoints;
            SpawnUnit(spawnBoss, randomRow);
        }
    }
    public void DefensiveSpawn(int lane, UnitId unitId)
    {
        if (!usingAI)
        {
            return;
        }
        bool oneSpawn = false;

        UnitId firstSpawn = UnitId.BACTERIA;
        UnitId secondSpawn = UnitId.VIRUS;

        switch (unitId)
        {
            case UnitId.EOSINOPHIL:
                firstSpawn = UnitId.VIRUS;
                break;
            case UnitId.NATURALKILLER:
                oneSpawn = true;
                break;
            case UnitId.TKILLER:
                oneSpawn = true;
                break;
            case UnitId.THELPER:
                oneSpawn = true;
                break;
            case UnitId.DENDRITIC:
                oneSpawn = true;
                break;
        }

        UnitData firstUnit = randomUnits.Find(unit => unit.unitId == firstSpawn);
        UnitData secondUnit = randomUnits.Find(unit => unit.unitId == secondSpawn);

        if (firstUnit != null)
        {
            if (currentResourcePoints >= firstUnit.UnitCost)
            {
                bool paymentComplete = PayUnit(firstUnit.UnitCost);
                if (paymentComplete)
                {
                    SpawnUnit((EnemyUnit)firstUnit, lane);
                }
            }
        }
        if (secondUnit != null && !oneSpawn)
        {
            if (currentResourcePoints >= secondUnit.UnitCost)
            {
                bool paymentComplete = PayUnit(secondUnit.UnitCost);
                if (paymentComplete)
                {
                    SpawnUnit((EnemyUnit)secondUnit, lane);
                }
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].DefendBase -= DefensiveSpawn;
        }
    }
}
