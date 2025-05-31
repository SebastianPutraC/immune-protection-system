using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawner
{
    Action<EnemyUnitController> EnemySpawned { get; set; }
    Action<AllyUnitController> AllySpawned { get; set; }

    void SpawnEnemy(EnemyUnit enemyUnit, List<StatModifier> modifiers, Route initialRoute, bool playSound = true);
    void SpawnAlly(AllyUnit allyUnit, List<StatModifier> modifiers, Route initialRoute, bool playSound = true);
    int GetInitialRouteId();
}

