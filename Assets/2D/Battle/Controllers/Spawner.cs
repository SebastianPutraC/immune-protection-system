using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Spawner : MonoBehaviour, IPointerClickHandler, ISpawner
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int initialRouteId;

    public Action<Spawner> spawnClicked;
    public UnitSide baseSide;

    public Action<AllyUnitController> AllySpawned { get; set; }
    public Action<EnemyUnitController> EnemySpawned { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        spawnClicked?.Invoke(this);
    }

    public void SpawnAlly(AllyUnit allyUnit, List<StatModifier> modifiers, Route initialRoute, bool playSound = true)
    {
        AllyUnitController newAlly = (AllyUnitController)Instantiate(allyUnit.unitPrefab, spawnPoint);
        newAlly.InitializeUnit(allyUnit, modifiers, initialRoute, playSound);
        AllySpawned?.Invoke(newAlly);
    }

    public void SpawnEnemy(EnemyUnit enemyUnit, List<StatModifier> modifiers, Route initialRoute, bool playSound = true)
    {
        EnemyUnitController newEnemy = (EnemyUnitController)Instantiate(enemyUnit.unitPrefab, spawnPoint);
        newEnemy.InitializeUnit(enemyUnit, modifiers, initialRoute, playSound);
        EnemySpawned?.Invoke(newEnemy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (baseSide == UnitSide.ALLY)
        {
            if (collision.CompareTag("EnemyUnit"))
            {
                collision.GetComponent<EnemyUnitController>().DealDamage(9999);
            }
        }
        else
        {
            if (collision.CompareTag("AllyUnit"))
            {
                collision.GetComponent<AllyUnitController>().DealDamage(9999);
            }
        }
    }

    public int GetInitialRouteId()
    {
        return initialRouteId;
    }
}
