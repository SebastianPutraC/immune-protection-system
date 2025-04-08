using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<EncyclopediaData> enyclopediaUnlock;
    public List<LevelData> levelUnlock;

    public SaveData(List<int> levelIds, List<int> encylopediaIds)
    {
        enyclopediaUnlock = new List<EncyclopediaData>();
        levelUnlock = new List<LevelData>();

        for (int i = 0; i < levelIds.Count; i++)
        {
            LevelData newLevelData = new LevelData(levelIds[i], false);
            if (i == 0)
            {
                newLevelData.Unlocked = true;
            }
            levelUnlock.Add(newLevelData);
        }

        for (int i = 0; i < encylopediaIds.Count; i++)
        {
            EncyclopediaData newEncylopediaData = new EncyclopediaData(encylopediaIds[i], false);
            enyclopediaUnlock.Add(newEncylopediaData);
        }
    }
}
