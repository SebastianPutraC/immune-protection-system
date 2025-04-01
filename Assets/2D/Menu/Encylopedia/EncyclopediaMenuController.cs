using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class EncyclopediaMenuController : MonoBehaviour
{
    [SerializeField] private Button cellsPanelButton;
    [SerializeField] private Button pathogenPanelButton;

    [SerializeField] private EncylopediaUnitGrid gridPrefab;

    [SerializeField] private GameObject cellsScrollViewObject;
    [SerializeField] private GameObject pathogenScrollViewObject;
    [SerializeField] private Transform cellsContent;
    [SerializeField] private Transform pathogenContent;

    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitDescriptionText;

    [SerializeField] private Button returnButton;

    [SerializeField] private List<EncylopediaUnit> encylopediaReference = new List<EncylopediaUnit>();

    private List<EncylopediaUnitGrid> unitGrids = new List<EncylopediaUnitGrid>();
    private EncylopediaUnitGrid selectedGrid;

    public Action OnReturnButtonPressed;

    private void Awake()
    {
        returnButton.onClick.AddListener(ReturnButtonPressed);
        cellsPanelButton.onClick.AddListener(delegate { SetupViewUnits(UnitType.CELLS); });
        pathogenPanelButton.onClick.AddListener(delegate { SetupViewUnits(UnitType.PATHOGEN); });
    }

    public void InitializeUnits(List<EncyclopediaData> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            EncylopediaUnit newUnit = encylopediaReference.Where(x => x.unitId.Equals((UnitId)units[i].UnitId)).FirstOrDefault();
            EncylopediaUnitGrid newUnitGrid;

            if (newUnit.unitType == UnitType.CELLS)
            {
                newUnitGrid = Instantiate(gridPrefab, cellsContent);
            }
            else if (newUnit.unitType == UnitType.PATHOGEN)
            {
                newUnitGrid = Instantiate(gridPrefab, pathogenContent);
            }
            else return;

            if (units[i].Unlocked)
            {
                newUnitGrid.InitializeData(newUnit, true);    
            }
            else
            {
                newUnitGrid.InitializeData(newUnit, false);
            }

            newUnitGrid.UnitClicked += OnUnitClicked;
            unitGrids.Add(newUnitGrid);
        }

        SetupViewUnits(UnitType.CELLS);
    }

    private void SetupViewUnits(UnitType unitType)
    {
        ClearDescriptionPanel();

        if (unitType == UnitType.CELLS)
        {
            cellsScrollViewObject.gameObject.SetActive(true);
            pathogenScrollViewObject.gameObject.SetActive(false);
        }
        else if (unitType == UnitType.PATHOGEN)
        {
            cellsScrollViewObject.gameObject.SetActive(false);
            pathogenScrollViewObject.gameObject.SetActive(true);
        }
    }
    private void ClearViewUnits()
    {
        for (int i = 0; i < unitGrids.Count; i++)
        {
            unitGrids[i].UnitClicked -= OnUnitClicked;
            Destroy(unitGrids[i].gameObject);
        }
        unitGrids.Clear();
    }

    private void OnUnitClicked(EncylopediaUnitGrid unitGrid)
    {
        if (selectedGrid != null)
        {
            selectedGrid.DeselectGrid();
        }

        selectedGrid = unitGrid;
        SetupDescriptionPanel(selectedGrid.unitSo);
        selectedGrid.SelectGrid();
    }
    private void SetupDescriptionPanel(EncylopediaUnit unitData)
    {
        ClearDescriptionPanel();

        unitNameText.text = unitData.unitName;
        unitImage.sprite = unitData.unitSprite;
        unitDescriptionText.text = unitData.unitDescription;
    }
    private void ClearDescriptionPanel()
    {
        unitNameText.text = "";
        unitDescriptionText.text = "";
        unitImage.sprite = null;
    }

    private void ReturnButtonPressed()
    {
        OnReturnButtonPressed?.Invoke();
    }
    private void OnDisable()
    {
        ClearViewUnits();
    }
    private void OnDestroy()
    {
        ClearViewUnits();
        returnButton.onClick.RemoveListener(ReturnButtonPressed);
        cellsPanelButton.onClick.RemoveListener(delegate { SetupViewUnits(UnitType.CELLS); });
        pathogenPanelButton.onClick.RemoveListener(delegate { SetupViewUnits(UnitType.PATHOGEN); });
    }
}
