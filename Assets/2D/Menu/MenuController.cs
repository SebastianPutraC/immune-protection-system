using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private LevelMenuController levelMenu;
    [SerializeField] private MainMenuController mainMenu;
    [SerializeField] private OptionMenuController optionMenu;
    [SerializeField] private EncyclopediaMenuController encyclopediaMenu;

    [SerializeField] private bool[] levelUnlocked = new bool[5];

    //PLACEHOLDER
    [SerializeField] private List<EncylopediaUnit> placeholderDataTest = new List<EncylopediaUnit>();

    private void Awake()
    {
        mainMenu.OnStartButtonPressed += OpenLevelMenu;
        mainMenu.OnOptionButtonPressed += OpenOptionMenu;
        mainMenu.OnEnyclopediaButtonPressed += OpenEncyclopediaMenu;
        optionMenu.OnReturnButtonPressed += OpenMainMenu;
        encyclopediaMenu.OnReturnButtonPressed += OpenMainMenu;
        levelMenu.ReturnButtonPressed += OpenMainMenu;
        levelMenu.LevelSelected += StartLevel;
    }

    private void Start()
    {
        int unlockedLevel = LevelDataSingleton.Instance.GetUnlockedLevel();
        bool isWon = LevelDataSingleton.Instance.isWon;
        if (unlockedLevel != -1 && isWon)
        {
            levelUnlocked[unlockedLevel - 1] = true;
        }

        OpenMainMenu();
    }

    private void HideAllMenu()
    {
        mainMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
        optionMenu.gameObject.SetActive(false);
        encyclopediaMenu.gameObject.SetActive(false);
    }

    private void StartLevel(LevelInfo levelSelected)
    {
        LevelDataSingleton.Instance.SetCurrentLevelInfo(levelSelected);
        SceneManager.LoadScene(1);
    }

    // Might just combine to one function
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
    private void OnDestroy()
    {
        mainMenu.OnStartButtonPressed -= OpenLevelMenu;
        mainMenu.OnOptionButtonPressed -= OpenOptionMenu;
        mainMenu.OnEnyclopediaButtonPressed -= OpenEncyclopediaMenu;
        optionMenu.OnReturnButtonPressed -= OpenMainMenu;
        encyclopediaMenu.OnReturnButtonPressed -= OpenMainMenu;
        levelMenu.ReturnButtonPressed -= OpenMainMenu;
        levelMenu.LevelSelected -= StartLevel;
    }
}
