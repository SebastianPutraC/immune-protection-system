using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class DecisionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button decisionButton;

    private BoostData boostData = new BoostData();

    public Action<BoostData> DecisionSelected;

    private void Awake()
    {
        decisionButton.onClick.AddListener(ButtonClicked);
    }

    public void InitializeButton(DecisionButtonSO info)
    {
        buttonText.text = info.DecisionButtonText;
        boostData.StatMod = info.StatMod;
    }

    private void ButtonClicked()
    {
        DecisionSelected?.Invoke(boostData);
    }

    private void OnDestroy()
    {
        decisionButton.onClick.RemoveListener(ButtonClicked);
    }
}
