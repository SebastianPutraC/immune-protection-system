using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AboutMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitDescriptionText;

    [SerializeField] private TextMeshProUGUI unitHpText;
    [SerializeField] private TextMeshProUGUI unitAtkText;
    [SerializeField] private TextMeshProUGUI unitRangedAtkText;
    [SerializeField] private TextMeshProUGUI unitCostText;
    [SerializeField] private TextMeshProUGUI unitTypeText;

    [SerializeField] private Button encyclopediaButton;

    public Action ReturnToEncyclopedia;

    private void Awake()
    {
        encyclopediaButton.onClick.AddListener(OnEncyclopediaButtonPressed);
    }
    public void InitializeMenu(UnitData unitData)
    {
        unitNameText.text = unitData.UnitName;
        unitDescriptionText.text = unitData.UnitFullDesc;

        unitHpText.text = "HP : " + unitData.UnitHp;
        unitAtkText.text = "ATK : " + unitData.UnitAtk;
        unitRangedAtkText.text = "Ranged ATK : " + unitData.UnitRangedAtk;
        unitCostText.text = "Cost : " + unitData.UnitCost;
        if (unitData.GetType() == typeof(AllyUnit))
        {
            AllyUnit allyUnit = (AllyUnit)unitData;
            unitTypeText.text = "Type : " +  allyUnit.ImmuneCellType;
        }
        else
        {
            unitTypeText.text = "Type : " + unitData.unitType;
        }
    }

    private void OnEncyclopediaButtonPressed()
    {
        ReturnToEncyclopedia?.Invoke();
    }
    private void OnDestroy()
    {
        encyclopediaButton.onClick.RemoveListener(OnEncyclopediaButtonPressed);
    }
}
