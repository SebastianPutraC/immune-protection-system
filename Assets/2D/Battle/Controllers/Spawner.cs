using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Spawner : MonoBehaviour, IPointerClickHandler, ISpawner
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int initialRouteId;

    public Action<Spawner> spawnClicked;

    public Action<AllyUnitController> AllySpawned { get; set; }
    public Action<EnemyUnitController> EnemySpawned { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        spawnClicked?.Invoke(this);
    }

    public void SpawnAlly(AllyUnit allyUnit, List<StatModifier> modifiers, Route initialRoute)
    {
        AllyUnitController newAlly = (AllyUnitController)Instantiate(allyUnit.unitPrefab, spawnPoint);
        newAlly.InitializeUnit(allyUnit, modifiers, initialRoute);
        AllySpawned?.Invoke(newAlly);
    }

    public void SpawnEnemy(EnemyUnit enemyUnit, List<StatModifier> modifiers, Route initialRoute)
    {
        EnemyUnitController newEnemy = (EnemyUnitController)Instantiate(enemyUnit.unitPrefab, spawnPoint);
        newEnemy.InitializeUnit(enemyUnit, modifiers, initialRoute);
        EnemySpawned?.Invoke(newEnemy);
    }

    public int GetInitialRouteId()
    {
        return initialRouteId;
    }
}
