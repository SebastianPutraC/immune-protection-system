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
            OpenMainMenu();
        }
        
    }

    private void HideAllMenu()
    {
        mainMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
        optionMenu.gameObject.SetActive(false);
        encyclopediaMenu.gameObject.SetActive(false);
        profileMenu.gameObject.SetActive(false);
    }

    private void OnProfileSelected()
    {
        OpenMainMenu();
    }
    private void StartLevel(LevelId levelSelected)
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
        levelMenu.InitializeLevelMenu(SaveManagerSingleton.Instance.GetSelectedSaveData().levelUnlock, LevelDataSingleton.Instance.GetAllLevelReferences());
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
