using System;
using UnityEngine;
using UnityEngine.UI;

public class ProfileButton : MonoBehaviour
{
    public string profileId;
    private SaveData saveData;

    [SerializeField] private Button profileButton;
    [SerializeField] private Button newProfileButton;
    [SerializeField] private Button deleteButton;

    public Action<string> ProfileSelected;
    public Action<string> DeleteSelected;
    public Action<string> NewProfileSelected;

    public bool IsDataExist;

    private void Awake()
    {
        profileButton.onClick.AddListener(OnProfileSelected);
        deleteButton.onClick.AddListener(OnDeleteSelected);
        newProfileButton.onClick.AddListener(OnNewProfileSelected);
    }

    public void InitializeButton(SaveData saveData, bool isDataExist)
    {
        if (isDataExist)
        {
            this.saveData = saveData;
            this.IsDataExist = isDataExist;
        }

        ShowProfile(isDataExist);
    }
    private void ShowProfile(bool isDataExist)
    {
        if (isDataExist)
        {
            profileButton.gameObject.SetActive(true);
            deleteButton.gameObject.SetActive(true);
            newProfileButton.gameObject.SetActive(false);
        }
        else
        {
            profileButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
            newProfileButton.gameObject.SetActive(true);
        }
    }

    private void OnProfileSelected()
    {
        ProfileSelected?.Invoke(profileId);
    }
    private void OnDeleteSelected()
    {
        DeleteSelected?.Invoke(profileId);
    }
    private void OnNewProfileSelected()
    {
        NewProfileSelected?.Invoke(profileId);
    }
}
