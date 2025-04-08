using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManagerSingleton : MonoBehaviour
{
    public static SaveManagerSingleton Instance { get; private set; }

    [SerializeField] private string fileName;

    [SerializeField] private string selectedProfileId = "";
    private SaveData selectedSaveData;

    private Dictionary<string, SaveData> allSaveData = new Dictionary<string, SaveData>();

    private SaveFileHandler saveFileHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        saveFileHandler = new SaveFileHandler(Application.persistentDataPath, fileName);
    }

    public void NewProfileData(string profileId)
    {
        selectedSaveData = new SaveData(GetAllLevelIds(), GetAllEncylopediaIds());
        saveFileHandler.SaveData(selectedSaveData, profileId);
        PickProfileData(profileId);
    }
    public void PickProfileData(string profileId)
    {
        LoadProfileData();
        selectedProfileId = profileId;
        selectedSaveData = allSaveData[profileId];
    }
    public void DeleteProfileData(string profileId)
    {
        saveFileHandler.DeleteData(profileId);
        LoadProfileData();
    }

    public void LoadProfileData()
    {
        allSaveData = saveFileHandler.LoadAllProfileData();
    }
    public string GetProfileId()
    {
        return selectedProfileId;
    }
    public SaveData GetSelectedSaveData()
    {
        return selectedSaveData;
    }
    public void UpdateSelectedData(UnitId unit = UnitId.NONE, LevelId level = LevelId.NONE)
    {
        if (unit != UnitId.NONE)
        {
            int unitId = (int)unit;
            selectedSaveData.enyclopediaUnlock.Where(x => x.UnitId.Equals(unitId)).FirstOrDefault().Unlocked = true;
        }

        if (level != LevelId.NONE)
        {
            int levelId = (int)level;
            selectedSaveData.levelUnlock.Where(x => x.LevelId.Equals(levelId)).FirstOrDefault().Unlocked = true;
        }
    }

    public Dictionary<string, SaveData> GetAllSaveData()
    {
        return allSaveData;
    }
    private List<int> GetAllEncylopediaIds()
    {
        List<int> ids = ((UnitId[])UnitId.GetValues(typeof(UnitId))).Select(c => (int)c).ToList();
        ids.Remove(0);
        return ids;
    }
    private List<int> GetAllLevelIds()
    {
        List<int> ids = ((LevelId[])LevelId.GetValues(typeof(LevelId))).Select(c => (int)c).ToList();
        ids.Remove(0);
        return ids;
    }

    public void SaveProfileData()
    {
        if (selectedSaveData == null || selectedProfileId == "")
        {
            Debug.LogWarning("No Save Data found");
            return;
        }

        saveFileHandler.SaveData(selectedSaveData, selectedProfileId);
    }

    private void OnApplicationQuit()
    {
        SaveProfileData();
    }
}
