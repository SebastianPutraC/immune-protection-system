using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class UnitPanelController : MonoBehaviour
{
    [SerializeField] private List<AllyUnit> unitReference; // Placeholders, Not Used
    [SerializeField] private List<UnitCardController> unitCards;
    private List<UnitCardController> adaptiveUnitCards = new List<UnitCardController>();

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

            if (allyUnit[i].ImmuneCellType == ImmuneType.ADAPTIVE)
            {
                newUnitCard.LockCard();
                adaptiveUnitCards.Add(newUnitCard);
            }
            unitCards.Add(newUnitCard);
        }
    }
    public void ResetUnitCards()
    {
        for (int i = 0; i < unitCards.Count; i++)
        {
            unitCards[i].cardClicked -= ReadySelectedCard;
            Destroy(unitCards[i].gameObject);
        }
        unitCards.Clear();
        for (int i = 0; i < adaptiveUnitCards.Count; i++)
        {
            Destroy(adaptiveUnitCards[i].gameObject);
        }
        adaptiveUnitCards.Clear();
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
        if (selectedCard != null)
        {
            selectedCard.DeselectCard();
        }

        selectedCard = unitCard;
        selectedCard.SelectCard();
    }
    public AllyUnit DeployCard()
    {
        if (selectedCard == null)
            return null;

        return selectedCard.unitData;
    }
    public void TurnTutorialArrow(UnitId unitId)
    {
        UnitCardController unitCardTutorial = unitCards.Where(x => x.unitData.unitId == unitId).FirstOrDefault();
        unitCardTutorial.tutorialArrow.SetActive(true);
    }
    public void TurnOffArrows()
    {
        for (int i = 0; i < unitCards.Count; i++)
        {
            unitCards[i].tutorialArrow.gameObject.SetActive(false);
        }
    }

    public void UnlockAdaptiveCard()
    {
        for (int i = 0; i < adaptiveUnitCards.Count; i++)
        {
            adaptiveUnitCards[i].UnlockCard();
        }
    }

    public UnitCardController GetUnitCard(UnitId id)
    {
        UnitCardController unitCard = unitCards.Where(x => x.unitData.unitId.Equals(id)).FirstOrDefault();
        return unitCard;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < unitCards.Count; i++)
        {
            unitCards[i].cardClicked -= ReadySelectedCard;
        }
    }
}
