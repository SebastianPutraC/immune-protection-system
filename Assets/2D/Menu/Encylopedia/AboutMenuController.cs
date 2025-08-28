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
    [SerializeField] private TextMeshProUGUI unitAtkCooldown;
    [SerializeField] private TextMeshProUGUI unitRangedAtkText;
    [SerializeField] private TextMeshProUGUI unitRangedCooldown;
    [SerializeField] private TextMeshProUGUI unitSpeed;
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
        unitAtkText.text = "Melee Damage : " + unitData.UnitAtk;
        unitAtkCooldown.text = "Melee Cooldown : " + unitData.UnitAtkSpeed + "s";
        unitRangedAtkText.text = "Ranged Damage : " + unitData.UnitRangedAtk;
        unitRangedCooldown.text = "Ranged Cooldown : " + unitData.UnitRangedAtkSpeed + "s";
        unitSpeed.text = "Kecepatan : " + unitData.UnitSpeed;
        unitCostText.text = "Harga : " + unitData.UnitCost;
        if (unitData.GetType() == typeof(AllyUnit))
        {
            AllyUnit allyUnit = (AllyUnit)unitData;
            if (allyUnit.ImmuneCellType == ImmuneType.INNATE)
            {
                unitTypeText.text = "Tipe : Bawaan";
            }
            else if (allyUnit.ImmuneCellType == ImmuneType.ADAPTIVE)
            {
                unitTypeText.text = "Tipe : Adaptif";
            }
        }
        else
        {
            unitTypeText.text = "Tipe : " + unitData.unitType;
        }
    }

    private void OnEncyclopediaButtonPressed()
    {
        ReturnToEncyclopedia?.Invoke();

        AudioManagerSingleton.Instance.PlayButtonSFX();
    }
    private void OnDestroy()
    {
        encyclopediaButton.onClick.RemoveListener(OnEncyclopediaButtonPressed);
    }
}
