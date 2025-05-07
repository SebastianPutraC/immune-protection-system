using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TutorialPhase
{
    NONE,
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
    FREEPLAY
}

public class TutorialController : BattleController
{
    [SerializeField] private Button skipTutorial;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private InformationPopup infoPopup;

    [SerializeField] private GameObject inGameIndicator;
    [SerializeField] private GameObject uiIndicator;

    [SerializeField] private DeployedUnitController tutorialNeutrophil;
    [SerializeField] private DeployedUnitController tutorialMacrophage; //Used for Stop All and Macrophage
    [SerializeField] private DeployedUnitController tutorialBacteriaOne;
    [SerializeField] private DeployedUnitController tutorialBacteriaTwo; //For NET Ability and probably Antigen

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

    private float waitTimer = 0f;

    private bool isUnitSpawn;
    private bool isSingularStop;
    private bool isSingularAdvance;
    private bool isAllStop;
    private bool isAllAdvance;
    private bool isUnitKilled;
    private bool isCollecting;
    private bool isTimerDone;

    [SerializeField] private TutorialPhase phase = TutorialPhase.NONE;
    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < allySpawners.Count; i++)
        {
            allySpawners[i].AllySpawned += OnUnitSpawned;
        }
        infoPopup.PopupFinished += OnInfoPopupFinished;
    }
    protected override void Update()
    {
        if (waitTimer > 0 && !isTimerDone)
        {
            waitTimer -= Time.deltaTime;
        }
        else
        {
            isTimerDone = true;
        }

        CheckPhase(phase);
        if (phase == TutorialPhase.FREEPLAY)
        {
            base.Update();
        }
        else
        {
            if (selectedUnit != null)
            {
                unitStatsController.UpdateUnitStats(selectedUnit);
            }
        }
    }

    private void CheckPhase(TutorialPhase currentPhase)
    {
        bool isPhaseDone = false;
        switch (currentPhase)
        {
            case TutorialPhase.DEPLOYNEUTRO:
                if (isUnitSpawn && isTimerDone && allAllyUnits.Count > 0)
                    isPhaseDone = true;
                break;
            case TutorialPhase.BASICMOVE:
                if (isTimerDone)
                    isPhaseDone = true;
                break;
            case TutorialPhase.STOPABILITY:
                if (isSingularStop && isSingularAdvance && isTimerDone)
                    isPhaseDone = true;
                break;
            case TutorialPhase.STOPALL:
                if (isAllStop && isAllAdvance && isTimerDone)
                    isPhaseDone = true;
                break;
            case TutorialPhase.BASICATTACK:
                if (isUnitKilled && isTimerDone)
                    isPhaseDone = true;
                break;
            case TutorialPhase.NETABILITY:
                if (isUnitKilled && isTimerDone)
                    isPhaseDone = true;
                break;
            case TutorialPhase.DEPLOYMAKRO:
                if (isUnitSpawn && isTimerDone && allAllyUnits.Count > 0)
                    isPhaseDone = true;
                break;
            case TutorialPhase.DIFFLANE:
                if (isTimerDone)
                    isPhaseDone = true;
                break;
            case TutorialPhase.ANTIGEN:
                if (isCollecting && isAdaptiveUnlocked)
                    isPhaseDone = true;
                break;
            case TutorialPhase.UNLOCKADAPTIVE:
                if (isUnitSpawn && isTimerDone && allAllyUnits.Count > 0)
                    isPhaseDone = true;
                break;
            case TutorialPhase.OBJECTIVEDEF:
                if (isTimerDone)
                    isPhaseDone = true;
                break;
            case TutorialPhase.OBJECTIVEATK:
                if (isTimerDone)
                    isPhaseDone = true;
                break;
            default:
                break;
        }

        if (isPhaseDone)
        {
            ResetBoolChecker();
            NextPhase();
        }
    }
    private void NextPhase()
    {
        phase++;
        List<InformationSO> currentInfos = GetPhaseInformation(phase);
        if (currentInfos != null)
        {
            ShowInfoPopup(currentInfos);
        }
        else
        {
            Debug.LogWarning("No Info on this phase : " + phase + " | RETURNING");
            phase--;
        }
    }

    private void ShowInfoPopup(List<InformationSO> informations)
    {
        infoPopup.gameObject.SetActive(true);
        infoPopup.InitializePopup(informations);

        Time.timeScale = 0;
    }
    private void OnInfoPopupFinished()
    {
        Time.timeScale = 1;
        switch (phase)
        {
            case TutorialPhase.DEPLOYNEUTRO:
                break;
            case TutorialPhase.BASICMOVE:
                break;
            case TutorialPhase.STOPABILITY:
                break;
            case TutorialPhase.STOPALL:
                break;
            case TutorialPhase.BASICATTACK:
                break;
            case TutorialPhase.NETABILITY:
                break;
            case TutorialPhase.DEPLOYMAKRO:
                break;
            case TutorialPhase.DIFFLANE:
                break;
            case TutorialPhase.ANTIGEN:
                break;
            case TutorialPhase.UNLOCKADAPTIVE:
                break;
            case TutorialPhase.OBJECTIVEDEF:
                break;
            case TutorialPhase.OBJECTIVEATK:
                break;
            default:
                break;
        }
    }    

    private List<InformationSO> GetPhaseInformation(TutorialPhase currentPhase)
    {
        switch (currentPhase)
        {
            case TutorialPhase.DEPLOYNEUTRO:
                return deployNeutroInfo;
            case TutorialPhase.BASICMOVE:
                return basicMoveInfo;
            case TutorialPhase.STOPABILITY:
                return stopAbilityInfo;
            case TutorialPhase.STOPALL:
                return stopAllInfo;
            case TutorialPhase.BASICATTACK:
                return basicAttackInfo;
            case TutorialPhase.NETABILITY:
                return netAbilityInfo;
            case TutorialPhase.DEPLOYMAKRO:
                return deployNeutroInfo;
            case TutorialPhase.DIFFLANE:
                return diffLaneInfo;
            case TutorialPhase.ANTIGEN:
                return antigenInfo;
            case TutorialPhase.UNLOCKADAPTIVE:
                return unlockAdaptiveInfo;
            case TutorialPhase.OBJECTIVEDEF:
                return objectiveDefInfo;
            case TutorialPhase.OBJECTIVEATK:
                return objectiveAtkInfo;
            case TutorialPhase.FREEPLAY:
                return freePlayInfo;
            default:
                return null;
        }
    }
    private void ResetBoolChecker()
    {
        isUnitSpawn = false;
        isSingularStop = false;
        isSingularAdvance = false;
        isAllStop = false;
        isAllAdvance = false;
        isUnitKilled = false;
        isCollecting = false;
        isTimerDone = false;
    }


    protected override void OnAbilitySelected(AbilityId ability)
    {
        base.OnAbilitySelected(ability);
        switch (ability)
        {
            case AbilityId.MOVE:
                OnSingularStopped();
                break;
            case AbilityId.APC:
                OnCollectingActivated();
                break;
        }
    }
    protected override void StopAllUnit()
    {
        base.StopAllUnit();
        OnAllStopped();
    }
    protected override void AdvanceAllUnit()
    {
        base.AdvanceAllUnit();
        OnAllAdvance();
    }

    #region PhaseMechanic
    private void DeployNeutroPhase()
    {
        tutorialText.text = "Klik kartu unit “Neutrofil”";
    }
    #endregion

    #region EventFunctions
    private void OnUnitSpawned(AllyUnitController unitController)
    {
        switch (phase)
        {
            case TutorialPhase.DEPLOYNEUTRO:
                if (unitController.UnitSO.unitId == UnitId.NEUTROPHIL)
                {
                    isUnitSpawn = true;
                }
                break;
            case TutorialPhase.DEPLOYMAKRO:
                if (unitController.UnitSO.unitId == UnitId.MACROPHAGE)
                {
                    isUnitSpawn = true;
                }
                break;
            case TutorialPhase.UNLOCKADAPTIVE:
                if (unitController.UnitSO.unitId == UnitId.PLASMACELL)
                {
                    isUnitSpawn = true;
                }
                break;
        }
    }
    private void OnSingularStopped()
    {
        if (phase == TutorialPhase.STOPABILITY)
        {
            if (isSingularStop)
            {
                OnSingularAdvanced();
                return;
            }
            isSingularStop = true;
        }
    }
    private void OnSingularAdvanced()
    {
        if (phase == TutorialPhase.STOPABILITY)
        {
            isSingularAdvance = true;
        }
    }
    private void OnAllStopped()
    {
        if (phase == TutorialPhase.STOPALL)
        {
            isAllStop = true;
        }
    }
    private void OnAllAdvance()
    {
        if (phase == TutorialPhase.STOPALL)
        {
            isAllAdvance = true;
        }
    }
    private void OnUnitKilled(UnitController unitKilled)
    {
        if (phase == TutorialPhase.BASICATTACK || phase == TutorialPhase.NETABILITY)
        {
            if (allEnemyUnits.Count <= 0)
            {
                isUnitKilled = true;
            }
        }
    }
    private void OnCollectingActivated()
    {
        if (phase == TutorialPhase.ANTIGEN)
        {
            isCollecting = true;
        }
    }
    #endregion


    protected override void OnDestroy()
    {
        base.OnDestroy();
        for (int i = 0; i < allySpawners.Count; i++)
        {
            allySpawners[i].AllySpawned -= OnUnitSpawned;
        }
        infoPopup.PopupFinished -= OnInfoPopupFinished;
    }
}
