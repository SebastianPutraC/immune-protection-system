using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class AbilityMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Transform abilityButtonContent;
    [SerializeField] private Image abilityImage;
    [SerializeField] private TextMeshProUGUI abilityDescriptionText;

    [SerializeField] private EncyclopediaAbilityButton abilityButtonPrefab;
    private List<EncyclopediaAbilityButton> abilityButtons = new List<EncyclopediaAbilityButton>();

    [SerializeField] private Button encyclopediaButton;

    public Action ReturnToEncyclopedia;

    private void Awake()
    {
        encyclopediaButton.onClick.AddListener(OnEncyclopediaButtonPressed);
    }
    public void InitializeMenu(UnitData unitData)
    {
        ResetDescription();
        unitNameText.text = unitData.UnitName;
        for (int i = 0; i < unitData.UnitAbility.Count; i++)
        {
            EncyclopediaAbilityButton newAbilityButton = Instantiate(abilityButtonPrefab, abilityButtonContent);
            newAbilityButton.InitializeButton(unitData.UnitAbility[i]);
            newAbilityButton.abilitySelected += UpdateAbilityDescription;
            abilityButtons.Add(newAbilityButton);
        }
    }

    public void ResetButtons()
    {
        for (int i = 0; i < abilityButtons.Count; i++)
        {
            abilityButtons[i].abilitySelected -= UpdateAbilityDescription;
            Destroy(abilityButtons[i].gameObject);
        }
        abilityButtons.Clear();
    }

    private void UpdateAbilityDescription(EncyclopediaAbilitySO info)
    {
        abilityImage.gameObject.SetActive(true);
        abilityImage.sprite = info.AbilityEncyclopediaImage;
        abilityDescriptionText.text = info.AbilityEncyclopediaName + "\n" + info.AbilityDescription;
        if (abilityImage.sprite == null)
        {
            abilityImage.gameObject.SetActive(false);
        }
    }
    private void ResetDescription()
    {
        abilityImage.gameObject.SetActive(false);
        abilityDescriptionText.text = "";
    }

    private void OnEncyclopediaButtonPressed()
    {
        ReturnToEncyclopedia?.Invoke();
    }
    private void OnDestroy()
    {
        encyclopediaButton.onClick.RemoveListener(OnEncyclopediaButtonPressed);
        ResetButtons();
    }
}
