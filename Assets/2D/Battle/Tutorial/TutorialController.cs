using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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
    ANTIGEN,
    UNLOCKADAPTIVE,
    OBJECTIVEDEF,
    OBJECTIVEATK,
    FREEPLAY
}

public class TutorialController : BattleController
{
    [SerializeField] private TextMeshProUGUI tutorialText;

    [SerializeField] private NeutrophilController tutorialNeutrophil;
    [SerializeField] private MacrophageController tutorialMacrophage; //Used for Stop All and Macrophage
    [SerializeField] private DeployedUnitController tutorialBacteriaOne;
    [SerializeField] private DeployedUnitController tutorialBacteriaTwo; //For NET Ability and probably Antigen
    [SerializeField] private DeployedUnitController deployableUnit;

    #region Info Lists
    [SerializeField] private List<InformationSO> deployNeutroInfo;
    [SerializeField] private List<InformationSO> basicMoveInfo;
    [SerializeField] private List<InformationSO> stopAbilityInfo;
    [SerializeField] private List<InformationSO> stopAllInfo;
    [SerializeField] private List<InformationSO> basicAttackInfo;
    [SerializeField] private List<InformationSO> netAbilityInfo;
    [SerializeField] private List<InformationSO> deployMakroInfo;
    [SerializeField] private List<InformationSO> antigenInfo;
    [SerializeField] private List<InformationSO> unlockAdaptiveInfo;
    [SerializeField] private List<InformationSO> objectiveDefInfo;
    [SerializeField] private List<InformationSO> objectiveAtkInfo;
    [SerializeField] private List<InformationSO> freePlayInfo;
    #endregion

    #region Arrows
    [SerializeField] private GameObject firstSpawnArrow;
    [SerializeField] private GameObject secondSpawnArrow;
    [SerializeField] private GameObject allyBaseArrow;
    [SerializeField] private GameObject enemyBaseArrow;
    [SerializeField] private GameObject firstRouteArrow;
    [SerializeField] private GameObject secondRouteArrow;

    [SerializeField] private GameObject intelPointArrow;
    [SerializeField] private GameObject allyHPArrow;
    [SerializeField] private GameObject enemyHPArrow;
    [SerializeField] private GameObject holdArrow;
    [SerializeField] private GameObject advanceArrow;
    #endregion

    private float waitTimer = 0f;
    private int subPhaseStep = 0;
    private bool isCheckTime = false;

    private bool isProcessRunning = false;
    private bool isPhaseDone = false;

    #region BoolChecker
    private bool isUnitSpawn = false;
    private bool isSingularStop = false;
    private bool isSingularAdvance = false;
    private bool isAllStop = false;
    private bool isAllAdvance = false;
    private bool isUnitKilled = false;
    private bool isCollecting = false;
    private bool isTimerDone = false;
    #endregion

    [SerializeField] private TutorialBarrier barrier;
    [SerializeField] private Transform moveBackNeutrophil;
    [SerializeField] private Transform moveBackMacrophage;

    [SerializeField] private Transform netAllyPosition;
    [SerializeField] private Transform netEnemyPosition;

