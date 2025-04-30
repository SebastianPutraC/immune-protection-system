using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TutorialPhase
{
    DEPLOYNEUTRO,
    BASICMOVE,
    STOPABILITY,
    STOPALL,
    BASICATTACK,
    NETABILITY,
    DEPLOYMAKRO,
    DIFFLANE,
    ANTIGEN,
    UNLOCKADAPTIVE,
    OBJECTIVEDEF,
    OBJECTIVEATK,
    FREEPLAY,
    NONE
}

public class TutorialController : BattleController
{
    [SerializeField] private Button skipTutorial;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private InformationPopup infoPopup;

    [SerializeField] private GameObject inGameIndicator;
    [SerializeField] private GameObject uiIndicator;

    #region Info Lists
    [SerializeField] private List<InformationSO> deployNeutroInfo;
    [SerializeField] private List<InformationSO> basicMoveInfo;
    [SerializeField] private List<InformationSO> stopAbilityInfo;
    [SerializeField] private List<InformationSO> stopAllInfo;
    [SerializeField] private List<InformationSO> basicAttackInfo;
    [SerializeField] private List<InformationSO> netAbilityInfo;
    [SerializeField] private List<InformationSO> deployMakroInfo;
    [SerializeField] private List<InformationSO> diffLaneInfo;
    [SerializeField] private List<InformationSO> antigenInfo;
    [SerializeField] private List<InformationSO> unlockAdaptiveInfo;
    [SerializeField] private List<InformationSO> objectiveDefInfo;
    [SerializeField] private List<InformationSO> objectiveAtkInfo;
    [SerializeField] private List<InformationSO> freePlayInfo;
    #endregion

    [SerializeField] private TutorialPhase phase;

    public void ShowInfoPopup(List<InformationSO> informations)
    {
        Time.timeScale = 0;
    }
}
