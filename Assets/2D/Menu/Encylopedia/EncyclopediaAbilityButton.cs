using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EncyclopediaAbilityButton : MonoBehaviour
{
    [SerializeField] private Button abilityButton;
    [SerializeField] private TextMeshProUGUI abilityName;

    private EncyclopediaAbilitySO abilityData;

    public Action<EncyclopediaAbilitySO> abilitySelected;

    private void Awake()
    {
        abilityButton.onClick.AddListener(ButtonClicked);
    }

    public void InitializeButton(EncyclopediaAbilitySO info)
    {
        abilityData = info;
        abilityName.text = info.AbilityEncyclopediaName;
    }

    private void ButtonClicked()
    {
        abilitySelected?.Invoke(abilityData);
    }

    private void OnDestroy()
    {
        abilityButton.onClick.RemoveListener(ButtonClicked);
    }
}