    [SerializeField] private TutorialPhase phase = TutorialPhase.NONE;
    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < levelController.allySpawners.Count; i++)
        {
            levelController.allySpawners[i].AllySpawned += OnUnitSpawned;
        }
        infoPopup.PopupFinished += OnInfoPopupFinished;
        for (int i = 0; i < levelController.tutorialAllySpawners.Count; i++)
        {
            levelController.tutorialAllySpawners[i].AllySpawned += OnAllyTutorialSpawned;
        }
        for (int i = 0; i < levelController.tutorialEnemySpawners.Count; i++)
        {
            levelController.tutorialEnemySpawners[i].EnemySpawned += OnEnemyTutorialSpawned;
        }
        barrier.UnitHitBarrier += ReturnUnit;

        enemyAI.usingAI = false;
        unitPanelController.ResetUnitCards();
        NextPhase();
    }
    protected override void Update()
    {
        if (isCheckTime)
        {
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
            }
            else
            {
                isCheckTime = false;
                isTimerDone = true;
            }
        }

        if (phase == TutorialPhase.FREEPLAY)
        {
            enemyAI.usingAI = true;
            base.Update();
        }
        else
        {
            if (selectedUnit != null)
            {
                unitStatsController.UpdateUnitStats(selectedUnit);
            }
        }
        CheckPhase(phase);
    }

    private void CheckPhase(TutorialPhase currentPhase)
    {
        if (isPhaseDone)
        {
            ResetBoolChecker();
            ResetArrows();
            NextPhase();
        }

        switch (currentPhase)
        {
            case TutorialPhase.DEPLOYNEUTRO:
                CheckNeutroPhase();
                if (!isProcessRunning)
                {
                    DeployNeutroPhase();
                }
                break;
            case TutorialPhase.BASICMOVE:
                CheckBasicMovePhase();
                if (!isProcessRunning)
                {
                    BasicMovePhase();
                }
                break;
            case TutorialPhase.STOPABILITY:
                CheckStopAbilityPhase();
                if (!isProcessRunning)
                {
                    StopAbilityPhase();
                }
                break;
            case TutorialPhase.STOPALL:
                CheckStopAllPhase();
                if (!isProcessRunning)
                {
                    StopAllPhase();
                }
                break;
            case TutorialPhase.BASICATTACK:
                CheckBasicAttackPhase();
                if (!isProcessRunning)
                {
                    BasicAttackPhase();
                }
                break;
            case TutorialPhase.NETABILITY:
                CheckNetAbilityPhase();
                if (!isProcessRunning)
                {
                    NetAbilityPhase();
                }
                break;
            case TutorialPhase.DEPLOYMAKRO:
                CheckMacroPhase();
                if (!isProcessRunning)
                {
                    DeployMacroPhase();
                }
                break;
            case TutorialPhase.ANTIGEN:
                CheckAntigenPhase();
                if (!isProcessRunning)
                {
                    CollectAntigenPhase();
                }
                break;
            case TutorialPhase.UNLOCKADAPTIVE:
                CheckPlasmaPhase();
                if (!isProcessRunning)
                {
                    DeployPlasmaPhase();
                }
                break;
            case TutorialPhase.OBJECTIVEDEF:
                CheckObjectDefPhase();
                if (!isProcessRunning)
                {
                    ObjectDefPhase();
                }
                break;
            case TutorialPhase.OBJECTIVEATK:
                CheckObjectAtkPhase();
                if (!isProcessRunning)
                {
                    ObjectAtkPhase();
                }
                break;
            default:
                break;
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

        if (phase == TutorialPhase.FREEPLAY)
        {
            KillAllUnit();
            ResetArrows();
            ResetBoolChecker();

            unitPanelController.ResetSelectedCard();
            unitPanelController.ResetUnitCards();
            unitStatsController.ClearUI();

            SetUpLevel(LevelDataSingleton.Instance.GetCurrentLevelInfo());
            UnlockAdaptiveCells();

            unitPanelController.gameObject.SetActive(true);
            unitStatsController.gameObject.SetActive(true);
            AdvanceAllButton.gameObject.SetActive(true);
            StopAllButton.gameObject.SetActive(true);

            unitSelectionController.canSelect = true;
            unitStatsController.HideBlock();

            tutorialText.text = "";
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
        subPhaseStep = 1;
        isProcessRunning = false;
        Time.timeScale = 1;
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
                return deployMakroInfo;
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
    private void ResetArrows()
    {
        firstSpawnArrow.gameObject.SetActive(false);
        secondSpawnArrow.gameObject.SetActive(false);
        allyBaseArrow.gameObject.SetActive(false);
        enemyBaseArrow.gameObject.SetActive(false);
        firstRouteArrow.gameObject.SetActive(false);
        secondRouteArrow.gameObject.SetActive(false);

        holdArrow.gameObject.SetActive(false);
        advanceArrow.gameObject.SetActive(false);
        allyHPArrow.gameObject.SetActive(false);
        enemyHPArrow.gameObject.SetActive(false);
        intelPointArrow.gameObject.SetActive(false);

        unitPanelController.TurnOffArrows();
        unitStatsController.TurnOffArrows();

        if (tutorialMacrophage != null)
        {
            tutorialMacrophage.TutorialArrow.gameObject.SetActive(false);
        }
        if (tutorialNeutrophil != null)
        {
            tutorialNeutrophil.TutorialArrow.gameObject.SetActive(false);
        }
    }
    private void HideUI()
    {
        unitPanelController.gameObject.SetActive(false);
        unitStatsController.gameObject.SetActive(false);
        AdvanceAllButton.gameObject.SetActive(false);
        StopAllButton.gameObject.SetActive(false);
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
    private void CheckNeutroPhase()
    {
        isPhaseDone = false;
        if (isUnitSpawn && isTimerDone && allAllyUnits.Count > 0 && subPhaseStep == 3)
        {
            isPhaseDone = true;
            return;
        }

        switch (subPhaseStep)
        {
            case 1:
                if (unitPanelController.selectedCard != null)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
            case 2:
                if (isUnitSpawn && allAllyUnits.Count > 0)
                {
                    subPhaseStep = 3;
                    isProcessRunning = false;
                }
                break;
        }
    } //
    private void DeployNeutroPhase()
    {
        HideUI();
        unitPanelController.gameObject.SetActive(true);
        switch (subPhaseStep)
        {
            case 1:
                tutorialText.text = "Klik kartu unit Neutrofil di panel kartu unit";

                AllyUnit neutrofil = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.NEUTROPHIL).FirstOrDefault();

                List<AllyUnit> unitList = new List<AllyUnit>();
                unitList.Add(neutrofil);

                unitPanelController.ResetSelectedCard();
                unitPanelController.ResetUnitCards();
                unitPanelController.InitializeUnitCards(unitList);
                unitPanelController.TurnTutorialArrow(neutrofil.unitId);
                unitSelectionController.canSelect = false;
                break;
            case 2:
                ResetArrows();
                tutorialText.text = "Pilih tempat “Spawner” untuk unit Neutrofil Anda\n Klik tombol kiri Mouse dan seret Mouse untuk menavigasi medan permainan";
                firstSpawnArrow.gameObject.SetActive(true);
                secondSpawnArrow.gameObject.SetActive(true);
                break;
            case 3:
                ResetArrows();
                waitTimer = 1.5f;
                isCheckTime = true;
                tutorialText.text = "Mohon Menunggu";
                deployableUnit = allAllyUnits[0];
                break;
        }

        isProcessRunning = true;
    } //
    private void CheckBasicMovePhase()
    {
        isPhaseDone = false;
        if (isTimerDone)
        {
            isPhaseDone = true;
            return;
        }
    } //
    private void BasicMovePhase()
    {
        HideUI();
        switch (subPhaseStep)
        {
            case 1:
                ResetArrows();
                waitTimer = 2.5f;
                isCheckTime = true;

                firstRouteArrow.gameObject.SetActive(true);
                secondRouteArrow.gameObject.SetActive(true);

                tutorialText.text = "Mohon Menunggu";
                break;
        }

        isProcessRunning = true;
    } //
    private void CheckStopAbilityPhase()
    {
        isPhaseDone = false;
        if (isSingularStop && isSingularAdvance && isTimerDone)
        {
            isPhaseDone = true;
            return;
        }

        switch (subPhaseStep)
        {
            case 1:
                if (selectedUnit != null)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
            case 2:
                if (isSingularStop)
                {
                    subPhaseStep = 3;
                    isProcessRunning = false;
                }
                break;
            case 3:
                if (isSingularAdvance)
                {
                    subPhaseStep = 4;
                    isProcessRunning = false;
                }
                break;
        }
    }
    private void StopAbilityPhase()
    {
        HideUI();
        unitStatsController.gameObject.SetActive(true);
        switch (subPhaseStep)
        {
            case 1:
                KillAllUnit();

                AllyUnit neutrofil = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.NEUTROPHIL).FirstOrDefault();
                DeployAllyTutorialUnit(neutrofil, levelController.tutorialAllySpawners[0]);
                barrier.gameObject.SetActive(true);
                tutorialText.text = "Klik unit Neutrofil dalam medan";

                unitSelectionController.canSelect = true;
                break;
            case 2:
                unitStatsController.TurnTutorialArrow(AbilityId.MOVE);
                barrier.gameObject.SetActive(true);
                tutorialText.text = "Klik tombol Stop dalam panel Kemampuan di kiri bawah layar";

                unitStatsController.BlockAbility(false);
                break;
            case 3:
                unitStatsController.TurnTutorialArrow(AbilityId.MOVE);
                tutorialText.text = "Klik tombol Maju dalam panel Kemampuan di kiri bawah layar";
                break;
            case 4:
                ResetArrows();
                waitTimer = 1.5f;
                isCheckTime = true;
                tutorialText.text = "Mohon Menunggu";
                barrier.gameObject.SetActive(false);

                unitSelectionController.canSelect = false;
                break;
        }

        isProcessRunning = true;
    }
    private void CheckStopAllPhase()
    {
        isPhaseDone = false;
        if (isAllStop && isAllAdvance && isTimerDone)
        {
            isPhaseDone = true;
            return;
        }
        switch (subPhaseStep)
        {
            case 1:
                if (isAllStop)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
            case 2:
                if (isAllAdvance)
                {
                    subPhaseStep = 3;
                    isProcessRunning = false;
                }
                break;
        }
    }
    private void StopAllPhase()
    {
        HideUI();
        switch (subPhaseStep)
        {
            case 1:
                KillAllUnit();

                AllyUnit neutrofil = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.NEUTROPHIL).FirstOrDefault();
                DeployAllyTutorialUnit(neutrofil, levelController.tutorialAllySpawners[0]);

                AllyUnit macrophage = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.MACROPHAGE).FirstOrDefault();
                DeployAllyTutorialUnit(macrophage, levelController.tutorialAllySpawners[1]);

                barrier.gameObject.SetActive(true);

                StopAllButton.gameObject.SetActive(true);
                holdArrow.SetActive(true);
                tutorialText.text = "Klik tombol Semua Berhenti di kiri layar";
                break;
            case 2:
                holdArrow.SetActive(false);
                advanceArrow.SetActive(true);
                AdvanceAllButton.gameObject.SetActive(true);
                tutorialText.text = "Klik tombol Semua Maju di kanan layar";
                break;
            case 3:
                ResetArrows();
                waitTimer = 1.5f;
                isCheckTime = true;
                tutorialText.text = "Mohon Menunggu";
                barrier.gameObject.SetActive(false);
                break;
        }

        isProcessRunning = true;
    }
    private void CheckBasicAttackPhase()
    {
        isPhaseDone = false;
        if (isUnitKilled && isTimerDone)
        {
            isPhaseDone = true;
            return;
        }
        switch (subPhaseStep)
        {
            case 1:
                if (isUnitKilled)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
        }
    }
    private void BasicAttackPhase()
    {
        HideUI();
        switch (subPhaseStep)
        {
            case 1:
                KillAllUnit();

                AllyUnit neutrofil = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.NEUTROPHIL).FirstOrDefault();
                DeployAllyTutorialUnit(neutrofil, levelController.tutorialAllySpawners[0]);

                EnemyUnit bacteria = (EnemyUnit)UnitReferences.Where(x => x.unitId == UnitId.BACTERIA).FirstOrDefault();
                DeployEnemyTutorialUnit(bacteria, levelController.tutorialEnemySpawners[1]);

                tutorialBacteriaOne.DealDamage(50);

                tutorialText.text = "Mohon Menunggu";
                break;
            case 2:
                ResetArrows();
                waitTimer = 1.5f;
                isCheckTime = true;
                tutorialText.text = "Mohon Menunggu";
                break;
        }

        isProcessRunning = true;
    }
    private void CheckNetAbilityPhase()
    {
        isPhaseDone = false;
        if (isUnitKilled && isTimerDone)
        {
            isPhaseDone = true;
            return;
        }

        switch (subPhaseStep)
        {
            case 1:
                if (selectedUnit != null)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
            case 2:
                if (isUnitKilled)
                {
                    subPhaseStep = 3;
                    isProcessRunning = false;
                }
                break;
        }
    }
    private void NetAbilityPhase()
    {
        HideUI();
        unitStatsController.gameObject.SetActive(true);
        switch (subPhaseStep)
        {
            case 1:
                unitStatsController.HideBlock();
                KillAllUnit();
                AllyUnit neutrofil = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.NEUTROPHIL).FirstOrDefault();
                DeployAllyTutorialUnit(neutrofil, levelController.tutorialAllySpawners[0]);

                EnemyUnit bacteria = (EnemyUnit)UnitReferences.Where(x => x.unitId == UnitId.BACTERIA).FirstOrDefault();
                DeployEnemyTutorialUnit(bacteria, levelController.tutorialEnemySpawners[0]);

                tutorialNeutrophil.TutorialArrow.gameObject.SetActive(true);
                tutorialNeutrophil.transform.position = netAllyPosition.position;
                tutorialNeutrophil.DisableDetection();
                tutorialNeutrophil.isMoveOrder = false;

                tutorialBacteriaOne.transform.position = netEnemyPosition.position;
                tutorialBacteriaOne.DisableDetection();
                tutorialBacteriaOne.isMoveOrder = false;

                unitSelectionController.canSelect = true;
                tutorialText.text = "Klik unit Neutrofil dalam medan";
                break;
            case 2:
                unitStatsController.TurnTutorialArrow(AbilityId.TRAP);
                tutorialText.text = "Klik tombol NET dalam panel Kemampuan di kiri bawah layar";
                unitStatsController.BlockAbility(true);
                break;
            case 3:
                ResetArrows();
                waitTimer = 1.5f;
                isCheckTime = true;
                tutorialText.text = "Mohon Menunggu";
                barrier.gameObject.SetActive(false);

                unitSelectionController.canSelect = false;
                break;
        }

        isProcessRunning = true;
    }
    private void CheckMacroPhase()
    {
        isPhaseDone = false;
        if (isUnitSpawn && isTimerDone && allAllyUnits.Count > 0 && subPhaseStep == 4 && isUnitKilled)
        {
            isPhaseDone = true;
            return;
        }

        switch (subPhaseStep)
        {
            case 1:
                if (unitPanelController.selectedCard != null)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
            case 2:
                if (isUnitSpawn && allAllyUnits.Count > 0)
                {
                    subPhaseStep = 3;
                    isProcessRunning = false;
                }
                break;
            case 3:
                if (isUnitKilled)
                {
                    subPhaseStep = 4;
                    isProcessRunning = false;
                }
                break;
        }
    } //
    private void DeployMacroPhase()
    {
        HideUI();
        unitPanelController.gameObject.SetActive(true);
        switch (subPhaseStep)
        {
            case 1:
                KillAllUnit();
                tutorialText.text = "Klik kartu unit Makrofag di panel kartu unit";

                AllyUnit macrophage = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.MACROPHAGE).FirstOrDefault();

                List<AllyUnit> unitList = new List<AllyUnit>();
                unitList.Add(macrophage);

                unitPanelController.ResetSelectedCard();
                unitPanelController.ResetUnitCards();
                unitPanelController.InitializeUnitCards(unitList);
                unitPanelController.TurnTutorialArrow(macrophage.unitId);
                break;
            case 2:
                ResetArrows();
                tutorialText.text = "Pilih tempat “Spawner” untuk unit Makrofag Anda";
                firstSpawnArrow.gameObject.SetActive(true);
                secondSpawnArrow.gameObject.SetActive(true);
                break;
            case 3:
                ResetArrows();

                deployableUnit = allAllyUnits[0];
                MacrophageController macrophageController = (MacrophageController)deployableUnit;

                int spawnIndex = 0;
                if (macrophageController.GetRouteTakenId() == 2)
                {
                    spawnIndex = 0;
                }
                else if (macrophageController.GetRouteTakenId() == 3)
                {
                    spawnIndex = 1;
                }

                EnemyUnit bacteria = (EnemyUnit)UnitReferences.Where(x => x.unitId == UnitId.BACTERIA).FirstOrDefault();
                DeployEnemyTutorialUnit(bacteria, levelController.tutorialEnemySpawners[spawnIndex]);

                tutorialText.text = "Makrofag akan menyerang patogen dalam jarak dekat";
                break;
            case 4:
                waitTimer = 1.5f;
                isCheckTime = true;
                tutorialText.text = "Mohon Menunggu";
                break;
        }

        isProcessRunning = true;
    } //
    private void CheckAntigenPhase()
    {
        isPhaseDone = false;
        if (isCollecting && isAdaptiveUnlocked && isTimerDone)
        {
            isPhaseDone = true;
            return;
        }
        switch (subPhaseStep)
        {
            case 1:
                if (selectedUnit != null)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
            case 2:
                if (isCollecting)
                {
                    subPhaseStep = 3;
                    isProcessRunning = false;
                }
                break;
            case 3:
                if (isAdaptiveUnlocked)
                {
                    subPhaseStep = 4;
                    isProcessRunning = false;
                }
                break;
        }
    }
    private void CollectAntigenPhase()
    {
        HideUI();
        unitStatsController.gameObject.SetActive(true);
        switch (subPhaseStep)
        {
            case 1:
                ClearAntigen();
                ResetArrows();
                KillAllUnit();
                unitStatsController.HideBlock();

                AllyUnit macrophage = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.MACROPHAGE).FirstOrDefault();
                DeployAllyTutorialUnit(macrophage, levelController.tutorialAllySpawners[0]);
                tutorialMacrophage.isMoveOrder = false;

                EnemyUnit bacteria = (EnemyUnit)UnitReferences.Where(x => x.unitId == UnitId.BACTERIA).FirstOrDefault();
                DeployEnemyTutorialUnit(bacteria, levelController.tutorialEnemySpawners[0]);
                tutorialBacteriaOne.isMoveOrder = false;

                tutorialText.text = "Klik unit Makrofag dalam medan";
                unitSelectionController.canSelect = true;
                break;
            case 2:
                unitStatsController.TurnTutorialArrow(AbilityId.APC);
                tutorialText.text = "Klik tombol Koleksi dalam panel Kemampuan di kiri bawah layar";
                unitStatsController.BlockAbility(true);
                break;
            case 3:
                tutorialMacrophage.isMoveOrder = true;
                tutorialBacteriaOne.isMoveOrder = true;
                unitStatsController.InitializeUnitStats(selectedUnit, selectedUnit.UnitSO.UnitAbility);
                tutorialText.text = "Makrofag akan membawa antigen ke markas dari patogen yang telah mati";
                unitStatsController.BlockAbility(false);
                break;
            case 4:
                waitTimer = 3f;
                isCheckTime = true;
                tutorialText.text = "Anda telah menambahkan Intel Points dan memenuhi syarat Sistem Imun Adaptif";
                unitSelectionController.canSelect = false;
                intelPointArrow.gameObject.SetActive(true);
                break;
        }

        isProcessRunning = true;
    }
    private void CheckPlasmaPhase()
    {
        isPhaseDone = false;
        if (isUnitSpawn && isTimerDone && allAllyUnits.Count > 0 && subPhaseStep == 3)
        {
            isPhaseDone = true;
            return;
        }

        switch (subPhaseStep)
        {
            case 1:
                if (unitPanelController.selectedCard != null)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
            case 2:
                if (isUnitSpawn && allAllyUnits.Count > 0)
                {
                    subPhaseStep = 3;
                    isProcessRunning = false;
                }
                break;
        }
    } //
    private void DeployPlasmaPhase()
    {
        HideUI();
        unitPanelController.gameObject.SetActive(true);
        switch (subPhaseStep)
        {
            case 1:
                unitStatsController.HideBlock();
                tutorialText.text = "Klik kartu unit Sel Plasma di panel kartu unit";

                AllyUnit plasma = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.PLASMACELL).FirstOrDefault();

                List<AllyUnit> unitList = new List<AllyUnit>();
                unitList.Add(plasma);

                unitPanelController.ResetSelectedCard();
                unitPanelController.ResetUnitCards();
                unitPanelController.InitializeUnitCards(unitList);
                unitPanelController.TurnTutorialArrow(plasma.unitId);

                unitPanelController.UnlockAdaptiveCard();
                break;
            case 2:
                ResetArrows();
                tutorialText.text = "Pilih tempat “Spawner” untuk unit Sel Plasma Anda";
                firstSpawnArrow.gameObject.SetActive(true);
                secondSpawnArrow.gameObject.SetActive(true);
                break;
            case 3:
                ResetArrows();
                waitTimer = 1.5f;
                isCheckTime = true;
                tutorialText.text = "Mohon Menunggu";
                break;
        }

        isProcessRunning = true;
    } //
    private void CheckObjectDefPhase()
    {
        isPhaseDone = false;
        if (isTimerDone && isUnitKilled)
        {
            isPhaseDone = true;
            return;
        }
        switch (subPhaseStep)
        {
            case 1:
                if (isUnitKilled)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
        }
    }
    private void ObjectDefPhase()
    {
        HideUI();
        switch (subPhaseStep)
        {
            case 1:
                KillAllUnit();
                EnemyUnit bacteria = (EnemyUnit)UnitReferences.Where(x => x.unitId == UnitId.BACTERIA).FirstOrDefault();
                DeployEnemyTutorialUnit(bacteria, levelController.tutorialEnemySpawners[0]);

                allyBaseArrow.transform.gameObject.SetActive(true);
                tutorialText.text = "Mohon Menunggu";
                break;
            case 2:
                ResetArrows();
                allyHPArrow.transform.gameObject.SetActive(true);
                waitTimer = 2f;
                isCheckTime = true;
                tutorialText.text = "HP Markas anda berkurang";
                break;
        }

        isProcessRunning = true;
    }
    private void CheckObjectAtkPhase()
    {
        isPhaseDone = false;
        if (currRepairPoint > 0 && isTimerDone)
        {
            isPhaseDone = true;
            return;
        }
        switch (subPhaseStep)
        {
            case 1:
                if (currRepairPoint > 0)
                {
                    subPhaseStep = 2;
                    isProcessRunning = false;
                }
                break;
        }
    }
    private void ObjectAtkPhase()
    {
        HideUI();
        switch (subPhaseStep)
        {
            case 1:
                KillAllUnit();
                AllyUnit macrophage = (AllyUnit)UnitReferences.Where(x => x.unitId == UnitId.MACROPHAGE).FirstOrDefault();
                DeployAllyTutorialUnit(macrophage, levelController.tutorialAllySpawners[0]);

                enemyBaseArrow.transform.gameObject.SetActive(true);
                tutorialText.text = "Mohon Menunggu";
                break;
            case 2:
                ResetArrows();
                enemyHPArrow.gameObject.SetActive(true);
                waitTimer = 3f;
                isCheckTime = true;
                tutorialText.text = "Anda Meningkatkan Repair Points!";
                break;
        }

        isProcessRunning = true;
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
        unitKilled.ThisUnitKilled -= OnUnitKilled;
        if (phase == TutorialPhase.BASICATTACK || phase == TutorialPhase.NETABILITY || phase == TutorialPhase.DEPLOYMAKRO || phase == TutorialPhase.OBJECTIVEDEF)
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

    private void KillAllUnit()
    {
        if (tutorialMacrophage != null)
        {
            tutorialMacrophage.DealDamage(9999);
            tutorialMacrophage = null;
        }
        if (tutorialNeutrophil != null)
        {
            tutorialNeutrophil.DealDamage(9999);
            tutorialNeutrophil = null;
        }
        if (tutorialBacteriaOne != null)
        {
            tutorialBacteriaOne.DealDamage(9999);
            tutorialBacteriaOne = null;
        }
        if (tutorialBacteriaTwo != null)
        {
            tutorialBacteriaTwo.DealDamage(9999);
            tutorialBacteriaTwo = null;
        }

        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            if (allAllyUnits[i] != null)
            {
                allAllyUnits[i].DealDamage(9999);
            }
        }
        for (int i = 0; i < allEnemyUnits.Count; i++)
        {
            if (allEnemyUnits[i] != null)
            {
                allEnemyUnits[i].DealDamage(9999);
            }
        }

        allAllyUnits.Clear();
        allEnemyUnits.Clear();
    }
    private void DeployAllyTutorialUnit(AllyUnit allyData, ISpawner spawner)
    {
        Route initialRoute = levelController.GetRouteByID(spawner.GetInitialRouteId());
        spawner.SpawnAlly(allyData, allyGlobalModifiers, initialRoute, false);
    }
    private void OnAllyTutorialSpawned(AllyUnitController allyUnit)
    {
        allyUnit.ThisUnitKilled += OnAllyKilled;
        allyUnit.UnitArrivedAtWaypoint += SetNewUnitRoute;
        if (allyUnit is IInflammator)
        {
            IInflammator inflammatorUnit = (IInflammator)allyUnit;
            inflammatorUnit.InflammatoryActivated += IncreaseResourcePoint;
        }
        if (allyUnit is IPresenter)
        {
            IPresenter presenterUnit = (IPresenter)allyUnit;
            presenterUnit.AntigenPresented += IncreaseIntelPoint;
        }

        if (allyUnit is MacrophageController)
        {
            tutorialMacrophage = (MacrophageController)allyUnit;
        }
        else if (allyUnit is NeutrophilController)
        {
            tutorialNeutrophil = (NeutrophilController)allyUnit;
        }
        allAllyUnits.Add(allyUnit);
    }
    private void DeployEnemyTutorialUnit(EnemyUnit enemyData, ISpawner spawner)
    {
        Route initialRoute = levelController.GetRouteByID(spawner.GetInitialRouteId());
        spawner.SpawnEnemy(enemyData, enemyGlobalModifiers, initialRoute, false);
    }
    private void OnEnemyTutorialSpawned(EnemyUnitController enemyUnit)
    {
        enemyUnit.ThisUnitKilled += OnEnemyKilled;
        enemyUnit.UnitArrivedAtWaypoint += SetNewUnitRoute;
        allEnemyUnits.Add(enemyUnit);

        if (tutorialBacteriaOne == null)
        {
            tutorialBacteriaOne = enemyUnit;
            tutorialBacteriaOne.ThisUnitKilled += OnUnitKilled;
        }
        else
        {
            tutorialBacteriaTwo = enemyUnit;
            tutorialBacteriaTwo.ThisUnitKilled += OnUnitKilled;
        }
    }
    private void ReturnUnit(DeployedUnitController unit)
    {
        if (unit == tutorialNeutrophil)
        {
            MoveUnit(unit, moveBackNeutrophil);
        }
        else if (unit == tutorialMacrophage)
        {
            MoveUnit(unit, moveBackMacrophage);
        }
    }
    private void MoveUnit(DeployedUnitController unit, Transform transform)
    {
        unit.transform.position = transform.position;
    }

    private void ClearAntigen()
    {
        List<AntigenController> antigens = Object.FindObjectsByType<AntigenController>(FindObjectsSortMode.None).ToList();
        for (int i = 0; i < antigens.Count; i++)
        {
            Destroy(antigens[i].gameObject);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        for (int i = 0; i < levelController.allySpawners.Count; i++)
        {
            levelController.allySpawners[i].AllySpawned -= OnUnitSpawned;
        }
        infoPopup.PopupFinished -= OnInfoPopupFinished;
        for (int i = 0; i < levelController.tutorialAllySpawners.Count; i++)
        {
            levelController.tutorialAllySpawners[i].AllySpawned -= OnAllyTutorialSpawned;
        }
        for (int i = 0; i < levelController.tutorialEnemySpawners.Count; i++)
        {
            levelController.tutorialEnemySpawners[i].EnemySpawned -= OnEnemyTutorialSpawned;
        }
        barrier.UnitHitBarrier -= ReturnUnit;
    }
}