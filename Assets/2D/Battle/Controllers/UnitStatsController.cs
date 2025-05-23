using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;

public class UnitStatsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitHpText;
    [SerializeField] private TextMeshProUGUI unitAtkText;

    [SerializeField] private AbilityButton abilityButtonPrefab;
    private List<AbilityButton> abilityButtons = new List<AbilityButton>();
    [SerializeField] private Transform abilityPanel;

    [SerializeField] private EncyclopediaAbilitySO stopAbility;

    [SerializeField] private GameObject blockAbility1;
    [SerializeField] private GameObject blockAbility2;

    public Action<AbilityId> AbilityButtonSelected;

    private void Awake()
    {
        ClearUI();
    }

    public void InitializeUnitStats(AllyUnitController allyUnit, List<EncyclopediaAbilitySO> abilities)
    {
        ClearUI();
        List<EncyclopediaAbilitySO> unitAbilities = new List<EncyclopediaAbilitySO>();

        if (abilities != null)
        {
            unitAbilities.AddRange(abilities);
        }

        unitAbilities.Insert(0, stopAbility); //ESSENTIAL ABILITY

        unitNameText.text = allyUnit.UnitSO.UnitName;

        for (int i = 0; i < unitAbilities.Count; i++)
        {
            if (!unitAbilities[i].IsManual)
            {
                continue;
            }
            AbilityButton newButton = Instantiate(abilityButtonPrefab, abilityPanel);

            string abilityString = unitAbilities[i].AbilityPrimaryName;
            bool secondary = false;
            Sprite secondaryImage = null;
            if (unitAbilities[i].AbilitySecondaryImage != null)
            {
                secondaryImage = unitAbilities[i].AbilitySecondaryImage;
            }

            switch (unitAbilities[i].AbilityID)
            {
                case AbilityId.MOVE:
                    if (allyUnit.isMoveOrder)
                    {
                        secondary = true;
                        abilityString = unitAbilities[i].AbilitySecondaryName;
                    }
                    break;
                case AbilityId.APC:
                    if (allyUnit is IPresenter)
                    {
                        IPresenter presenter = (IPresenter)allyUnit;
                        if (presenter.isPresenting)
                        {
                            secondary = true;
                            abilityString = unitAbilities[i].AbilitySecondaryName;
                        }
                    }
                    break;
                default:
                    if (abilityString == "")
                        abilityString = "PLACEHOLDER";
                    break;
            }

            newButton.InitializeButton(unitAbilities[i].AbilityID, abilityString, unitAbilities[i].AbilityPrimaryImage, secondaryImage, secondary);
            newButton.abilityPressed += OnAbilityButtonSelected;
            abilityButtons.Add(newButton);
        }

        UpdateUnitStats(allyUnit);
    }

    private void OnAbilityButtonSelected(AbilityId abilitySelected)
    {
        AbilityButtonSelected?.Invoke(abilitySelected);
    }

    public void UpdateUnitStats(AllyUnitController allyUnit)
    {
        unitHpText.text = "HP : " + allyUnit.unitHp;
        unitAtkText.text = "ATK : " + allyUnit.UnitSO.UnitAtk;
    }

    public void TurnTutorialArrow(AbilityId abilityId)
    {
        AbilityButton ability = abilityButtons.Where(x => x.abilityType == abilityId).FirstOrDefault();
        ability.tutorialArrow.SetActive(true);
    }
    public void TurnOffArrows()
    {
        for (int i = 0; i < abilityButtons.Count; i++)
        {
            abilityButtons[i].tutorialArrow.gameObject.SetActive(false);
        }
    }
    public void BlockAbility(bool left)
    {
        if (left)
        {
            blockAbility1.gameObject.SetActive(true);
        }
        else
        {
            blockAbility2.gameObject.SetActive(true);
        }
    }
    public void HideBlock()
    {
        blockAbility1.gameObject.SetActive(false);
        blockAbility2.gameObject.SetActive(false);
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
