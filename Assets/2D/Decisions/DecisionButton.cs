using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class DecisionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button decisionButton;
    private DecisionButtonSO decisionInfo;

    private int setId = -1;

    public Action<DecisionButtonSO, int> DecisionSelected;

    private void Awake()
    {
        decisionButton.onClick.AddListener(ButtonClicked);
    }

    public void InitializeButton(DecisionButtonSO info)
    {
        buttonText.text = info.DecisionButtonText;
        decisionInfo = info;
        if (info.IsPickNextInfo == true)
        {
            setId = info.NextDecisionID;
        }
    }

    private void ButtonClicked()
    {
        DecisionSelected?.Invoke(decisionInfo, setId);
    }

    private void OnDestroy()
    {
        decisionButton.onClick.RemoveListener(ButtonClicked);
    }
}
