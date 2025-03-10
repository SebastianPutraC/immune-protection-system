using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button encylopediaButton;
    [SerializeField] private Button exitButton;

    public Action OnStartButtonPressed;
    public Action OnOptionButtonPressed;
    public Action OnEnyclopediaButtonPressed;

    private void Start()
    {
        startButton.onClick.AddListener(StartButtonPressed);
        optionButton.onClick.AddListener(OptionButtonPressed);
        encylopediaButton.onClick.AddListener(EncylopediaButtonPressed);
        exitButton.onClick.AddListener(ExitButtonPressed);
    }

    private void StartButtonPressed()
    {
        OnStartButtonPressed?.Invoke();
    }
    private void OptionButtonPressed()
    {
        OnOptionButtonPressed?.Invoke();
    }
    private void EncylopediaButtonPressed()
    {
        OnEnyclopediaButtonPressed?.Invoke();
    }
    private void ExitButtonPressed()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(StartButtonPressed);
        optionButton.onClick.RemoveListener(OptionButtonPressed);
        encylopediaButton.onClick.RemoveListener(EncylopediaButtonPressed);
        exitButton.onClick.RemoveListener(ExitButtonPressed);
    }
}
