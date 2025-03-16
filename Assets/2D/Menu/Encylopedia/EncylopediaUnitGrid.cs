using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class EncylopediaUnitGrid : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;

    [SerializeField] private GameObject selectedIndicator;

    public Action UnitClicked;

    public void InitializeData()
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UnitClicked?.Invoke();
    }
}
