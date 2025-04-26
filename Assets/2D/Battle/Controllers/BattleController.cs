using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public int resourcePoints = 300;
    public int basicIncome = 50;
    public int maxResourcePoints = 800;
    public int intelPoints = 0;
    public int unlockIntelPoints = 1000;

    public float RpIncomeTimer = 3f;

    protected bool adaptiveUnlocked;

    [SerializeField] protected List<Spawner> allySpawners;
    [SerializeField] protected List<Spawner> enemySpawners;
    [SerializeField] protected RouteController routeController;
    [SerializeField] protected UnitPanelController unitPanelController;
    [SerializeField] protected BattlePanelController battlePanelController;
    [SerializeField] protected UnitStatsController unitStatsController;
    [SerializeField] protected UnitSelectionController unitSelectionController;
    [SerializeField] protected PausePanelController pausePanelController;
    [SerializeField] protected GameOverPanelController gameOverPanelController;

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

    protected void Awake()
    {
        for (int i = 0; i < allySpawners.Count; i++)
        {
            allySpawners[i].spawnClicked += CheckAllyCardSelected;
            allySpawners[i].AllySpawned += OnAllySpawned;
        }
        for (int i = 0; i < enemySpawners.Count; i++)
        {
            enemySpawners[i].enemySpawned += OnEnemySpawned;
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
        UpdateResourcePointUI();

        ResumeGame();
        gameOverPanelController.gameObject.SetActive(false);
    }
    protected void Update()
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

        UpdateResourcePointUI();
    }
    protected void IncreaseIntelPoint(int intelPoints)
    {
        this.intelPoints += intelPoints;
        if (intelPoints >= unlockIntelPoints)
        {
            intelPoints = unlockIntelPoints;
            UnlockAdaptiveCells();
        }

        battlePanelController.UpdateIntelPointUI(intelPoints, unlockIntelPoints);
    }
    protected void UnlockAdaptiveCells()
    {
        if (!adaptiveUnlocked)
        {
            unitPanelController.UnlockAdaptiveCard();
            adaptiveUnlocked = true;
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
    protected void DeployAllyUnit(AllyUnit allyData, Spawner spawner)
    {
        if (allyData.UnitCost > resourcePoints)
            return;

        resourcePoints -= allyData.UnitCost;
        UpdateResourcePointUI();

        Route initialRoute = routeController.GetRouteByID(spawner.initialRouteId);

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
        allAllyUnits.Remove((AllyUnitController)allyUnit);
    }

    protected void DeployEnemyUnit(EnemyUnit enemyData, Spawner spawner)
    {
        Route initialRoute = routeController.GetRouteByID(spawner.initialRouteId);
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


    protected void StopAllUnit()
    {
        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            allAllyUnits[i].isMoveOrder = false;
        }
    }
    protected void AdvanceAllUnit()
    {
        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            allAllyUnits[i].isMoveOrder = true;
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

        unitStatsController.InitializeUnitStats(allyUnitSelected, allyUnitSelected.UnitSO.AbilityType);
    }
    protected void OnAbilitySelected(AbilityType ability)
    {
        selectedUnit.ProcessAbility(ability);
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
    protected void UpdateResourcePointUI()
    {
        battlePanelController.UpdateResourcePointUI(resourcePoints);
    }

    protected void OnDestroy()
    {
        for (int i = 0; i < allySpawners.Count; i++)
        {
            allySpawners[i].spawnClicked -= CheckAllyCardSelected;
            allySpawners[i].AllySpawned -= OnAllySpawned;
        }
        for (int i = 0; i < enemySpawners.Count; i++)
        {
            enemySpawners[i].enemySpawned -= OnEnemySpawned;
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
        }
        for (int i = 0; i < allEnemyUnits.Count; i++)
        {
            allEnemyUnits[i].ThisUnitKilled -= OnEnemyKilled;
            allEnemyUnits[i].UnitArrivedAtWaypoint -= SetNewUnitRoute;
        }
    }


    protected bool isSpawned;
    [SerializeField] protected List<EnemyUnit> testUnits;
    protected IEnumerator RandomSpawn() // Basic ahh ai spawner
    {
        isSpawned = true;
        yield return new WaitForSeconds(3f);

        int randomRow = Random.Range(0, enemySpawners.Count - 1);
        int randomUnit = Random.Range(0, testUnits.Count);
        EnemyUnit spawnEnemy = testUnits[randomUnit];

        DeployEnemyUnit(spawnEnemy, enemySpawners[randomRow]);
        isSpawned = false;
    }
}
