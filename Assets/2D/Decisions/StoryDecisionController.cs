using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryDecisionController : MonoBehaviour
{
    private LevelInfo levelInfo;

    [SerializeField] private DecisionMenuController decisionMenuController;
    [SerializeField] private BonusPanelController BoostPanelController;
    [SerializeField] private BonusPanelController NerfPanelController;

    [SerializeField] private GameObject readyMenu;
    [SerializeField] private Button startLevel;

    private List<DecisionInfoSO> decisionInfos = new List<DecisionInfoSO>();
    private List<DecisionInfoSO> quizInfos = new List<DecisionInfoSO>();
    [SerializeField] private DecisionInfoSO quizDecisionPrompt;

    private int currentIndex = 1;
    private StoryPhase currentPhase = StoryPhase.NONE;

    private bool isPlayQuiz = false;
    private int currentScore;
    private int depleteScore;

    private List<BoostData> allyBoostData = new List<BoostData>();
    private List<BoostData> enemyBoostData = new List<BoostData>(); // Might never be used

    private void Awake()
    {
        decisionMenuController.DecisionChosen += OnDecisionChosen;
        startLevel.onClick.AddListener(OnStartSelected);
    }

    private void Start()
    {
        currentPhase = StoryPhase.NONE;
        levelInfo = LevelDataSingleton.Instance.GetCurrentLevelInfo();

        decisionMenuController.gameObject.SetActive(true);
        readyMenu.SetActive(false);

        if (levelInfo.DecisionInfos.Count > 0 || levelInfo.QuizInfos.Count > 0)
        {
            if (levelInfo.QuizInfos.Count > 0)
            {
                currentPhase = StoryPhase.PROMPT;
                quizInfos = levelInfo.QuizInfos;
            }
            if (levelInfo.DecisionInfos.Count > 0)
            {
                currentPhase = StoryPhase.DECISION;
                decisionInfos = levelInfo.DecisionInfos;
            }

            DecisionInfoSO newDecision = FindDecision(currentIndex);
            NextDecision(newDecision);
        }
        else
        {
            currentPhase = StoryPhase.READY;
            OpenReadyMenu();
        }
    }

    private void OnDecisionChosen(DecisionButtonSO buttonInfo, int setId = -1)
    {
        if (setId == -1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = setId;
        }

        if (buttonInfo.StatMod.StatToMod == StatType.NONE)
        {
            if (buttonInfo.StatMod.ModValue < 0)
            {
                NerfPanelController.UpdateBonusText(buttonInfo.StatMod.StatToMod, buttonInfo.StatMod.ModValue);
            }
            else
            {
                BoostPanelController.UpdateBonusText(buttonInfo.StatMod.StatToMod, buttonInfo.StatMod.ModValue);
            }

            BoostData newBoost = new BoostData(buttonInfo.StatMod);
            allyBoostData.Add(newBoost);
        }

        if (buttonInfo.IsQuiz)
        {
            if (!buttonInfo.IsCorrect)
            {
                currentScore -= depleteScore;
            }
        }

        CheckPhase();
        if (currentPhase != StoryPhase.NONE && currentPhase != StoryPhase.READY)
        {
            DecisionInfoSO newDecision = null;
            newDecision = FindDecision(currentIndex);
            NextDecision(newDecision);
        }
        else
        {
            OpenReadyMenu();
        }
    }
    private void CheckPhase()
    {
        switch (currentPhase)
        {
            case StoryPhase.DECISION:
                if (!decisionInfos.Any(x => x.decisionId == currentIndex))
                {
                    if (quizInfos.Count > 0)
                    {
                        currentPhase = StoryPhase.PROMPT;
                    }
                    else
                    {
                        currentPhase = StoryPhase.READY;
                    }
                }
                break;
            case StoryPhase.PROMPT:
                if (currentIndex == 0)
                {
                    currentPhase = StoryPhase.READY;
                }
                else
                {
                    currentPhase = StoryPhase.QUIZ;

                    isPlayQuiz = true;
                    currentScore = 100;
                    depleteScore = 100 / quizInfos.Count;
                }
                break;
            case StoryPhase.QUIZ:
                if (!quizInfos.Any(x => x.decisionId == currentIndex))
                {
                    currentPhase = StoryPhase.READY; 
                }
                break;
        }
    }
    private DecisionInfoSO FindDecision(int index)
    {
        DecisionInfoSO newDecision = null;
        switch (currentPhase)
        {
            case StoryPhase.DECISION:
                newDecision = decisionInfos.Where(x => x.decisionId == index).FirstOrDefault();
                break;
            case StoryPhase.PROMPT:
                newDecision = quizDecisionPrompt;
                break;
            case StoryPhase.QUIZ:
                newDecision = quizInfos.Where(x => x.decisionId == index).FirstOrDefault();
                break;
        }

        return newDecision;
    }
    private void NextDecision(DecisionInfoSO decisionInfo)
    {
        decisionMenuController.InitializeMenu(decisionInfo);
    }
    private void OpenReadyMenu()
    {
        currentPhase = StoryPhase.READY;
        decisionMenuController.gameObject.SetActive(false);
        readyMenu.SetActive(true);
    }

    private void OnStartSelected()
    {
        if (isPlayQuiz)
        {
            SaveManagerSingleton.Instance.UpdateScore(levelInfo.levelId, currentScore);
        }

        LevelDataSingleton.Instance.SetModifiersData(allyBoostData, UnitSide.ALLY);
        SceneManager.LoadScene(2);
    }

    private void OnDisable()
    {
        decisionMenuController.DecisionChosen -= OnDecisionChosen;
        startLevel.onClick.RemoveListener(OnStartSelected);
    }
    private void OnDestroy()
    {
        decisionMenuController.DecisionChosen -= OnDecisionChosen;
        startLevel.onClick.RemoveListener(OnStartSelected);
    }
}

public enum StoryPhase
{
    NONE,
    DECISION,
    PROMPT,
    QUIZ,
    READY
}
