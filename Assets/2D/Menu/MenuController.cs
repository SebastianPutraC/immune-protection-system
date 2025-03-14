using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MainMenuController mainMenu;
    [SerializeField] private OptionMenuController optionMenu;
    [SerializeField] private EncyclopediaMenuController encyclopediaMenu;

    private void Awake()
    {
        mainMenu.OnStartButtonPressed += OpenMainMenu;
        mainMenu.OnOptionButtonPressed += OpenOptionMenu;
        mainMenu.OnEnyclopediaButtonPressed += OpenEncyclopediaMenu;
        optionMenu.OnReturnButtonPressed += OpenMainMenu;
        encyclopediaMenu.OnReturnButtonPressed += OpenMainMenu;
    }

    private void Start()
    {
        OpenMainMenu();
    }

    private void HideAllMenu()
    {
        mainMenu.gameObject.SetActive(false);
        optionMenu.gameObject.SetActive(false);
        encyclopediaMenu.gameObject.SetActive(false);
    }

    // Might just combine to one function
    private void StartGame()
    {

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
    }
    private void OnDestroy()
    {
        mainMenu.OnStartButtonPressed -= OpenMainMenu;
        mainMenu.OnOptionButtonPressed -= OpenOptionMenu;
        mainMenu.OnEnyclopediaButtonPressed -= OpenEncyclopediaMenu;
        optionMenu.OnReturnButtonPressed -= OpenMainMenu;
        encyclopediaMenu.OnReturnButtonPressed -= OpenMainMenu;
    }
}
