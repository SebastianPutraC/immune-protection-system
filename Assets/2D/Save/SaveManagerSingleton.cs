using System.Collections.Generic;
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
        selectedSaveData = new SaveData();
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
    public Dictionary<string, SaveData> GetAllSaveData()
    {
        return allSaveData;
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
