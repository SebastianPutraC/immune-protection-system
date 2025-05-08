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

    private int decisionIndex = 0;
    private List<DecisionInfoSO> decisionInfos = new List<DecisionInfoSO>();

    private List<BoostData> allyBoostData = new List<BoostData>();

    private List<BoostData> enemyBoostData = new List<BoostData>(); // Might never be used

    private void Awake()
    {
        decisionMenuController.DecisionChosen += OnDecisionChosen;
        startLevel.onClick.AddListener(OnStartSelected);
    }

    private void Start()
    {
        levelInfo = LevelDataSingleton.Instance.GetCurrentLevelInfo();

        if (levelInfo.DecisionInfos.Count > 0)
        {
            decisionInfos = levelInfo.DecisionInfos;
            readyMenu.SetActive(false);
        }
        else
        {
            decisionMenuController.gameObject.SetActive(false);
            readyMenu.SetActive(true);
        }

        NextDecision(decisionIndex);
    }

    private void OnDecisionChosen(BoostData boostData, int setId = -1)
    {
        if (setId <= -1)
        {
            decisionIndex++;
        }
        else
        {
            decisionIndex = decisionInfos.Where(x => x.decisionId == setId).FirstOrDefault().decisionId;
        }

        allyBoostData.Add(boostData);

        if (boostData.StatMod.ModValue < 0)
        {
            NerfPanelController.UpdateBonusText(boostData.StatMod.StatToMod, boostData.StatMod.ModValue);
        }
        else
        {
            BoostPanelController.UpdateBonusText(boostData.StatMod.StatToMod, boostData.StatMod.ModValue);
        }

        NextDecision(decisionIndex);
    }
    private void NextDecision(int index)
    {
        decisionMenuController.ResetMenu();

        if (index >= decisionInfos.Count)
        {
            decisionMenuController.gameObject.SetActive(false);
            readyMenu.SetActive(true);
            return;
        }

        decisionMenuController.InitializeMenu(decisionInfos[index]);
    }

    private void OnStartSelected()
    {
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
