using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaMechanicGrid : MonoBehaviour
{
    public MechanicSO mechanicSo;

    [SerializeField] private Button mechanicButton;
    [SerializeField] private TextMeshProUGUI MechanicNameText;

    [SerializeField] private GameObject selectedIndicator;

    public Action<EncyclopediaMechanicGrid> MechanicClicked;

    private void Awake()
    {
        mechanicButton.onClick.AddListener(OnClick);
    }

    public void InitializeData(MechanicSO mechanicData)
    {
        this.mechanicSo = mechanicData;

        MechanicNameText.text = mechanicSo.mechanicTitle;
        mechanicButton.interactable = true;

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
        MechanicClicked?.Invoke(this);
    }

    private void OnDestroy()
    {
        mechanicButton.onClick.RemoveListener(OnClick);
    }
}
