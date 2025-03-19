using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AbilityButton : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI abilityText;

    private AbilityType abilityType; // Should Change to its own SO

    public Action<AbilityType> abilityPressed;

    public void InitializeButton(AbilityType abilityType, Color color, string abilityText)
    {
        this.abilityType = abilityType;
        this.abilityText.text = abilityText;
        buttonImage.color = color;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        abilityPressed?.Invoke(abilityType);
    }
}
