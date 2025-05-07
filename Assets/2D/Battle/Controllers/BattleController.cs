using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class BattleController : MonoBehaviour
{
    protected int resourcePoints = 300;
    protected int basicIncome = 50;
    protected int maxResourcePoints = 800;
    protected int intelPoints = 0;
    protected int unlockIntelPoints = 100;

    public float RpIncomeTimer = 3f;

    protected bool isAdaptiveUnlocked;

    [SerializeField] protected List<Spawner> allySpawners;
    [SerializeField] protected List<Spawner> enemySpawners;
    [SerializeField] protected List<BodyCellController> cellSpawner;

    [SerializeField] protected RouteController routeController;
    [SerializeField] protected UnitPanelController unitPanelController;
    [SerializeField] protected BattlePanelController battlePanelController;
    [SerializeField] protected UnitStatsController unitStatsController;
    [SerializeField] protected UnitSelectionController unitSelectionController;
    [SerializeField] protected PausePanelController pausePanelController;
    [SerializeField] protected GameOverPanelController gameOverPanelController;
    [SerializeField] protected BattleCameraController battleCameraController;

    public List<UnitData> UnitReferences = new List<UnitData>();
    public List<AllyUnitController> allAllyUnits = new List<AllyUnitController>(); // Public for visual
    public List<EnemyUnitController> allEnemyUnits = new List<EnemyUnitController>();
    public AllyUnitController selectedUnit; //Public for visual

    [SerializeField] protected AllyBaseController allyBaseController;
    [SerializeField] protected EnemyBaseController enemyBaseController;

    [SerializeField] protected Button StopAllButton;
    [SerializeField] protected Button AdvanceAllButton;
    [SerializeField] protected Button pauseButton;

    protected List<StatModifier> allyGlobalModifiers = new List<StatModifier>();
    protected List<StatModifier> enemyGlobalModifiers = new List<StatModifier>();

    protected virtual void Awake()
    {
        for (int i = 0; i < allySpawners.Count; i++)
        {
            allySpawners[i].spawnClicked += CheckAllyCardSelected;
            allySpawners[i].AllySpawned += OnAllySpawned;
        }
        for (int i = 0; i < enemySpawners.Count; i++)
        {
            enemySpawners[i].EnemySpawned += OnEnemySpawned;
        }
        for (int i = 0; i < cellSpawner.Count; i++)
        {
            cellSpawner[i].CellSpawning += DeployEnemyUnit;
            cellSpawner[i].EnemySpawned += OnEnemySpawned;
            cellSpawner[i].ThisUnitKilled += OnCellKilled;
        }
        allyBaseController.allyBaseAttacked += UpdateAllyBaseUI;
        allyBaseController.allyBaseDestroyed += GameLost;
        enemyBaseController.enemyBaseAttacked += UpdateEnemyBaseUI;
        enemyBaseController.enemyBaseDestroyed += GameWon;

        allyBaseController.InitializeBase(100);
        enemyBaseController.InitializeBase(100);

        unitSelectionController.UnitSelected += OnUnitSelected;
        unitStatsController.AbilityButtonSelected += OnAbilitySelected;
        pausePanelController.ResumePressed += ResumeGame;
        pausePanelController.QuitPressed += ReturnToMenu;
        gameOverPanelController.QuitPressed += ReturnToMenu;
        gameOverPanelController.NextLevelPressed += StartNextLevel;

        StopAllButton.onClick.AddListener(StopAllUnit);
        AdvanceAllButton.onClick.AddListener(AdvanceAllUnit);
        pauseButton.onClick.AddListener(PauseGame);
    }
    protected void Start()
    {
        //Get Level Info
        LevelInfo levelInfo = LevelDataSingleton.Instance.GetCurrentLevelInfo();
        allyGlobalModifiers = LevelDataSingleton.Instance.GetModifiersData(UnitSide.ALLY);
        enemyGlobalModifiers = LevelDataSingleton.Instance.GetModifiersData(UnitSide.ENEMY);
        if (levelInfo != null)
        {
            SetUpLevel(levelInfo);
        }

        UpdateAllyBaseUI(allyBaseController.unitHp);
        UpdateEnemyBaseUI(enemyBaseController.unitHp);
        battlePanelController.UpdateResourcePointUI(resourcePoints);
        battlePanelController.UpdateIntelPointUI(0, unlockIntelPoints);

        ResumeGame();
        gameOverPanelController.gameObject.SetActive(false);
    }
    protected virtual void Update()
    {
        if (RpIncomeTimer > 0)
        {
            RpIncomeTimer -= Time.deltaTime;
        }
        else
        {
            IncreaseResourcePoint(basicIncome);
            RpIncomeTimer = 3f;
        }

        if (!isSpawned)
        {
            StartCoroutine(RandomSpawn());
        }


        //Maybe Change
        if (selectedUnit != null)
        {
            unitStatsController.UpdateUnitStats(selectedUnit);
        }
    }
    protected void SetUpLevel(LevelInfo levelInfo)
    {
        resourcePoints = levelInfo.defaultStatingResource;
        basicIncome = levelInfo.defaultIncome;

        List<AllyUnit> alliesData = new List<AllyUnit>();
        for (int i = 0; i < levelInfo.allyUnits.Count; i++)
        {
            AllyUnit allyUnit = (AllyUnit)UnitReferences.Find(unit => unit.unitId == levelInfo.allyUnits[i]);
            alliesData.Add(allyUnit);
        }
        unitPanelController.InitializeUnitCards(alliesData);


        List<EnemyUnit> enemiesData = new List<EnemyUnit>();
        for (int i = 0; i < levelInfo.enemyUnits.Count; i++)
        {
            EnemyUnit enemyUnit = (EnemyUnit)UnitReferences.Find(unit => unit.unitId == levelInfo.enemyUnits[i]);
            enemiesData.Add(enemyUnit);
        }
        testUnits = enemiesData;
    }


    protected void IncreaseResourcePoint(int income)
    {
        resourcePoints += income;
        if (resourcePoints > maxResourcePoints)
        {
            resourcePoints = maxResourcePoints;
        }

        battlePanelController.UpdateResourcePointUI(resourcePoints);
    }
    protected void IncreaseIntelPoint(int income)
    {
        intelPoints += income;
        if (intelPoints >= unlockIntelPoints)
        {
            intelPoints = unlockIntelPoints;
            UnlockAdaptiveCells();
        }

        battlePanelController.UpdateIntelPointUI(intelPoints, unlockIntelPoints);
    }
    protected void UnlockAdaptiveCells()
    {
        if (!isAdaptiveUnlocked)
        {
            unitPanelController.UnlockAdaptiveCard();
            isAdaptiveUnlocked = true;
        }
    }


    protected void CheckAllyCardSelected(Spawner spawner)
    {
        if (unitPanelController.selectedCard != null)
        {
            DeployAllyUnit(unitPanelController.DeployCard(), spawner);
            unitPanelController.ResetSelectedCard();
        }
    }
    protected void DeployAllyUnit(AllyUnit allyData, ISpawner spawner)
    {
        if (allyData.UnitCost > resourcePoints)
            return;

        resourcePoints -= allyData.UnitCost;
        battlePanelController.UpdateResourcePointUI(resourcePoints);

        Route initialRoute = routeController.GetRouteByID(spawner.GetInitialRouteId());

        spawner.SpawnAlly(allyData, allyGlobalModifiers, initialRoute);
    }
    protected void OnAllySpawned(AllyUnitController allyUnit)
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
        allAllyUnits.Add(allyUnit);
    }
    protected void OnAllyKilled(UnitController allyUnit)
    {
        if (allyUnit == selectedUnit)
        {
            unitStatsController.ClearUI();
        }
        allyUnit.ThisUnitKilled -= OnAllyKilled;
        allyUnit.UnitArrivedAtWaypoint -= SetNewUnitRoute;
        if (allyUnit is IInflammator)
        {
            IInflammator inflammatorUnit = (IInflammator)allyUnit;
            inflammatorUnit.InflammatoryActivated -= IncreaseResourcePoint;
        }
        if (allyUnit is IPresenter)
        {
            IPresenter presenterUnit = (IPresenter)allyUnit;
            presenterUnit.AntigenPresented -= IncreaseIntelPoint;
        }
        allAllyUnits.Remove((AllyUnitController)allyUnit);
    }

    protected void DeployEnemyUnit(EnemyUnit enemyData, ISpawner spawner)
    {
        Route initialRoute = routeController.GetRouteByID(spawner.GetInitialRouteId());
        spawner.SpawnEnemy(enemyData, enemyGlobalModifiers, initialRoute);
    }
    protected void OnEnemySpawned(EnemyUnitController enemyUnit)
    {
        enemyUnit.ThisUnitKilled += OnEnemyKilled;
        enemyUnit.UnitArrivedAtWaypoint += SetNewUnitRoute;
        allEnemyUnits.Add(enemyUnit);
    }
    protected void OnEnemyKilled(UnitController enemyUnit)
    {
        enemyUnit.ThisUnitKilled -= OnEnemyKilled;
        enemyUnit.UnitArrivedAtWaypoint -= SetNewUnitRoute;
        allEnemyUnits.Remove((EnemyUnitController)enemyUnit);
    }
    protected void OnCellKilled(UnitController cellUnit)
    {
        BodyCellController cellController = (BodyCellController) cellUnit;
        cellController.ThisUnitKilled -= OnCellKilled;
        cellController.CellSpawning -= DeployEnemyUnit;
        cellController.EnemySpawned -= OnEnemySpawned;
        cellSpawner.Remove(cellController);
    }


    protected virtual void StopAllUnit()
    {
        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            allAllyUnits[i].isMoveOrder = false;
        }
        if (selectedUnit != null)
        {
            unitStatsController.InitializeUnitStats(selectedUnit, selectedUnit.UnitSO.UnitAbility);
        }
    }
    protected virtual void AdvanceAllUnit()
    {
        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            allAllyUnits[i].isMoveOrder = true;
        }
        if (selectedUnit != null)
        {
            unitStatsController.InitializeUnitStats(selectedUnit, selectedUnit.UnitSO.UnitAbility);
        }
    }
    protected void OnUnitSelected(AllyUnitController allyUnitSelected)
    {
        if (selectedUnit != null)
        {
            selectedUnit.UnitDeselect();
        }

        selectedUnit = allyUnitSelected;
        selectedUnit.UnitSelect();

        unitStatsController.InitializeUnitStats(allyUnitSelected, selectedUnit.UnitSO.UnitAbility);
    }
    protected virtual void OnAbilitySelected(AbilityId ability)
    {
        selectedUnit.ProcessAbility(ability);
        unitStatsController.InitializeUnitStats(selectedUnit, selectedUnit.UnitSO.UnitAbility);
    }
    protected void SetNewUnitRoute(int routeId, DeployedUnitController unit)
    {
        Route newRoute = routeController.GetRouteByID(routeId);
        unit.SetNewRoute(newRoute);
    }


    protected void PauseGame()
    {
        pausePanelController.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    protected void ResumeGame()
    {
        pausePanelController.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    protected void GameLost()
    {
        gameOverPanelController.gameObject.SetActive(true);
        gameOverPanelController.GameLost();
        Time.timeScale = 0;
    }
    protected void GameWon()
    {
        LevelId unlockedLevel = LevelDataSingleton.Instance.GetUnlockedLevel();
        SaveManagerSingleton.Instance.UpdateSelectedData(UnitId.NONE, unlockedLevel);

        List<UnitId> unlockedUnits = LevelDataSingleton.Instance.GetAllUnits();
        for (int i = 0; i < unlockedUnits.Count; i++)
        {
            SaveManagerSingleton.Instance.UpdateSelectedData(unlockedUnits[i], LevelId.NONE);
        }

        gameOverPanelController.gameObject.SetActive(true);
        gameOverPanelController.GameWon();
        Time.timeScale = 0;
    }
    protected void StartNextLevel()
    {
        LevelId unlockedLevel = LevelDataSingleton.Instance.GetUnlockedLevel();
        LevelDataSingleton.Instance.SetCurrentLevelInfo(unlockedLevel);
        SceneManager.LoadScene(1);
    }
    protected void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    // To be easily subscribed to event and called to UI
    // If too complex, combine to one reset UI
    protected void UpdateAllyBaseUI(float allyBaseHp)
    {
        battlePanelController.UpdateAllyBaseUI(allyBaseHp);
    }
    protected void UpdateEnemyBaseUI(float enemyBaseHp)
    {
        battlePanelController.UpdateEnemyBaseUI(enemyBaseHp);
    }

    protected virtual void OnDestroy()
    {
        for (int i = 0; i < allySpawners.Count; i++)
        {
            allySpawners[i].spawnClicked -= CheckAllyCardSelected;
            allySpawners[i].AllySpawned -= OnAllySpawned;
        }
        for (int i = 0; i < enemySpawners.Count; i++)
        {
            enemySpawners[i].EnemySpawned -= OnEnemySpawned;
        }
        for (int i = 0; i < cellSpawner.Count; i++)
        {
            cellSpawner[i].CellSpawning -= DeployEnemyUnit;
            cellSpawner[i].EnemySpawned -= OnEnemySpawned;
        }
        allyBaseController.allyBaseAttacked -= UpdateAllyBaseUI;
        allyBaseController.allyBaseDestroyed -= GameLost;
        enemyBaseController.enemyBaseAttacked -= UpdateEnemyBaseUI;
        enemyBaseController.enemyBaseDestroyed -= GameWon;

        unitSelectionController.UnitSelected -= OnUnitSelected;
        unitStatsController.AbilityButtonSelected -= OnAbilitySelected;
        pausePanelController.ResumePressed -= ResumeGame;
        pausePanelController.QuitPressed -= ReturnToMenu;
        gameOverPanelController.QuitPressed -= ReturnToMenu;
        gameOverPanelController.NextLevelPressed -= StartNextLevel;

        StopAllButton.onClick.RemoveListener(StopAllUnit);
        AdvanceAllButton.onClick.RemoveListener(AdvanceAllUnit);
        pauseButton.onClick.RemoveListener(PauseGame);

        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            allAllyUnits[i].ThisUnitKilled -= OnAllyKilled;
            allAllyUnits[i].UnitArrivedAtWaypoint -= SetNewUnitRoute;
            if (allAllyUnits[i] is IInflammator)
            {
                IInflammator inflammatorUnit = (IInflammator)allAllyUnits[i];
                inflammatorUnit.InflammatoryActivated -= IncreaseResourcePoint;
            }
            if (allAllyUnits[i] is IPresenter)
            {
                IPresenter presenterUnit = (IPresenter)allAllyUnits[i];
                presenterUnit.AntigenPresented -= IncreaseIntelPoint;
            }
        }
        for (int i = 0; i < allEnemyUnits.Count; i++)
        {
            allEnemyUnits[i].ThisUnitKilled -= OnEnemyKilled;
            allEnemyUnits[i].UnitArrivedAtWaypoint -= SetNewUnitRoute;
        }
    }

    // Basic ahh AI spawner, Will move to other script
    protected bool isSpawned;
    [SerializeField] protected List<EnemyUnit> testUnits;
    protected float enemyTestspawnTimer = 3f;
    protected IEnumerator RandomSpawn() 
    {
        isSpawned = true;
        yield return new WaitForSeconds(enemyTestspawnTimer);

        int randomRow = Random.Range(0, enemySpawners.Count - 1);
        int randomUnit = Random.Range(0, testUnits.Count);
        EnemyUnit spawnEnemy = testUnits[randomUnit];

        DeployEnemyUnit(spawnEnemy, enemySpawners[randomRow]);
        isSpawned = false;
    }
}
