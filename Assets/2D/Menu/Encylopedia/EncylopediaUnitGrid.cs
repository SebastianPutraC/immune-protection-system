using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class EncylopediaUnitGrid : MonoBehaviour
{
    public EncylopediaUnit unitSo;

    [SerializeField] private Button unitButton;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image unitImage;

    [SerializeField] private GameObject selectedIndicator;

    public Action<EncylopediaUnitGrid> UnitClicked;

    private void Awake()
    {
        unitButton.onClick.AddListener(OnClick);
    }

    public void InitializeData(EncylopediaUnit unitData)
    {
        this.unitSo = unitData;
        unitNameText.text = unitSo.unitName;
        unitImage.sprite = unitSo.unitSprite;

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
