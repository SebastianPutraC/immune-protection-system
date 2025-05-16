using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{
    [SerializeField] private GameObject unitPanel;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitDescriptionText;

    [SerializeField] private GameObject mechanicPanel;
    [SerializeField] private TextMeshProUGUI mechanicTitleText;
    [SerializeField] private TextMeshProUGUI mechanicDescriptionText;

    [SerializeField] private Button aboutButton;
    [SerializeField] private Button abilityButton;

    public Action AboutPressed;
    public Action AbilityPressed;

    private void Awake()
    {
        aboutButton.onClick.AddListener(OnAboutButtonClicked);
        abilityButton.onClick.AddListener(OnAbilityButtonClicked);
    }

    public void SetupUnitDescriptionPanel(UnitData unitData)
    {
        ClearDescriptionPanel();

        SwitchToUnit();
        if (unitData != null)
        {
            unitImage.gameObject.SetActive(true);
            unitImage.sprite = unitData.UnitSprite;
            unitNameText.text = unitData.UnitName;
            unitDescriptionText.text = unitData.UnitSimpleDesc;
            abilityButton.interactable = true;
            aboutButton.interactable = true;
        }
    }
    public void SetupMechanicDescriptionPanel(MechanicSO mechanicInfo)
    {
        ClearDescriptionPanel();

        SwitchToMechanic();
        if (mechanicInfo != null)
        {
            mechanicTitleText.text = mechanicInfo.mechanicTitle;
            mechanicDescriptionText.text = mechanicInfo.mechanicDescription;
        }
    }

    public void SwitchToMechanic()
    {
        unitPanel.gameObject.SetActive(false);
        mechanicPanel.gameObject.SetActive(true);
    }
    public void SwitchToUnit()
    {
        unitPanel.gameObject.SetActive(true);
        mechanicPanel.gameObject.SetActive(false);
    }

    private void OnAbilityButtonClicked()
    {
        AbilityPressed?.Invoke();
    }
    private void OnAboutButtonClicked()
    {
        AboutPressed?.Invoke();
    }

    public void ClearDescriptionPanel()
    {
        unitImage.gameObject.SetActive(false);
        unitImage.sprite = null;
        unitNameText.text = "";
        unitDescriptionText.text = "";
        abilityButton.interactable = false;
        aboutButton.interactable = false;

        mechanicTitleText.text = "";
        mechanicDescriptionText.text = "";
    }

    private void OnDestroy()
    {
        aboutButton.onClick.RemoveListener(OnAboutButtonClicked);
        abilityButton.onClick.RemoveListener(OnAbilityButtonClicked);
    }
}
