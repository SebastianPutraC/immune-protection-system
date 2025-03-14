using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncyclopediaMenuController : MonoBehaviour
{
    [SerializeField] private Button cellsPanelButton;
    [SerializeField] private Button pathogenPanelButton;

    [SerializeField] private Transform viewContent;

    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitDescriptionText;

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
