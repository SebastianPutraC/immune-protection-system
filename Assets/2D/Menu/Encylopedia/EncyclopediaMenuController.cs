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
    [SerializeField] private Transform viewContent;

    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitDescriptionText;

    [SerializeField] private Button returnButton;

    private List<EncylopediaUnitGrid> unitGrids = new List<EncylopediaUnitGrid>();
    private List<EncylopediaUnit> unlockedUnits = new List<EncylopediaUnit>();
    private EncylopediaUnitGrid selectedGrid;

    public Action OnReturnButtonPressed;

    private void Awake()
    {
        returnButton.onClick.AddListener(ReturnButtonPressed);
        cellsPanelButton.onClick.AddListener(delegate { SetupViewUnits(UnitType.CELLS); });
        pathogenPanelButton.onClick.AddListener(delegate { SetupViewUnits(UnitType.PATHOGEN); });
    }

    public void InitializeUnits(List<EncylopediaUnit> units)
    {
        unlockedUnits = units;
        SetupViewUnits(UnitType.CELLS);
    }

    private void SetupViewUnits(UnitType unitType) //Might change this to better way besides Initializing and Destroying gameoObject
    {
        ClearViewUnits();

        List<EncylopediaUnit> panelUnits = unlockedUnits.Where(unit => unit.unitType == unitType).ToList();
        for (int i = 0; i < panelUnits.Count; i++)
        {
            EncylopediaUnitGrid newUnitGrid = Instantiate(gridPrefab, viewContent);
            newUnitGrid.InitializeData(panelUnits[i]);
            newUnitGrid.UnitClicked += OnUnitClicked;
            unitGrids.Add(newUnitGrid);
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
        SetupDescription(selectedGrid.unitSo);
        selectedGrid.SelectGrid();
    }
    private void SetupDescription(EncylopediaUnit unitData)
    {
        unitNameText.text = "";
        unitDescriptionText.text = "";

        unitNameText.text = unitData.unitName;
        unitImage.sprite = unitData.unitSprite;
        unitDescriptionText.text = unitData.unitDescription;
    }

    private void ReturnButtonPressed()
    {
        OnReturnButtonPressed?.Invoke();
    }
    private void OnDestroy()
    {
        returnButton.onClick.RemoveListener(ReturnButtonPressed);
        cellsPanelButton.onClick.RemoveListener(delegate { SetupViewUnits(UnitType.CELLS); });
        pathogenPanelButton.onClick.RemoveListener(delegate { SetupViewUnits(UnitType.PATHOGEN); });
    }
}
