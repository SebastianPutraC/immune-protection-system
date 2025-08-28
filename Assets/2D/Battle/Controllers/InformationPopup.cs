using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class InformationPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoDescription;
    [SerializeField] private Image infoImage;
    [SerializeField] private Image infoMascot;
    private List<InformationSO> currentInformation;

    [SerializeField] private Button nextInfoButton;
    [SerializeField] private Button skipInfoButton;
    [SerializeField] private Button skipSomeButton;
    [SerializeField] private Button previousInfoButton;

    private List<int> skipIndexList = new List<int>();

    private int currentInfoIndex = 0;

    public Action PopupFinished;

    private void Awake()
    {
        nextInfoButton.onClick.AddListener(NextInfo);
        skipInfoButton.onClick.AddListener(ExitPopup);
        skipSomeButton.onClick.AddListener(SkipSomeInfo);
        previousInfoButton.onClick.AddListener(PreviousInfo);
    }

    public void InitializePopup(List<InformationSO> informations, List<int> skipIndex = null)
    {
        this.gameObject.SetActive(true);
        if (skipIndex != null)
        {
            skipIndexList = skipIndex;
        }
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
    private void PreviousInfo()
    {
        currentInfoIndex--;
        if (currentInfoIndex < 0)
        {
            currentInfoIndex = 0;
        }
        ShowInfo(currentInformation[currentInfoIndex]);
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

        if (currentInfoIndex <= 0)
        {
            previousInfoButton.interactable = false;
        }
        else
        {
            previousInfoButton.interactable = true;
        }
    }
    private void SkipSomeInfo()
    {
        List<int> forwardIndex = skipIndexList.Where(x => x > currentInfoIndex).ToList();
        if (forwardIndex.Count <= 0)
        {
            ExitPopup();
        }
        else
        {
            currentInfoIndex = forwardIndex[0];
            ShowInfo(currentInformation[currentInfoIndex]);
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
        skipSomeButton.onClick.RemoveListener(SkipSomeInfo);
        previousInfoButton.onClick.RemoveListener(PreviousInfo);
    }
}
