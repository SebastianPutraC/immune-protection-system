using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class UnitCardController : MonoBehaviour, IPointerClickHandler
{
    public AllyUnit unitData;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitCostText;

    public Action<UnitCardController> cardClicked;

    private readonly Color deselectColor = new Color(1, 1, 1, 0.5f);
    private readonly Color selectColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    public void InitializeData(AllyUnit data)
    {
        unitData = data;
        unitImage.sprite = unitData.UnitSprite;
        unitNameText.text = unitData.UnitName;
        unitCostText.text = unitData.UnitCost.ToString();
        backgroundImage.color = deselectColor;
    }

    private void SelectCard()
    {
        backgroundImage.color = selectColor; 
    }
    
    public void DeselectCard()
    {
        backgroundImage.color = deselectColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cardClicked?.Invoke(this);
        SelectCard();
    }
}
