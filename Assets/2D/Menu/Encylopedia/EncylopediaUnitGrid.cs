using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class EncylopediaUnitGrid : MonoBehaviour
{
    public UnitData unitSo;

    [SerializeField] private Button unitButton;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image unitImage;
    [SerializeField] private Sprite lockedImage;

    [SerializeField] private GameObject selectedIndicator;

    public Action<EncylopediaUnitGrid> UnitClicked;

    private void Awake()
    {
        unitButton.onClick.AddListener(OnClick);
    }

    public void InitializeData(UnitData unitData, bool unlocked)
    {
        this.unitSo = unitData;

        if (unlocked)
        {
            unitNameText.text = unitSo.UnitName;
            unitImage.sprite = unitSo.UnitSprite;
            unitButton.interactable = true;
        }
        else
        {
            unitNameText.text = "You haven't unlocked this unit yet";
            unitImage.sprite = lockedImage;
            unitButton.interactable = false;
        }

        DeselectGrid();
    }

    public void SelectGrid()
    {
        selectedIndicator.SetActive(true);
    }
    public void DeselectGrid()
    {
        selectedIndicator.SetActive(false);
    }

    private void OnClick()
    {
        UnitClicked?.Invoke(this);
    }

    private void OnDestroy()
    {
        unitButton.onClick.RemoveListener(OnClick);
    }
}
