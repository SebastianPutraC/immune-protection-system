using System;
using UnityEngine;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] private PauseMenuController pauseMenu;
    [SerializeField] private OptionMenuController optionMenu;
    [SerializeField] private EncyclopediaMenuController encyclopediaMenu;

    public Action ResumePressed;
    public Action QuitPressed;

    private void Awake()
    {
        pauseMenu.OptionButtonPressed += OpenOptionMenu;
        pauseMenu.EncyclopediaButtonPressed += OpenEncyclopediaMenu;
        pauseMenu.ResumeButtonPressed += OnResumePressed;
        pauseMenu.QuitButtonPressed += OnQuitPressed;
        optionMenu.OnReturnButtonPressed += OpenPauseMenu;
        encyclopediaMenu.OnReturnButtonPressed += OpenPauseMenu;
    }

    private void OnEnable()
    {
        OpenPauseMenu();
    }

    private void HideAllMenu()
    {
        pauseMenu.gameObject.SetActive(false);
        optionMenu.gameObject.SetActive(false);
        encyclopediaMenu.gameObject.SetActive(false);
    }
    private void OpenPauseMenu()
    {
        HideAllMenu();
        pauseMenu.gameObject.SetActive(true);
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

    private void OnResumePressed()
    {
        ResumePressed?.Invoke();
    }
    private void OnQuitPressed()
    {
        QuitPressed?.Invoke();
    }

    private void OnDestroy()
    {
        pauseMenu.OptionButtonPressed -= OpenOptionMenu;
        pauseMenu.EncyclopediaButtonPressed -= OpenEncyclopediaMenu;
        pauseMenu.ResumeButtonPressed -= OnResumePressed;
        pauseMenu.QuitButtonPressed -= OnQuitPressed;
        optionMenu.OnReturnButtonPressed -= OpenPauseMenu;
        encyclopediaMenu.OnReturnButtonPressed -= OpenPauseMenu;
    }
}
