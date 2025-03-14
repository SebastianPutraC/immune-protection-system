using System;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaMenuController : MonoBehaviour
{
    [SerializeField] private Button returnButton;

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
