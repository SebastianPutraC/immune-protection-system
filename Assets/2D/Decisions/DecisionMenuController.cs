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

    [SerializeField] private Button continueButtonPrefab;
    private Button continueButton;

    public Action<DecisionButtonSO, int> DecisionChosen;
    public Action ContinueAnswer;

    public void InitializeMenu(DecisionInfoSO data, StoryPhase phase)
    {
        ResetMenu();

        int fontSize = 24;
        if (phase == StoryPhase.QUIZ)
        {
            fontSize = 36;
        }

        decisionDescription.alignment = TextAlignmentOptions.Justified;
        decisionDescription.fontSize = fontSize;
        decisionDescription.text = data.DecisionInfoText;

        for (int i = 0; i < data.DecisionButtons.Count; i++)
        {
            DecisionButton newDecision = Instantiate(buttonPrefab, decisionButtonsContent);
            newDecision.InitializeButton(data.DecisionButtons[i]);
            newDecision.DecisionSelected += OnDecisionSelected;
            decisionButtons.Add(newDecision);
        }
    }
    public void InitializeMenuQuizAnswer(string quizAnswer, bool isCorrect)
    {
        ResetMenu();
        decisionDescription.alignment = TextAlignmentOptions.Center;
        decisionDescription.fontSize = 36;
        if (isCorrect)
        {
            decisionDescription.text = "ANDA BENAR :)";
        }
        else
        {
            decisionDescription.text = "ANDA SALAH :( \n\n" + "Jawaban yang benar adalah : \n" + quizAnswer;
        }

        continueButton = Instantiate(continueButtonPrefab, decisionButtonsContent);
        continueButton.onClick.AddListener(OnContinueSelected);
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

    private void OnDecisionSelected(DecisionButtonSO buttonData, int setId = -1)
    {
        DecisionChosen?.Invoke(buttonData, setId);
    }
    private void OnContinueSelected()
    {
        continueButtonPrefab.onClick.RemoveListener(OnContinueSelected);
        Destroy(continueButton.gameObject);
        ContinueAnswer?.Invoke();
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
