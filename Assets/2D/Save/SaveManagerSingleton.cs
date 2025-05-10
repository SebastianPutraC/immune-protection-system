using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManagerSingleton : MonoBehaviour
{
    public static SaveManagerSingleton Instance { get; private set; }

    [SerializeField] private string fileName;

    [SerializeField] private string profileId = "Profile";
    private SaveData selectedSaveData;

    private Dictionary<string, SaveData> allSaveData = new Dictionary<string, SaveData>();

    private SaveFileHandler saveFileHandler;

    private string defaultProfileName = "TidakAdaNama";
    private int numberOfQuiz = 6;

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

        LoadProfileData();
        if (!allSaveData.ContainsKey(profileId))
        {
            NewProfileData(profileId);
        }
        PickProfileData(profileId);
    }

    public void NewProfileData(string profileId)
    {
        selectedSaveData = new SaveData(defaultProfileName, GetAllLevelIds(), GetAllEncylopediaIds(), GetAllScoreIds());
        saveFileHandler.SaveData(selectedSaveData, profileId);
        PickProfileData(profileId);
    }
    public void PickProfileData(string profileId)
    {
        LoadProfileData();
        this.profileId = profileId;
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
        return profileId;
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
    private List<int> GetAllScoreIds()
    {
        List<int> ids = ((LevelId[])LevelId.GetValues(typeof(LevelId))).Select(c => (int)c).ToList();
        ids.RemoveRange(0, 2);
        return ids;
    }

    public void SaveProfileData()
    {
        if (selectedSaveData == null || profileId == "")
        {
            Debug.LogWarning("No Save Data found");
            NewProfileData(profileId);
            return;
        }

        saveFileHandler.SaveData(selectedSaveData, profileId);
    }

    private void OnApplicationQuit()
    {
        SaveProfileData();
    }
}
