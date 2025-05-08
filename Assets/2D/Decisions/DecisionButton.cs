using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class DecisionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button decisionButton;

    private BoostData boostData = new BoostData();
    private int setId = -1;

    public Action<BoostData, int> DecisionSelected;

    private void Awake()
    {
        decisionButton.onClick.AddListener(ButtonClicked);
    }

    public void InitializeButton(DecisionButtonSO info)
    {
        buttonText.text = info.DecisionButtonText;
        boostData.StatMod = info.StatMod;
        if (info.IsPickNextInfo == true)
        {
            setId = info.NextDecisionID;
        }
    }

    private void ButtonClicked()
    {
        DecisionSelected?.Invoke(boostData, setId);
    }

    private void OnDestroy()
    {
        decisionButton.onClick.RemoveListener(ButtonClicked);
    }
}
