using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AbilityButton : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private Image primaryImage;
    [SerializeField] private Image secondaryImage;

    [SerializeField] private TextMeshProUGUI abilityText;

    private AbilityId abilityType;

    public Action<AbilityId> abilityPressed;

    public void InitializeButton(AbilityId abilityType, string abilityText, Sprite primary, Sprite secondary = null, bool isSecondaryOn = false)
    {
        this.abilityType = abilityType;
        this.abilityText.text = abilityText;
        this.primaryImage.sprite = primary;
        this.secondaryImage.sprite = secondary;

        if (isSecondaryOn)
        {
            SwapImage();
        }
        else
        {
            secondaryImage.gameObject.SetActive(false);
        }
    }

    private void SwapImage()
    {
        if (primaryImage.gameObject.activeSelf)
        {
            secondaryImage.gameObject.SetActive(true);
            primaryImage.gameObject.SetActive(false);
        }
        else
        {
            secondaryImage.gameObject.SetActive(false);
            primaryImage.gameObject.SetActive(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        abilityPressed?.Invoke(abilityType);
    }
}
