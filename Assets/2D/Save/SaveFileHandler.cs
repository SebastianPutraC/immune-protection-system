using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class SaveFileHandler
{
    private string saveFilePath = "";
    private string saveFileName = "";
    private string backupExtension = ".bak";

    public SaveFileHandler(string filePath, string fileName)
    {
        this.saveFilePath = filePath;
        this.saveFileName = fileName;
    }

    public SaveData LoadData(string profileId)
    {
        if (profileId == null)
        {
            return null;
        }

        string pathCombined = Path.Combine(saveFilePath, profileId, saveFileName);
        SaveData dataLoaded = null;
        if (File.Exists(pathCombined))
        {
            try
            {
                string loadData = "";
                using (FileStream stream = new FileStream(pathCombined, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadData = reader.ReadToEnd();
                    }
                }

                dataLoaded = JsonUtility.FromJson<SaveData>(loadData);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Rollback using backup data file : " + e);
                bool isRollbackSuccess = RollbackBackupData(pathCombined);
                if (isRollbackSuccess)
                {
                    dataLoaded = LoadData(profileId);
                }
                else
                {
                    Debug.LogWarning("Rollback Failure");
                }
            }
        }
        return dataLoaded;
    }

    public void SaveData(SaveData saveData, string profileId)
    {
        if (profileId == null)
        {
            return;
        }

        string pathCombined = Path.Combine(saveFilePath, profileId, saveFileName);
        string backupFilePath = pathCombined + backupExtension;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(pathCombined));

            string saveDataJson = JsonUtility.ToJson(saveData, true);

            using (FileStream stream = new FileStream(pathCombined, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(saveDataJson);
                }
            }

            SaveData dataToBackup = LoadData(profileId);
            if (dataToBackup != null)
            {
                File.Copy(pathCombined, backupFilePath, true);
            }
            else
            {
                Debug.LogWarning("Load Failed at : " + pathCombined);
            }

        }
        catch (Exception e)
        {
            Debug.LogWarning("Error to save path : " + pathCombined + " : " + e);
        }
    }

    public void DeleteData(string profileId)
    {
        if (profileId == null)
        {
            return;
        }

        string pathCombined = Path.Combine(saveFilePath, profileId, saveFileName);
        try
        {
            if (File.Exists(pathCombined))
            {
                Directory.Delete(Path.GetDirectoryName(pathCombined), true);
            }
            else
            {
                Debug.LogWarning("No data found to be deleted: " + pathCombined);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error to delete data ID : " + profileId +  " : " + e);
        }
    }

    public Dictionary<string, SaveData> LoadAllProfileData()
    {
        Dictionary<string, SaveData> allProfileData = new Dictionary<string, SaveData>();

        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(saveFilePath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            string pathCombined = Path.Combine(saveFilePath, profileId, saveFileName);
            if (!File.Exists(pathCombined))
            {
                Debug.LogWarning("Skip no data at path : " + pathCombined);
                continue;
            }

            SaveData profileData = LoadData(profileId);
            if (profileData != null)
            {
                allProfileData.Add(profileId, profileData);
            }
            else
            {
                Debug.LogWarning("Error to load profile id : " + profileId);
            }
        }

        return allProfileData;
    }

    private bool RollbackBackupData(string pathCombined)
    {
        bool isRollbackSuccess = false;
        string backupFilePath = pathCombined + backupExtension;
        try
        {
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, pathCombined, true);
                isRollbackSuccess = true;
            }
            else
            {
                Debug.LogWarning("No backup data to rollback");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error at rollback path : " + backupFilePath + " : " + e);
        }

        return isRollbackSuccess;
    }
}
