using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public int resourcePoints = 300;
    public int basicIncome;
    public readonly int maxResourcePoints = 800;

    private bool isGainIncome;

    [SerializeField] private List<RowController> rowsController;
    [SerializeField] private UnitPanelController unitPanelController;
    [SerializeField] private BattlePanelController battlePanelController;
    [SerializeField] private UnitStatsController unitStatsController;
    [SerializeField] private UnitSelectionController unitSelectionController;

    public List<AllyUnitController> allAllyUnits = new List<AllyUnitController>(); // Public for visual
    public AllyUnitController selectedUnit; //Public for visual

    [SerializeField] private AllyBaseController allyBaseController;
    [SerializeField] private EnemyBaseController enemyBaseController;

    [SerializeField] private Button StopAllButton;
    [SerializeField] private Button AdvanceAllButton;

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

        StopAllButton.onClick.AddListener(StopAllUnit);
        AdvanceAllButton.onClick.AddListener(AdvanceAllUnit);

        UpdateAllyBaseUI(allyBaseController.unitHp);
        UpdateEnemyBaseUI(enemyBaseController.unitHp);
        UpdateResourcePointUI();
    }

    private void Update()
    {
        if (!isGainIncome)
        {
            StartCoroutine(PassiveIncome());
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

    private IEnumerator PassiveIncome()
    {
        isGainIncome = true;
        yield return new WaitForSeconds(1f);

        resourcePoints += basicIncome;
        if (resourcePoints > maxResourcePoints)
        {
            resourcePoints = maxResourcePoints;
        }
        UpdateResourcePointUI();

        isGainIncome = false;
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
        spawnRow.SpawnAlly(allyData);
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
        spawnRow.SpawnEnemy(enemyData);
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


    private void GameLost() // Stop the game somehow
    {
        Debug.Log("YOU LOST");
    }
    private void GameWon() // Stop the game somehow
    {
        Debug.Log("YOU WON");
    }


    // To be easily subscribed to event and called to UI
    // If too complex, combine to one reset UI
    private void UpdateAllyBaseUI(int allyBaseHp)
    {
        battlePanelController.UpdateAllyBaseUI(allyBaseHp);
    }

    private void UpdateEnemyBaseUI(int enemyBaseHp)
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

        StopAllButton.onClick.RemoveListener(StopAllUnit);
        AdvanceAllButton.onClick.RemoveListener(AdvanceAllUnit);

        for (int i = 0; i < allAllyUnits.Count; i++)
        {
            allAllyUnits[i].ThisUnitKilled -= OnAllyKilled;
        }
    }


    private bool isSpawned;
    [SerializeField] private EnemyUnit testUnit;
    private IEnumerator RandomSpawn() // Basic ahh ai spawner
    {
        isSpawned = true;
        yield return new WaitForSeconds(3f);

        int randomRow = Random.Range(0, 3);
        DeployEnemyUnit(testUnit, rowsController[randomRow]);
        isSpawned = false;
    }
}
