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
    [SerializeField] private Button mechanicPanelButton;

    [SerializeField] private EncylopediaUnitGrid unitGridPrefab;
    [SerializeField] private EncyclopediaMechanicGrid mechanicGridPrefab;

    [SerializeField] private GameObject cellsScrollViewObject;
    [SerializeField] private GameObject pathogenScrollViewObject;
    [SerializeField] private GameObject mechanicScrollViewObject;
    [SerializeField] private Transform cellsContent;
    [SerializeField] private Transform pathogenContent;
    [SerializeField] private Transform mechanicContent;

    [SerializeField] private AbilityMenuController abilityMenuController;
    [SerializeField] private AboutMenuController aboutMenuController;
    [SerializeField] private DescriptionPanel descriptionPanel;
    [SerializeField] private GameObject encyclopediaPanel;

    [SerializeField] private Button returnButton;

    [SerializeField] private List<UnitData> encylopediaReference = new List<UnitData>();
    [SerializeField] private List<MechanicSO> mechanicReference = new List<MechanicSO>();

    private List<EncylopediaUnitGrid> unitGrids = new List<EncylopediaUnitGrid>();
    private EncylopediaUnitGrid selectedUnitGrid;
    private List<EncyclopediaMechanicGrid> mechanicGrids = new List<EncyclopediaMechanicGrid>();
    private EncyclopediaMechanicGrid selectedMechanicGrid;

    public Action<bool> OnReturnButtonPressed;
    public Action ButtonPressed;

    private void Awake()
    {
        returnButton.onClick.AddListener(ReturnButtonPressed);
        cellsPanelButton.onClick.AddListener(delegate { SetupViewUnits(UnitType.CELLS); });
        pathogenPanelButton.onClick.AddListener(delegate { SetupViewUnits(UnitType.PATHOGEN); });
        mechanicPanelButton.onClick.AddListener(SetupViewMechanic);
        descriptionPanel.AbilityPressed += OpenAbilityMenu;
        descriptionPanel.AboutPressed += OpenAboutMenu;
        abilityMenuController.ReturnToEncyclopedia += ReturnToEncyclopedia;
        aboutMenuController.ReturnToEncyclopedia += ReturnToEncyclopedia;
    }

    public void InitializeEncyclopedia(List<EncyclopediaData> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            UnitData newUnit = encylopediaReference.Where(x => x.unitId.Equals((UnitId)units[i].UnitId)).FirstOrDefault();
            EncylopediaUnitGrid newUnitGrid;

            if (newUnit.unitType == UnitType.CELLS)
            {
                newUnitGrid = Instantiate(unitGridPrefab, cellsContent);
            }
            else if (newUnit.unitType == UnitType.PATHOGEN)
            {
                newUnitGrid = Instantiate(unitGridPrefab, pathogenContent);
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

        for (int i = 0; i < mechanicReference.Count; i++)
        {
            EncyclopediaMechanicGrid newMechanicGrid = Instantiate(mechanicGridPrefab, mechanicContent);
            newMechanicGrid.InitializeData(mechanicReference[i]);
            newMechanicGrid.MechanicClicked += OnMechanicClicked;
            mechanicGrids.Add(newMechanicGrid);
        }

        SetupViewUnits(UnitType.CELLS);
    }

    private void SetupViewUnits(UnitType unitType)
    {
        descriptionPanel.ClearDescriptionPanel();
        descriptionPanel.SwitchToUnit();

        if (unitType == UnitType.CELLS)
        {
            cellsScrollViewObject.gameObject.SetActive(true);
            pathogenScrollViewObject.gameObject.SetActive(false);
            mechanicScrollViewObject.gameObject.SetActive(false);
        }
        else if (unitType == UnitType.PATHOGEN)
        {
            cellsScrollViewObject.gameObject.SetActive(false);
            pathogenScrollViewObject.gameObject.SetActive(true);
            mechanicScrollViewObject.gameObject.SetActive(false);
        }

        for (int i = 0; i < unitGrids.Count; i++)
        {
            unitGrids[i].DeselectGrid();
        }
        ButtonPressed?.Invoke();

        AudioManagerSingleton.Instance.PlayButtonSFX();
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
    private void SetupViewMechanic()
    {
        descriptionPanel.ClearDescriptionPanel();
        descriptionPanel.SwitchToMechanic();

        cellsScrollViewObject.gameObject.SetActive(false);
        pathogenScrollViewObject.gameObject.SetActive(false);
        mechanicScrollViewObject.gameObject.SetActive(true);

        for (int i = 0; i < mechanicGrids.Count; i++)
        {
            mechanicGrids[i].DeselectGrid();
        }

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }
    private void ClearViewMechanic()
    {
        for (int i = 0; i < mechanicGrids.Count; i++)
        {
            mechanicGrids[i].MechanicClicked -= OnMechanicClicked;
            Destroy(mechanicGrids[i].gameObject);
        }
        mechanicGrids.Clear();
    }

    private void OnUnitClicked(EncylopediaUnitGrid unitGrid)
    {
        if (selectedUnitGrid != null)
        {
            selectedUnitGrid.DeselectGrid();
        }

        selectedUnitGrid = unitGrid;
        descriptionPanel.SetupUnitDescriptionPanel(selectedUnitGrid.unitSo);
        selectedUnitGrid.SelectGrid();

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }
    private void OnMechanicClicked(EncyclopediaMechanicGrid mechanicGrid)
    {
        if (selectedMechanicGrid != null)
        {
            selectedMechanicGrid.DeselectGrid();
        }

        selectedMechanicGrid = mechanicGrid;
        descriptionPanel.SetupMechanicDescriptionPanel(selectedMechanicGrid.mechanicSo);
        selectedMechanicGrid.SelectGrid();

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }
    

    private void ReturnToEncyclopedia()
    {
        abilityMenuController.ResetButtons();
        encyclopediaPanel.gameObject.SetActive(true);
        abilityMenuController.gameObject.SetActive(false);
        aboutMenuController.gameObject.SetActive(false);

        ButtonPressed?.Invoke();
    }
    private void OpenAbilityMenu()
    {
        if (selectedUnitGrid == null)
        {
            return;
        }
        abilityMenuController.gameObject.SetActive(true);
        abilityMenuController.InitializeMenu(selectedUnitGrid.unitSo);
        aboutMenuController.gameObject.SetActive(false);
        encyclopediaPanel.gameObject.SetActive(false);

        ButtonPressed?.Invoke();

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }
    private void OpenAboutMenu()
    {
        if (selectedUnitGrid == null)
        {
            return;
        }
        aboutMenuController.gameObject.SetActive(true);
        aboutMenuController.InitializeMenu(selectedUnitGrid.unitSo);
        abilityMenuController.gameObject.SetActive(false);
        encyclopediaPanel.gameObject.SetActive(false);

        ButtonPressed?.Invoke();

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }

    private void ReturnButtonPressed()
    {
        OnReturnButtonPressed?.Invoke(true);
    }
    private void OnDisable()
    {
        ClearViewUnits();
        ClearViewMechanic();
    }
    private void OnDestroy()
    {
        ClearViewUnits();
        ClearViewMechanic();
        returnButton.onClick.RemoveListener(ReturnButtonPressed);
        cellsPanelButton.onClick.RemoveListener(delegate { SetupViewUnits(UnitType.CELLS); });
        pathogenPanelButton.onClick.RemoveListener(delegate { SetupViewUnits(UnitType.PATHOGEN); });
        mechanicPanelButton.onClick.RemoveListener(SetupViewMechanic);
        descriptionPanel.AbilityPressed -= OpenAbilityMenu;
        descriptionPanel.AboutPressed -= OpenAboutMenu;
        abilityMenuController.ReturnToEncyclopedia -= ReturnToEncyclopedia;
        aboutMenuController.ReturnToEncyclopedia -= ReturnToEncyclopedia;
    }
}
