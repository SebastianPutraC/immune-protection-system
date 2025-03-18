using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenuController : MonoBehaviour
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public Action OnReturnButtonPressed;

    private void Awake()
    {
        returnButton.onClick.AddListener(ReturnButtonPressed);
    }

    private void ReturnButtonPressed()
    {
        OnReturnButtonPressed?.Invoke();
    }

    private void OnDestroy()
    {
        returnButton.onClick.RemoveListener(ReturnButtonPressed);
    }
}
