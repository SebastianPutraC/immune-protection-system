using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Spawner : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform spawnPoint;
    public int initialRouteId;

    public Action<Spawner> spawnClicked;

    public Action<AllyUnitController> AllySpawned;
    public Action<EnemyUnitController> enemySpawned;

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
        enemySpawned?.Invoke(newEnemy);
    }
}
