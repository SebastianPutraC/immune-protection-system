using System;
using UnityEngine;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] private PauseMenuController pauseMenu;
    [SerializeField] private OptionMenuController optionMenu;
    [SerializeField] private EncyclopediaMenuController encyclopediaMenu;

    [SerializeField] private AudioSource pauseSound;

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
        pauseSound.Play();
        OpenPauseMenu(false);
    }

    private void HideAllMenu()
    {
        pauseMenu.gameObject.SetActive(false);
        optionMenu.gameObject.SetActive(false);
        encyclopediaMenu.gameObject.SetActive(false);
    }
    private void OpenPauseMenu(bool playButtonSound)
    {
        HideAllMenu();
        pauseMenu.gameObject.SetActive(true);

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
