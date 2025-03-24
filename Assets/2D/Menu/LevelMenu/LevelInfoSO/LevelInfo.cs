using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfo", menuName = "Scriptable Objects/LevelInfo")]
public class LevelInfo : ScriptableObject
{
    public int levelId;
    public string levelName;
    public List<UnitId> allyUnits;
    public List<UnitId> enemyUnits;
    public int defaultStatingResource;
    public int defaultIncome;
    public int levelIdUnlock;
}
