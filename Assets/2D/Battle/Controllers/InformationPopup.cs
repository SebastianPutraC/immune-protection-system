using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InformationPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoDescription;
    [SerializeField] private Image infoImage;
    [SerializeField] private Image infoMascot;
    private List<InformationSO> currentInformation;

    [SerializeField] private Button nextInfoButton;
    [SerializeField] private Button skipInfoButton;

    private int currentInfoIndex = 0;

    public Action PopupFinished;

    private void Awake()
    {
        nextInfoButton.onClick.AddListener(NextInfo);
        skipInfoButton.onClick.AddListener(ExitPopup);
    }

    public void InitializePopup(List<InformationSO> informations)
    {
        this.gameObject.SetActive(true);
        currentInfoIndex = 0;
        currentInformation = informations;
        ShowInfo(currentInformation[0]);
    }
    private void NextInfo()
    {
        currentInfoIndex++;
        if (currentInfoIndex >= currentInformation.Count)
        {
            ExitPopup();
        }
        else
        {
            ShowInfo(currentInformation[currentInfoIndex]);
        }
    }
    private void ShowInfo(InformationSO info)
    {
        infoDescription.text = info.InfoText;
        infoImage.sprite = info.InfoImage;
        if (infoImage.sprite == null)
        {
            infoImage.gameObject.SetActive(false);
        }
        else
        {
            infoImage.gameObject.SetActive(true);
        }
        infoMascot.sprite = info.InfoMascot;
        if (infoMascot.sprite == null)
        {
            infoMascot.gameObject.SetActive(false);
        }
        else
        {
            infoMascot.gameObject.SetActive(true);
        }
    }
    public void ExitPopup()
    {
        this.gameObject.SetActive(false);
        PopupFinished?.Invoke();
    }

    private void OnDestroy()
    {
        nextInfoButton.onClick.RemoveListener(NextInfo);
        skipInfoButton.onClick.RemoveListener(ExitPopup);
    }
}
