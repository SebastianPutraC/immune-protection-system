using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryDecisionController : MonoBehaviour
{
    private LevelInfo levelInfo;

    [SerializeField] private DecisionMenuController decisionMenuController;
    [SerializeField] private BonusPanelController allyBonusController;
    [SerializeField] private BonusPanelController enemyBonusController;

    [SerializeField] private GameObject readyMenu;
    [SerializeField] private Button startLevel;

    private int decisionIndex = 0;
    private List<DecisionInfoSO> decisionInfos = new List<DecisionInfoSO>();

    private List<BoostData> allyBoostData = new List<BoostData>();
    private List<BoostData> enemyBoostData = new List<BoostData>();

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

    private void OnDecisionChosen(BoostData boostData)
    {
        decisionIndex++;

        if (boostData.SideToBoost == UnitSide.ALLY)
        {
            allyBoostData.Add(boostData);
            allyBonusController.UpdateBonusText(boostData.StatMod.StatToMod, boostData.StatMod.ModValue);
        }
        else if (boostData.SideToBoost == UnitSide.ENEMY)
        {
            enemyBoostData.Add(boostData);
            enemyBonusController.UpdateBonusText(boostData.StatMod.StatToMod, boostData.StatMod.ModValue);
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
        LevelDataSingleton.Instance.SetModifiersData(enemyBoostData, UnitSide.ENEMY);
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
