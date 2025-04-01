using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelDataSingleton : MonoBehaviour
{
    public static LevelDataSingleton Instance { get; private set; }

    [SerializeField] private LevelInfo currentLevelInfo;
    public bool isWon = false; // Expand on this later, Probably making its own class

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetCurrentLevelInfo(LevelInfo levelInfo)
    {
        this.currentLevelInfo = levelInfo;
    }
    public LevelInfo GetCurrentLevelInfo()
    {
        if (currentLevelInfo != null)
        {
            return currentLevelInfo;
        }

        return null; // Set to default level
    }
    public void RemoveCurrentLevel()
    {
        currentLevelInfo = null;
    }

    public int GetUnlockedLevel()
    {
        if (currentLevelInfo != null)
        {
            return currentLevelInfo.levelIdUnlock;
        }

        return -1;
    }
    public List<UnitId> GetAllUnits()
    {
        List<UnitId> allUnits = currentLevelInfo.allyUnits.Concat(currentLevelInfo.enemyUnits).ToList();
        return allUnits;
    }
}
