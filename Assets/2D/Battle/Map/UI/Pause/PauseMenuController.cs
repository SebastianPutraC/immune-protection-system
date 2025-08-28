using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button encylopediaButton;
    [SerializeField] private Button quitButton;

    public Action ResumeButtonPressed;
    public Action OptionButtonPressed;
    public Action EncyclopediaButtonPressed;
    public Action QuitButtonPressed;

    private void Awake()
    {
        resumeButton.onClick.AddListener(OnResumeButtonPressed);
        optionButton.onClick.AddListener(OnOptionButtonPressed);
        encylopediaButton.onClick.AddListener(OnEncyclopediaButtonPressed);
        quitButton.onClick.AddListener(OnQuitButtonPressed);
    }

    private void OnResumeButtonPressed()
    {
        ResumeButtonPressed?.Invoke();
    }
    private void OnOptionButtonPressed()
    {
        OptionButtonPressed?.Invoke();
    }
    private void OnEncyclopediaButtonPressed()
    {
        EncyclopediaButtonPressed?.Invoke();
    }
    private void OnQuitButtonPressed()
    {
        QuitButtonPressed?.Invoke();
    }

    private void OnDestroy()
    {
        resumeButton.onClick.AddListener(OnResumeButtonPressed);
        optionButton.onClick.RemoveListener(OnOptionButtonPressed);
        encylopediaButton.onClick.RemoveListener(OnEncyclopediaButtonPressed);
        quitButton.onClick.AddListener(OnQuitButtonPressed);
    }
}
