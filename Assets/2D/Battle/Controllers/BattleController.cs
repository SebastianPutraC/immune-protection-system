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
    public float RpIncomeTimer = 3f;

    protected int intelPoints = 0;
    protected int unlockIntelPoints = 100;

    protected int currBaseHp;
    protected int maxBaseHp;
    protected int currRepairPoint = 0;
    protected int maxRepairPoint;

    protected bool isAdaptiveUnlocked;

    [SerializeField] protected LevelController levelController;
    [SerializeField] protected UnitPanelController unitPanelController;
    [SerializeField] protected BattlePanelController battlePanelController;
    [SerializeField] protected UnitStatsController unitStatsController;
    [SerializeField] protected UnitSelectionController unitSelectionController;
    [SerializeField] protected PausePanelController pausePanelController;
    [SerializeField] protected GameOverPanelController gameOverPanelController;
    [SerializeField] protected BattleCameraController battleCameraController;
    [SerializeField] protected EnemyAiManager enemyAI;

    [SerializeField] protected AudioSource battleMusicAudioSource;
    [SerializeField] protected AudioClip winningMusic;
    [SerializeField] protected AudioClip losingMusic;

    [SerializeField] protected AudioSource buttonAudioSource;

    [SerializeField] protected AudioSource gainAudioSource;
    [SerializeField] protected AudioClip intelGain;

    [SerializeField] protected InformationPopup infoPopup;

    public List<UnitData> UnitReferences = new List<UnitData>();
    public List<AllyUnitController> allAllyUnits = new List<AllyUnitController>(); // Public for visual
    public List<EnemyUnitController> allEnemyUnits = new List<EnemyUnitController>();
    public AllyUnitController selectedUnit; //Public for visual

    [SerializeField] protected Button StopAllButton;
    [SerializeField] protected Button AdvanceAllButton;
    [SerializeField] protected Button pauseButton;

    protected List<StatModifier> allyGlobalModifiers = new List<StatModifier>();
    protected List<StatModifier> enemyGlobalModifiers = new List<StatModifier>();

    protected void Awake()
    {
        unitSelectionController.UnitSelected += OnUnitSelected;
        unitStatsController.AbilityButtonSelected += OnAbilitySelected;
        pausePanelController.ResumePressed += ResumeGame;
        pausePanelController.QuitPressed += ReturnToMenu;
        gameOverPanelController.QuitPressed += ReturnToMenu;
        gameOverPanelController.NextLevelPressed += StartNextLevel;
        infoPopup.PopupFinished += ResumeGame;

        StopAllButton.onClick.AddListener(StopAllUnit);
        AdvanceAllButton.onClick.AddListener(AdvanceAllUnit);
        pauseButton.onClick.AddListener(PauseGame);
    }
    protected virtual void Start()
    {
        battleMusicAudioSource.Play();
        //Get Level Info
        LevelInfo levelInfo = LevelDataSingleton.Instance.GetCurrentLevelInfo();
        allyGlobalModifiers = LevelDataSingleton.Instance.GetModifiersData(UnitSide.ALLY);
        enemyGlobalModifiers = LevelDataSingleton.Instance.GetModifiersData(UnitSide.ENEMY);
        if (levelInfo != null)
        {
            SetUpLevel(levelInfo);
        }

        if (levelInfo.IntroInfos.Count > 0)
        {
            Time.timeScale = 0;
            infoPopup.InitializePopup(levelInfo.IntroInfos, levelInfo.skipIntroIndex);
        }
        else
        {
            ResumeGame();
            infoPopup.ExitPopup();
        }

        if (levelController == null)
        {
            levelController = Instantiate(levelInfo.levelPrefab);
        }
        
        for (int i = 0; i < levelController.allyBaseControllers.Count; i++)
        {
            levelController.allyBaseControllers[i].InitializeBase(UnitSide.ALLY);
        }
        for (int i = 0; i < levelController.enemyBaseControllers.Count; i++)
        {
            levelController.enemyBaseControllers[i].InitializeBase(UnitSide.ENEMY);
        }

        for (int i = 0; i < levelController.allySpawners.Count; i++)
        {
            levelController.allySpawners[i].spawnClicked += CheckAllyCardSelected;
            levelController.allySpawners[i].AllySpawned += OnAllySpawned;
        }
        List<EnemySpawnController> enemySpawner = new List<EnemySpawnController>();
        for (int i = 0; i < levelController.enemySpawners.Count; i++)
        {
            levelController.enemySpawners[i].EnemySpawned += OnEnemySpawned;
            EnemySpawnController newEnemySpawn = levelController.enemySpawners[i].transform.GetChild(0).GetComponent<EnemySpawnController>();
            newEnemySpawn.InitializeSpawn(i);
            enemySpawner.Add(newEnemySpawn);
        }
        for (int i = 0; i < levelController.cellSpawner.Count; i++)
        {
            levelController.cellSpawner[i].CellSpawning += DeployEnemyUnit;
            levelController.cellSpawner[i].EnemySpawned += OnEnemySpawned;
            levelController.cellSpawner[i].ThisUnitKilled += OnCellKilled;
        }
        for (int i = 0; i < levelController.allyBaseControllers.Count; i++)
        {
            levelController.allyBaseControllers[i].baseAttacked += BaseAttacked;
        }
        for (int i = 0; i < levelController.enemyBaseControllers.Count; i++)
        {
            levelController.enemyBaseControllers[i].baseAttacked += BaseAttacked;
        }

        UnitData bossUnit = UnitReferences.Find(unit => unit.unitId == levelInfo.bossUnit);
        UnitData boomUnit = UnitReferences.Find(unit => unit.unitId == levelInfo.boomUnit);
        List<UnitData> randomUnits = new List<UnitData>();

        for (int i = 0; i < levelInfo.randomUnit.Count; i++)
        {
            UnitId enemyId = levelInfo.randomUnit[i];
            UnitData randomUnit = UnitReferences.Find(unit => unit.unitId == enemyId);
            randomUnits.Add(randomUnit);
        }
        enemyAI.InitializeAI(levelInfo.enemyStartingResource, levelInfo.enemyIncome, bossUnit, boomUnit, randomUnits, enemySpawner, true);
        enemyAI.EnemySpawn += DeployEnemyUnit;

        battleCameraController.InitializeBattleCamera(levelController.upLimit, levelController.leftLimit, 
            levelController.rightLimit, levelController.downLimit, levelController.startingCameraPosition);
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
            enemyAI.IncreaseResourcePoint();
            RpIncomeTimer = 2f;
        }

        /*if (!isSpawned)
        {
            StartCoroutine(RandomSpawn());
        }*/

        if (selectedUnit != null)
        {
            unitStatsController.UpdateUnitStats(selectedUnit);
        }

        ProcessInput();
    }

    protected virtual void ProcessInput()
    {
        if (selectedUnit != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                selectedUnit.isMoveOrder = false;
                if (selectedUnit != null)
                {
                    unitStatsController.InitializeUnitStats(selectedUnit, selectedUnit.UnitSO.UnitAbility);
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                selectedUnit.isMoveOrder = true;
                if (selectedUnit != null)
                {
                    unitStatsController.InitializeUnitStats(selectedUnit, selectedUnit.UnitSO.UnitAbility);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StopAllUnit();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AdvanceAllUnit();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        if (unitPanelController.selectedCard != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                unitPanelController.ResetSelectedCard();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (levelController.allySpawners[0] != null)
                {
                    CheckAllyCardSelected(levelController.allySpawners[0]);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (levelController.allySpawners[1] != null)
                {
                    CheckAllyCardSelected(levelController.allySpawners[1]);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (levelController.allySpawners[2] != null)
                {
                    CheckAllyCardSelected(levelController.allySpawners[2]);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (levelController.allySpawners[3] != null)
                {
                    CheckAllyCardSelected(levelController.allySpawners[3]);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (levelController.allySpawners[4] != null)
                {
                    CheckAllyCardSelected(levelController.allySpawners[4]);
                }
            }
        }
    }
    protected void SetUpLevel(LevelInfo levelInfo)
    {
        maxBaseHp = levelInfo.allyBaseMaxHP;
        currBaseHp = maxBaseHp;
        maxRepairPoint = levelInfo.maxRepairPoints;
        currRepairPoint = 0;
        unlockIntelPoints = levelInfo.unlockIntelPoint;

        UpdateBaseUI(currBaseHp, maxBaseHp, UnitSide.ALLY);
        UpdateBaseUI(currRepairPoint, maxRepairPoint, UnitSide.ENEMY);
        battlePanelController.UpdateResourcePointUI(resourcePoints, maxResourcePoints);
        battlePanelController.UpdateIntelPointUI(0, unlockIntelPoints);

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
        else
        {
            //gainAudioSource.clip = rpGain;
            //gainAudioSource.Play();
        }

        battlePanelController.UpdateResourcePointUI(resourcePoints, maxResourcePoints);
    }
    protected void IncreaseIntelPoint(int income)
    {
        intelPoints += income;
        if (intelPoints >= unlockIntelPoints)
        {
            intelPoints = unlockIntelPoints;
            UnlockAdaptiveCells();
        }
        else
        {
            gainAudioSource.clip = intelGain;
            gainAudioSource.Play();
        }

        battlePanelController.UpdateIntelPointUI(intelPoints, unlockIntelPoints);
    }
    protected void UnlockAdaptiveCells()
    {
        unitPanelController.UnlockAdaptiveCard();
        isAdaptiveUnlocked = true;
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
        battlePanelController.UpdateResourcePointUI(resourcePoints, maxResourcePoints);

        Route initialRoute = levelController.GetRouteByID(spawner.GetInitialRouteId());

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

    protected void DeployEnemyUnit(EnemyUnit enemyData, int spawnerId)
    {
        ISpawner chosenSpawner = levelController.enemySpawners[spawnerId];
        Route initialRoute = levelController.GetRouteByID(chosenSpawner.GetInitialRouteId());
        chosenSpawner.SpawnEnemy(enemyData, enemyGlobalModifiers, initialRoute);
    }
    protected void DeployEnemyUnit(EnemyUnit enemyData, ISpawner chosenSpawner)
    {
        Route initialRoute = levelController.GetRouteByID(chosenSpawner.GetInitialRouteId());
        chosenSpawner.SpawnEnemy(enemyData, enemyGlobalModifiers, initialRoute);
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
        levelController.cellSpawner.Remove(cellController);
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
    protected void ResetSelected()
    {
        selectedUnit = null;
    }
    protected virtual void OnAbilitySelected(AbilityId ability)
    {
        selectedUnit.ProcessAbility(ability);
        unitStatsController.InitializeUnitStats(selectedUnit, selectedUnit.UnitSO.UnitAbility);
    }
    protected void SetNewUnitRoute(int routeId, DeployedUnitController unit)
    {
        Route newRoute = levelController.GetRouteByID(routeId);
        unit.SetNewRoute(newRoute);
    }


    protected void PauseGame()
    {
        battleMusicAudioSource.Pause();
        pausePanelController.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    protected void ResumeGame()
    {
        battleMusicAudioSource.UnPause();
        pausePanelController.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    protected void GameEnd(UnitSide side)
    {
        if (side == UnitSide.ALLY)
        {
            LevelId unlockedLevel = LevelDataSingleton.Instance.GetUnlockedLevel();
            SaveManagerSingleton.Instance.UpdateSelectedData(UnitId.NONE, unlockedLevel);

            List<UnitId> unlockedUnits = LevelDataSingleton.Instance.GetAllUnits();
            for (int i = 0; i < unlockedUnits.Count; i++)
            {
                SaveManagerSingleton.Instance.UpdateSelectedData(unlockedUnits[i], LevelId.NONE);
            }
            if (LevelDataSingleton.Instance.GetCurrentLevelInfo().levelId == LevelId.LEVEL_SEVEN)
            {
                SaveManagerSingleton.Instance.WonAllLevel();
            }

            gameOverPanelController.gameObject.SetActive(true);
            gameOverPanelController.GameWon();

            battleMusicAudioSource.clip = winningMusic;
            battleMusicAudioSource.Play();
        }
        else
        {
            gameOverPanelController.gameObject.SetActive(true);
            gameOverPanelController.GameLost();
            battleMusicAudioSource.clip = losingMusic;
            battleMusicAudioSource.Play();
            Time.timeScale = 0;
        }
       
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

    protected void BaseAttacked(UnitSide side)
    {
        if (side == UnitSide.ALLY)
        {
            currBaseHp--;
            UpdateBaseUI(currBaseHp, maxBaseHp, UnitSide.ALLY);
            if (currBaseHp <= 0)
            {
                GameEnd(UnitSide.ENEMY);
            }
        }
        else
        {
            currRepairPoint++;
            UpdateBaseUI(currRepairPoint, maxRepairPoint, UnitSide.ENEMY);
            if (currRepairPoint >= maxRepairPoint)
            {
                GameEnd(UnitSide.ALLY);
            }
        }
    }
    protected void UpdateBaseUI(int currPoint, int maxPoint, UnitSide side)
    {
        if (side == UnitSide.ALLY)
        {
            battlePanelController.UpdateAllyBaseUI(currPoint, maxPoint);
        }
        else
        {
            battlePanelController.UpdateEnemyBaseUI(currPoint, maxPoint);
        }
    }

    protected virtual void OnDestroy()
    {
        for (int i = 0; i < levelController.allySpawners.Count; i++)
        {
            levelController.allySpawners[i].spawnClicked -= CheckAllyCardSelected;
            levelController.allySpawners[i].AllySpawned -= OnAllySpawned;
        }
        for (int i = 0; i < levelController.enemySpawners.Count; i++)
        {
            levelController.enemySpawners[i].EnemySpawned -= OnEnemySpawned;
        }
        for (int i = 0; i < levelController.cellSpawner.Count; i++)
        {
            levelController.cellSpawner[i].CellSpawning -= DeployEnemyUnit;
            levelController.cellSpawner[i].EnemySpawned -= OnEnemySpawned;
        }
        for (int i = 0; i < levelController.allyBaseControllers.Count; i++)
        {
            levelController.allyBaseControllers[i].baseAttacked -= BaseAttacked;
        }
        for (int i = 0; i < levelController.enemyBaseControllers.Count; i++)
        {
            levelController.enemyBaseControllers[i].baseAttacked -= BaseAttacked;
        }

        unitSelectionController.UnitSelected -= OnUnitSelected;
        unitStatsController.AbilityButtonSelected -= OnAbilitySelected;
        pausePanelController.ResumePressed -= ResumeGame;
        pausePanelController.QuitPressed -= ReturnToMenu;
        gameOverPanelController.QuitPressed -= ReturnToMenu;
        gameOverPanelController.NextLevelPressed -= StartNextLevel;
        infoPopup.PopupFinished -= ResumeGame;

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

        int randomRow = Random.Range(0, levelController.enemySpawners.Count - 1);
        int randomUnit = Random.Range(0, testUnits.Count);
        EnemyUnit spawnEnemy = testUnits[randomUnit];

        DeployEnemyUnit(spawnEnemy, levelController.enemySpawners[randomRow]);
        isSpawned = false;
    }
}
