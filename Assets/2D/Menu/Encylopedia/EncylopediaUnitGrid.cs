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

    private readonly Color deselectColor = new Color32(103, 43, 144, 255);
    private readonly Color selectColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

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
            unitNameText.text = "Anda belum pernah menemui Unit ini";
            unitImage.sprite = lockedImage;
            unitButton.interactable = false;
        }

        DeselectGrid();
    }

    public void SelectGrid()
    {
        unitButton.image.color = selectColor;
    }
    public void DeselectGrid()
    {
        unitButton.image.color = deselectColor;
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
