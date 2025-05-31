using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyCellController : UnitController, ISpawner
{
    public bool isInfected = false;
    private float spawnCooldown = 10f;
    private EnemyUnit pathogenToSpawn;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int initialRouteId;

    [SerializeField] private SpriteRenderer spriteRenderer;

    public Action<EnemyUnit, BodyCellController> CellSpawning;
    public Action<EnemyUnitController> EnemySpawned { get; set; }
    public Action<AllyUnitController> AllySpawned { get; set; }

    private void Update()
    {
        if (isInfected)
        {
            if (spawnCooldown <= 0)
            {
                CellSpawning?.Invoke(pathogenToSpawn, this);
                spawnCooldown = 10f;
            }
            spawnCooldown -= Time.deltaTime;
        }
    }

    public void InfectCell(EnemyUnit enemyUnitInfo)
    {
        isInfected = true;
        spriteRenderer.color = Color.green;
        pathogenToSpawn = enemyUnitInfo;
    }
    public void Apoptosis()
    {
        Destroy(this.gameObject);
    }

    public void SpawnEnemy(EnemyUnit enemyUnit, List<StatModifier> modifiers, Route initialRoute, bool playSound = true)
    {
        EnemyUnitController newEnemy = (EnemyUnitController)Instantiate(enemyUnit.unitPrefab, spawnPoint);
        newEnemy.InitializeUnit(enemyUnit, modifiers, initialRoute);
        EnemySpawned?.Invoke(newEnemy);
    }
    public void SpawnAlly(AllyUnit allyUnit, List<StatModifier> modifiers, Route initialRoute, bool playSound = true)
    {
        AllyUnitController newAlly = (AllyUnitController)Instantiate(allyUnit.unitPrefab, spawnPoint);
        newAlly.InitializeUnit(allyUnit, modifiers, initialRoute);
        AllySpawned?.Invoke(newAlly);
    }
    public int GetInitialRouteId()
    {
        return initialRouteId;
    }
}
