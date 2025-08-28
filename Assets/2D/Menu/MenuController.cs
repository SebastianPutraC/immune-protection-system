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
        OpenMainMenu(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && mainMenu.gameObject.activeSelf == true)
        {
            SaveManagerSingleton.Instance.UseBetaProfile();
            OpenMainMenu(true);
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

    private void StartLevel(LevelId levelSelected)
    {
        if (levelSelected == LevelId.LEVEL_TUTORIAL)
        {
            LevelDataSingleton.Instance.SetCurrentLevelInfo(levelSelected);
            SceneManager.LoadScene(3);
            return;
        }
        LevelDataSingleton.Instance.SetCurrentLevelInfo(levelSelected);
        SceneManager.LoadScene(1);
    }

    #region Menu
    // Might just combine to one function
    private void OpenProfileMenu()
    {
        HideAllMenu();
        profileMenu.gameObject.SetActive(true);
        profileMenu.InitializeProfile(SaveManagerSingleton.Instance.GetSelectedSaveData());

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }
    private void OpenLevelMenu()
    {
        HideAllMenu();
        levelMenu.gameObject.SetActive(true);
        levelMenu.InitializeLevelMenu(SaveManagerSingleton.Instance.GetSelectedSaveData().levelUnlock, 
            LevelDataSingleton.Instance.GetAllLevelReferences(), SaveManagerSingleton.Instance.GetSelectedSaveData().isWonAllLevel);

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }
    private void OpenMainMenu(bool playButtonSound) 
    {
        HideAllMenu();
        mainMenu.gameObject.SetActive(true);
        mainMenu.InitializeMenu(SaveManagerSingleton.Instance.GetSelectedSaveData().profileName);

        if (playButtonSound)
        {
            AudioManagerSingleton.Instance.PlayButtonSFX();
        }
    }
    private void OpenOptionMenu()
    {
        HideAllMenu();
        optionMenu.gameObject.SetActive(true);

        float musicVolume = SaveManagerSingleton.Instance.GetSelectedSaveData().musicVolume;
        float sfxVolume = SaveManagerSingleton.Instance.GetSelectedSaveData().sfxVolume;
        optionMenu.InitializeSetting(musicVolume, sfxVolume);

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }
    private void OpenEncyclopediaMenu()
    {
        HideAllMenu();
        encyclopediaMenu.gameObject.SetActive(true);
        encyclopediaMenu.InitializeEncyclopedia(SaveManagerSingleton.Instance.GetSelectedSaveData().enyclopediaUnlock);

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }
    #endregion

    private void OnDestroy()
    {
        profileMenu.ReturnButtonPressed -= OpenMainMenu;
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
