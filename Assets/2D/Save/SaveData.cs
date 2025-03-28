using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<bool> enyclopediaUnlock = new List<bool>();
    public List<bool> levelUnlock = new List<bool>();

    private readonly int LevelsToUnlock = 5;

    public SaveData()
    {
        enyclopediaUnlock = new List<bool>();
        levelUnlock = new List<bool>();

        levelUnlock.Add(true); // First Level
        for (int i = 1; i < LevelsToUnlock; i++)
        {
            levelUnlock.Add(false);
        }
    }
}
