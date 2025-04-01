using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private ProfileMenuController profileMenu;
    [SerializeField] private LevelMenuController levelMenu;
    [SerializeField] private MainMenuController mainMenu;
    [SerializeField] private OptionMenuController optionMenu;
    [SerializeField] private EncyclopediaMenuController encyclopediaMenu;


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
            int unlockedLevel = LevelDataSingleton.Instance.GetUnlockedLevel();
            List<UnitId> unlockedUnit = LevelDataSingleton.Instance.GetAllUnits();
            bool isWon = LevelDataSingleton.Instance.isWon;

            SaveData selectedSaveData = SaveManagerSingleton.Instance.GetSelectedSaveData();
            if (unlockedLevel != -1 && isWon) //If Match Won
            {
                selectedSaveData.levelUnlock[unlockedLevel - 1] = true;
                for (int i = 0; i < unlockedUnit.Count; i++)
                {
                    int unitId = (int)unlockedUnit[i];
                    selectedSaveData.enyclopediaUnlock.Where(x => x.UnitId.Equals(unitId)).FirstOrDefault().Unlocked = true;
                }

                SaveManagerSingleton.Instance.SetProfileData(selectedSaveData);
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
        OpenMainMenu();
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
        levelMenu.InitializeLevelMenu(SaveManagerSingleton.Instance.GetSelectedSaveData().levelUnlock);
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
        encyclopediaMenu.InitializeUnits(SaveManagerSingleton.Instance.GetSelectedSaveData().enyclopediaUnlock);
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
