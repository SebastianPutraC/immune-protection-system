using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitPanelController : MonoBehaviour
{
    [SerializeField] private List<AllyUnit> unitReference; // Placeholders
    [SerializeField] private List<UnitCardController> unitCards;

    [SerializeField] private UnitCardController unitCardPrefab;
    [SerializeField] private Transform unitCardContent;

    public UnitCardController selectedCard; //Change to private or move to battle prob

    private void Awake()
    {
        //InitializeUnitCards(unitReference); //Default
    }

    public void InitializeUnitCards(List<AllyUnit> allyUnit)
    {
        for (int i = 0; i < allyUnit.Count; i++)
        {
            UnitCardController newUnitCard = Instantiate(unitCardPrefab, unitCardContent);
            newUnitCard.InitializeData(allyUnit[i]);
            newUnitCard.cardClicked += ReadySelectedCard;
            unitCards.Add(newUnitCard);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Change to better input system
        {
            ResetSelectedCard();
        }
    }

    public void ResetSelectedCard()
    {
        if (selectedCard == null)
            return;

        selectedCard.DeselectCard();
        selectedCard = null;
    }

    private void ReadySelectedCard(UnitCardController unitCard)
    {
        selectedCard = unitCard;
    }

    public AllyUnit DeployCard()
    {
        if (selectedCard == null)
            return null;

        return selectedCard.unitData;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < unitCards.Count; i++)
        {
            unitCards[i].cardClicked -= ReadySelectedCard;
        }
    }
}
