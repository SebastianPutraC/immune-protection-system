using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private ProfileMenuController profileMenu;
    [SerializeField] private LevelMenuController levelMenu;
    [SerializeField] private MainMenuController mainMenu;
    [SerializeField] private OptionMenuController optionMenu;
    [SerializeField] private EncyclopediaMenuController encyclopediaMenu;

    [SerializeField] private List<bool> levelUnlocked = new List<bool>();

    //PLACEHOLDER
    [SerializeField] private List<EncylopediaUnit> placeholderDataTest = new List<EncylopediaUnit>();

    private void Awake()
    {
        profileMenu.ReturnButtonPressed += OpenMainMenu;
        profileMenu.ProfileSelected += OnProfileSelected;
        mainMenu.OnStartButtonPressed += OpenLevelMenu;
        mainMenu.OnOptionButtonPressed += OpenOptionMenu;
        mainMenu.OnEnyclopediaButtonPressed += OpenEncyclopediaMenu;
        mainMenu.OnChangeProfileButtonPressed += OpenProfileMenu;
        optionMenu.OnReturnButtonPressed += OpenMainMenu;
        encyclopediaMenu.OnReturnButtonPressed += OpenMainMenu;
        levelMenu.ReturnButtonPressed += OpenMainMenu;
        levelMenu.LevelSelected += StartLevel;
    }

    private void Start()
    {
        if (SaveManagerSingleton.Instance.GetSelectedSaveData() == null) // Force to Profile Menu
        {
            OpenProfileMenu();
            return;
        }
        else // To Main Menu
        {
            SetProfileData(); // From Save

            int unlockedLevel = LevelDataSingleton.Instance.GetUnlockedLevel();
            bool isWon = LevelDataSingleton.Instance.isWon;
            if (unlockedLevel != -1 && isWon) //If Match Won
            {
                levelUnlocked[unlockedLevel - 1] = true;

                //Might Seperate this
                SaveManagerSingleton.Instance.GetSelectedSaveData().levelUnlock = levelUnlocked;
                SaveManagerSingleton.Instance.SaveProfileData();
            }

            OpenMainMenu();
        }
        
    }

    private void HideAllMenu()
    {
        mainMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
        optionMenu.gameObject.SetActive(false);
        encyclopediaMenu.gameObject.SetActive(false);
    }

    private void OnProfileSelected()
    {
        SetProfileData();
        OpenMainMenu();
    }
    private void SetProfileData()
    {
        SaveData saveData = SaveManagerSingleton.Instance.GetSelectedSaveData();
        if (saveData != null)
        {
            levelUnlocked = saveData.levelUnlock;
        }
    }
    private void StartLevel(LevelInfo levelSelected)
    {
        LevelDataSingleton.Instance.SetCurrentLevelInfo(levelSelected);
        SceneManager.LoadScene(1);
    }

    #region Menu
    // Might just combine to one function
    private void OpenProfileMenu()
    {
        HideAllMenu();
        profileMenu.gameObject.SetActive(true);
        profileMenu.InitializeProfile();
    }
    private void OpenLevelMenu()
    {
        HideAllMenu();
        levelMenu.gameObject.SetActive(true);
        levelMenu.InitializeLevelMenu(levelUnlocked);
    }
    private void OpenMainMenu() 
    {
        HideAllMenu();
        mainMenu.gameObject.SetActive(true);
        mainMenu.InitializeMenu(SaveManagerSingleton.Instance.GetProfileId());
    }
    private void OpenOptionMenu()
    {
        HideAllMenu();
        optionMenu.gameObject.SetActive(true);
    }
    private void OpenEncyclopediaMenu()
    {
        HideAllMenu();
        encyclopediaMenu.gameObject.SetActive(true);
        encyclopediaMenu.InitializeUnits(placeholderDataTest);
    }
    #endregion

    private void OnDestroy()
    {
        profileMenu.ReturnButtonPressed -= OpenMainMenu;
        profileMenu.ProfileSelected -= OnProfileSelected;
        mainMenu.OnStartButtonPressed -= OpenLevelMenu;
        mainMenu.OnOptionButtonPressed -= OpenOptionMenu;
        mainMenu.OnEnyclopediaButtonPressed -= OpenEncyclopediaMenu;
        mainMenu.OnChangeProfileButtonPressed -= OpenProfileMenu;
        optionMenu.OnReturnButtonPressed -= OpenMainMenu;
        encyclopediaMenu.OnReturnButtonPressed -= OpenMainMenu;
        levelMenu.ReturnButtonPressed -= OpenMainMenu;
        levelMenu.LevelSelected -= StartLevel;
    }
}
