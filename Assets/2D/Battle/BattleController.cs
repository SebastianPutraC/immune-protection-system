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

    private bool adaptiveUnlocked;

    [SerializeField] private List<RowController> rowsController;
    [SerializeField] private UnitPanelController unitPanelController;
    [SerializeField] private BattlePanelController battlePanelController;
    [SerializeField] private UnitStatsController unitStatsController;
    [SerializeField] private UnitSelectionController unitSelectionController;
    [SerializeField] private PausePanelController pausePanelController;
    [SerializeField] private GameOverPanelController gameOverPanelController;

    public List<UnitData> UnitReferences = new List<UnitData>();
    public List<AllyUnitController> allAllyUnits = new List<AllyUnitController>(); // Public for visual
    public AllyUnitController selectedUnit; //Public for visual

    [SerializeField] private AllyBaseController allyBaseController;
    [SerializeField] private EnemyBaseController enemyBaseController;

    [SerializeField] private Button StopAllButton;
    [SerializeField] private Button AdvanceAllButton;
    [SerializeField] private Button pauseButton;

    private List<StatModifier> allyGlobalModifiers = new List<StatModifier>();
    private List<StatModifier> enemyGlobalModifiers = new List<StatModifier>();

    private void Awake()
    {
        for (int i = 0; i < rowsController.Count; i++)
        {
            rowsController[i].RowClicked += CheckAllyCardSelected;
            rowsController[i].AllySpawned += OnAllySpawned;
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

        StopAllButton.onClick.AddListener(StopAllUnit);
        AdvanceAllButton.onClick.AddListener(AdvanceAllUnit);
        pauseButton.onClick.AddListener(PauseGame);
    }
    private void Start()
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

    private void Update()
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

    private void SetUpLevel(LevelInfo levelInfo)
    {
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

    private void IncreaseResourcePoint(int income)
    {
        resourcePoints += income;
        if (resourcePoints > maxResourcePoints)
        {
            resourcePoints = maxResourcePoints;
        }

        UpdateResourcePointUI();
    }
    private void IncreaseIntelPoint(int intelPoints)
    {
        this.intelPoints += intelPoints;
        if (intelPoints >= unlockIntelPoints)
        {
            intelPoints = unlockIntelPoints;
            UnlockAdaptiveCells();
        }

        battlePanelController.UpdateIntelPointUI(intelPoints, unlockIntelPoints);
    }
    private void UnlockAdaptiveCells()
    {
        if (!adaptiveUnlocked)
        {
            unitPanelController.UnlockAdaptiveCard();
            adaptiveUnlocked = true;
        }
    }

    private void CheckAllyCardSelected(RowController spawnRow)
    {
        if (unitPanelController.selectedCard != null)
        {
            DeployAllyUnit(unitPanelController.DeployCard(), spawnRow);
            unitPanelController.ResetSelectedCard();
        }
    }
    private void DeployAllyUnit(AllyUnit allyData, RowController spawnRow)
    {
        if (allyData.UnitCost > resourcePoints)
            return;

        resourcePoints -= allyData.UnitCost;
        UpdateResourcePointUI();
        spawnRow.SpawnAlly(allyData, allyGlobalModifiers);
    }
    private void OnAllySpawned(AllyUnitController allyUnit)
    {
        allyUnit.ThisUnitKilled += OnAllyKilled;
        allAllyUnits.Add(allyUnit);
    }
    private void OnAllyKilled(UnitController allyUnit)
    {
        if (allyUnit == selectedUnit)
        {
            unitStatsController.ClearUI();
        }
        allyUnit.ThisUnitKilled -= OnAllyKilled;
        allAllyUnits.Remove((AllyUnitController)allyUnit);
    }
    
    private void DeployEnemyUnit(EnemyUnit enemyData, RowController spawnRow)
    {
        spawnRow.SpawnEnemy(enemyData, enemyGlobalModifiers);
    }


    private void StopAllUnit()
    {
        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            allAllyUnits[i].isMoveOrder = false;
        }
    }
    private void AdvanceAllUnit()
    {
        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            allAllyUnits[i].isMoveOrder = true;
        }
    }
    private void OnUnitSelected(AllyUnitController allyUnitSelected)
    {
        if (selectedUnit != null)
        {
            selectedUnit.UnitDeselect();
        }

        selectedUnit = allyUnitSelected;
        selectedUnit.UnitSelect();

        unitStatsController.InitializeUnitStats(allyUnitSelected, allyUnitSelected.UnitSO.abilityType);
    }
    private void OnAbilitySelected(AbilityType ability)
    {
        selectedUnit.ProcessAbility(ability);
    }

    private void PauseGame()
    {
        pausePanelController.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    private void ResumeGame()
    {
        pausePanelController.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    private void GameLost()
    {
        LevelDataSingleton.Instance.isWon = false;
        gameOverPanelController.gameObject.SetActive(true);
        gameOverPanelController.GameLost();
        Time.timeScale = 0;
    }
    private void GameWon()
    {
        LevelDataSingleton.Instance.isWon = true;
        gameOverPanelController.gameObject.SetActive(true);
        gameOverPanelController.GameWon();
        Time.timeScale = 0;
    }
    private void StartNextLevel(LevelInfo levelSelected)
    {
        LevelDataSingleton.Instance.SetCurrentLevelInfo(levelSelected);
        SceneManager.LoadScene(1);
    }
    private void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    // To be easily subscribed to event and called to UI
    // If too complex, combine to one reset UI
    private void UpdateAllyBaseUI(float allyBaseHp)
    {
        battlePanelController.UpdateAllyBaseUI(allyBaseHp);
    }
    private void UpdateEnemyBaseUI(float enemyBaseHp)
    {
        battlePanelController.UpdateEnemyBaseUI(enemyBaseHp);
    }
    private void UpdateResourcePointUI()
    {
        battlePanelController.UpdateResourcePointUI(resourcePoints);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < rowsController.Count; i++)
        {
            rowsController[i].RowClicked -= CheckAllyCardSelected;
            rowsController[i].AllySpawned -= OnAllySpawned;
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

        StopAllButton.onClick.RemoveListener(StopAllUnit);
        AdvanceAllButton.onClick.RemoveListener(AdvanceAllUnit);
        pauseButton.onClick.RemoveListener(PauseGame);

        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            allAllyUnits[i].ThisUnitKilled -= OnAllyKilled;
        }
    }


    private bool isSpawned;
    [SerializeField] private List<EnemyUnit> testUnits;
    private IEnumerator RandomSpawn() // Basic ahh ai spawner
    {
        isSpawned = true;
        yield return new WaitForSeconds(3f);

        int randomRow = Random.Range(0, 3);
        int randomUnit = Random.Range(0, testUnits.Count);
        EnemyUnit spawnEnemy = testUnits[randomUnit];

        DeployEnemyUnit(spawnEnemy, rowsController[randomRow]);
        isSpawned = false;
    }
}
