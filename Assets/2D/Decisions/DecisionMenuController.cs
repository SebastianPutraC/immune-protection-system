using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class DecisionMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI decisionDescription;
    [SerializeField] private Transform decisionButtonsContent;

    [SerializeField] private DecisionButton buttonPrefab;
    private List<DecisionButton> decisionButtons = new List<DecisionButton>();

    public Action<BoostData> DecisionChosen;

    public void InitializeMenu(DecisionInfoSO data)
    {
        decisionDescription.text = data.DecisionInfoText;

        for (int i = 0; i < data.DecisionButtons.Count; i++)
        {
            DecisionButton newDecision = Instantiate(buttonPrefab, decisionButtonsContent);
            newDecision.InitializeButton(data.DecisionButtons[i]);
            newDecision.DecisionSelected += OnDecisionSelected;
            decisionButtons.Add(newDecision);
        }
    }
    
    public void ResetMenu()
    {
        decisionDescription.text = "";

        for (int i = 0; i < decisionButtons.Count; i++)
        {
            decisionButtons[i].DecisionSelected -= OnDecisionSelected;
            Destroy(decisionButtons[i].gameObject);
        }
        decisionButtons.Clear();
    }

    private void OnDecisionSelected(BoostData boostData)
    {
        DecisionChosen?.Invoke(boostData);
    }


    private void OnDisable()
    {
        for (int i = 0; i < decisionButtons.Count; i++)
        {
            decisionButtons[i].DecisionSelected -= OnDecisionSelected;
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < decisionButtons.Count; i++)
        {
            decisionButtons[i].DecisionSelected -= OnDecisionSelected;
        }
    }
}
