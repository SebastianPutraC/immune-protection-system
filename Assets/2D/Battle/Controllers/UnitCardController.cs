using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class UnitCardController : MonoBehaviour
{
    public AllyUnit unitData;

    [SerializeField] private Button cardButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitCostText;

    public GameObject tutorialArrow;

    public Action<UnitCardController> cardClicked;

    private readonly Color deselectColor = new Color(1, 1, 1, 0.5f);
    private readonly Color selectColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private void Awake()
    {
        cardButton.onClick.AddListener(OnCardClicked);
    }
    public void InitializeData(AllyUnit data)
    {
        unitData = data;
        unitImage.sprite = unitData.UnitSprite;
        unitNameText.text = unitData.UnitName;
        unitCostText.text = unitData.UnitCost.ToString();
    }

    public void SelectCard()
    {
        backgroundImage.color = selectColor; 
    }
    public void DeselectCard()
    {
        backgroundImage.color = deselectColor;
    }

    public void LockCard()
    {
        cardButton.interactable = false;
    }
    public void UnlockCard()
    {
        cardButton.interactable = true;
    }

    public void OnCardClicked()
    {
        cardClicked?.Invoke(this);
    }
}
