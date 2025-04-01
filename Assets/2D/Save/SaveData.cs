using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<EncyclopediaData> enyclopediaUnlock = new List<EncyclopediaData>();
    public List<bool> levelUnlock = new List<bool>();

    public SaveData(int LevelsToUnlock, List<int> encylopediaIds)
    {
        enyclopediaUnlock = new List<EncyclopediaData>();
        levelUnlock = new List<bool>();

        levelUnlock.Add(true); // First Level
        for (int i = 1; i < LevelsToUnlock; i++)
        {
            levelUnlock.Add(false);
        }

        for (int i = 0; i < encylopediaIds.Count; i++)
        {
            EncyclopediaData newData = new EncyclopediaData(encylopediaIds[i], false);
            enyclopediaUnlock.Add(newData);
        }
    }
}
