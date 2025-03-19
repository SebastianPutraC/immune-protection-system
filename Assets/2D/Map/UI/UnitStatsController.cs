using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class UnitStatsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitHpText;
    [SerializeField] private TextMeshProUGUI unitAtkText;

    [SerializeField] private AbilityButton abilityButtonPrefab;
    private List<AbilityButton> abilityButtons = new List<AbilityButton>();
    [SerializeField] private Transform abilityPanel;

    private readonly Color stopColor = Color.red;
    private readonly Color netColor = Color.black;
    private readonly Color repairColor = Color.green;

    public Action<AbilityType> AbilityButtonSelected;

    private void Awake()
    {
        ClearUI();
    }

    public void InitializeUnitStats(AllyUnitController allyUnit, List<AbilityType> abilities)
    {
        unitNameText.text = allyUnit.UnitSO.UnitName;

        ClearUI();


        for (int i = 0; i < abilities.Count; i++)
        {
            AbilityButton newButton = Instantiate(abilityButtonPrefab, abilityPanel);

            Color buttonColor = Color.white;
            string abilityString = "PLACEHOLDER ";
            switch(abilities[i])
            {
                case AbilityType.STOP:
                    buttonColor = stopColor;
                    abilityString = "STOP";
                    break;
                case AbilityType.NET:
                    buttonColor = netColor;
                    abilityString = "NET";
                    break;
                case AbilityType.REPAIR:
                    buttonColor = repairColor;
                    abilityString = "REPAIR";
                    break;
                default:
                    buttonColor = Color.white;
                    abilityString = "PLACEHOLDER";
                    break;
            }

            newButton.InitializeButton(abilities[i], buttonColor, abilityString);
            newButton.abilityPressed += OnAbilityButtonSelected;
            abilityButtons.Add(newButton);
        }

        UpdateUnitStats(allyUnit);
    }

    private void OnAbilityButtonSelected(AbilityType abilitySelected)
    {
        AbilityButtonSelected?.Invoke(abilitySelected);
    }

    public void UpdateUnitStats(AllyUnitController allyUnit)
    {
        unitHpText.text = "HP : " + allyUnit.unitHp;
        unitAtkText.text = "ATK : " + allyUnit.UnitSO.UnitAtk;
    }

    public void ClearUI()
    {
        ClearButtons();
        ClearUnitStats();
    }
    private void ClearButtons()
    {
        for (int i = 0; i < abilityButtons.Count; i++) // Clearing
        {
            abilityButtons[i].abilityPressed -= OnAbilityButtonSelected;
            Destroy(abilityButtons[i].gameObject);
        }
        abilityButtons.Clear();
    }
    public void ClearUnitStats()
    {
        unitNameText.text = "";
        unitHpText.text = "";
        unitAtkText.text = "";
    }

    private void OnDestroy()
    {
        ClearUI();
    }
}
