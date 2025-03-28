using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProfileMenuController : MonoBehaviour
{
    [SerializeField] private List<ProfileButton> profileButtons;
    [SerializeField] private Button returnButton;

    public Action ReturnButtonPressed;
    public Action ProfileSelected;

    private void Awake()
    {
        returnButton.onClick.AddListener(OnReturnButtonPressed);

        for (int i = 0; i < profileButtons.Count; i++)
        {
            profileButtons[i].ProfileSelected += OnProfilePick;
            profileButtons[i].DeleteSelected += OnProfileDelete;
            profileButtons[i].NewProfileSelected += OnNewProfile;
        }
    }

    public void InitializeProfile()
    {
        SaveManagerSingleton.Instance.LoadProfileData();
        Dictionary<string, SaveData> loadedProfiles = SaveManagerSingleton.Instance.GetAllSaveData();

        bool allEmpty = true;
        for (int i = 0; i < profileButtons.Count; i++)
        {
            SaveData loadData = loadedProfiles.Where(data => data.Key == profileButtons[i].profileId).FirstOrDefault().Value;
            if (loadData != null)
            {
                allEmpty = false;
                profileButtons[i].InitializeButton(loadData, true);
            }
            else
            {
                profileButtons[i].InitializeButton(null, false);
            }
        }
        
        if (allEmpty || SaveManagerSingleton.Instance.GetSelectedSaveData() == null)
        {
            returnButton.gameObject.SetActive(false);
        }
        else
        {
            returnButton.gameObject.SetActive(true);
        }
    }

    private void OnNewProfile(string profileId)
    {
        SaveManagerSingleton.Instance.NewProfileData(profileId);
        ProfileSelected?.Invoke();
    }
    private void OnProfilePick(string profileId)
    {
        SaveManagerSingleton.Instance.PickProfileData(profileId);
        ProfileSelected?.Invoke();
    }
    private void OnProfileDelete(string profileId)
    {
        SaveManagerSingleton.Instance.DeleteProfileData(profileId);
        InitializeProfile();
    }


    private void OnReturnButtonPressed()
    {
        ReturnButtonPressed?.Invoke();
    }

    private void OnDestroy()
    {
        returnButton.onClick.RemoveListener(OnReturnButtonPressed);
        for (int i = 0; i < profileButtons.Count; i++)
        {
            profileButtons[i].ProfileSelected -= OnProfilePick;
            profileButtons[i].DeleteSelected -= OnProfileDelete;
            profileButtons[i].NewProfileSelected -= OnNewProfile;
        }
    }
}
