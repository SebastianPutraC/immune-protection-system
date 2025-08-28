using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaMechanicGrid : MonoBehaviour
{
    public MechanicSO mechanicSo;

    [SerializeField] private Button mechanicButton;
    [SerializeField] private TextMeshProUGUI MechanicNameText;

    public Action<EncyclopediaMechanicGrid> MechanicClicked;

    private readonly Color deselectColor = new Color32(168, 168, 168, 255);
    private readonly Color selectColor = new Color(0.5f, 0.5f, 0.5f, 1f);

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
        mechanicButton.image.color = selectColor;
    }
    public void DeselectGrid()
    {
        mechanicButton.image.color = deselectColor;
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
