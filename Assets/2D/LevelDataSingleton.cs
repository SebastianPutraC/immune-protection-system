using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelDataSingleton : MonoBehaviour
{
    public static LevelDataSingleton Instance { get; private set; }

    [SerializeField] private LevelInfo currentLevelInfo;
    public bool isWon = false; // Expand on this later, Probably making its own class

    [SerializeField] private List<BoostData> allyBoostData = new List<BoostData>();
    [SerializeField] private List<BoostData> enemyBoostData = new List<BoostData>();

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

    public void SetModifiersData(List<BoostData> boostDatas, UnitSide side)
    {
        if (side == UnitSide.ALLY)
        {
            allyBoostData = boostDatas;
        }
        else if (side == UnitSide.ENEMY)
        {
            enemyBoostData = boostDatas;
        }
    }
    public List<StatModifier> GetModifiersData(UnitSide side)
    {
        if (side == UnitSide.ALLY)
        {
            return allyBoostData.Select(data => data.StatMod).ToList();
        }
        else if (side == UnitSide.ENEMY)
        {
            return enemyBoostData.Select(data => data.StatMod).ToList();
        }
        else
        {
            return null;
        }
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
