using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RowController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform allySpawn;
    [SerializeField] private Transform enemySpawn;

    [SerializeField] private EnemyUnitController enemyPrefab;

    public Action<RowController> RowClicked; // Might move to other script
    public Action<AllyUnitController> AllySpawned;

    public void OnPointerClick(PointerEventData eventData)
    {
        RowClicked?.Invoke(this);
    }

    public void SpawnAlly(AllyUnit allyUnit, List<StatModifier> modifiers)
    {
        AllyUnitController newAlly = (AllyUnitController)Instantiate(allyUnit.unitPrefab, allySpawn);
        newAlly.InitializeUnit(allyUnit, modifiers);
        AllySpawned?.Invoke(newAlly);
    }

    public void SpawnEnemy(EnemyUnit enemyUnit, List<StatModifier> modifiers)
    {
        EnemyUnitController newEnemy = (EnemyUnitController)Instantiate(enemyUnit.unitPrefab, enemySpawn);
        newEnemy.InitializeUnit(enemyUnit, modifiers);
    }
}
