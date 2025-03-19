using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RowController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform allySpawn;
    [SerializeField] private Transform enemySpawn;

    [SerializeField] private AllyUnitController allyPrefab;
    [SerializeField] private EnemyUnitController enemyPrefab;

    public Action<RowController> RowClicked; // Might move to other script
    public Action<AllyUnitController> AllySpawned;

    public void OnPointerClick(PointerEventData eventData)
    {
        RowClicked?.Invoke(this);
    }

    public void SpawnAlly(AllyUnit allyUnit)
    {
        AllyUnitController newAlly = Instantiate(allyPrefab, allySpawn);
        newAlly.InitializeUnit(allyUnit);
        AllySpawned?.Invoke(newAlly);
    }

    public void SpawnEnemy(EnemyUnit enemyUnit)
    {
        EnemyUnitController newEnemy = Instantiate(enemyPrefab, enemySpawn);
        newEnemy.InitializeUnit(enemyUnit);
    }
}
