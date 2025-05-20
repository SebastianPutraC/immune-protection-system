using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ProfileMenuController : MonoBehaviour
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button changeNameButton;
    [SerializeField] private Button quizMenuButton;
    [SerializeField] private Button statisticMenuButton;
    [SerializeField] private Button deleteProgressButton;

    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI encyclopediaText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI averageStatisticText;
    [SerializeField] private TextMeshProUGUI quizStatisticText;
    [SerializeField] private List<TextMeshProUGUI> quizTexts;

    [SerializeField] private TMP_InputField newNameInput;

    [SerializeField] private GameObject statisticPanel;
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject changeNamePanel;

    public Action ReturnButtonPressed;
    public Action ProfileSelected;

    private void Awake()
    {
        returnButton.onClick.AddListener(OnReturnButtonPressed);
        quizMenuButton.onClick.AddListener(OpenQuizPanel);
        statisticMenuButton.onClick.AddListener(OpenStatisticPanel);
        changeNameButton.onClick.AddListener(OpenChangeNamePanel);
        cancelButton.onClick.AddListener(OpenStatisticPanel);
        confirmButton.onClick.AddListener(ChangeName);
        deleteProgressButton.onClick.AddListener(ResetProgress);
    }

    public void InitializeProfile(SaveData saveData)
    {
        playerNameText.text = "Nama Pemain : " + saveData.profileName;

        int unlockedEncyclopedia = saveData.enyclopediaUnlock.Where(x => x.Unlocked == true).ToList().Count;
        encyclopediaText.text = "Enyclopedia Terbuka : " + unlockedEncyclopedia + " / 14";

        int unlockedLevel = saveData.levelUnlock.Where(x => x.Unlocked == true).ToList().Count;
        levelText.text = "Level Selesai : " + unlockedLevel + " / 8";

        List<int> scores = saveData.quizScores.Select(x => x.quizScore).ToList();
        double average = CalculateAverage(scores);
        averageStatisticText.text = "Rata-rata Nilai Quiz : " + String.Format("{0:0.00}", average);
        quizStatisticText.text = "Rata-rata Nilai Quiz : " + String.Format("{0:0.00}", average);

        int levelIndex = 2;
        for (int i = 0; i < quizTexts.Count; i++)
        {
            quizTexts[i].text = "Level " + levelIndex + " : " + scores[i];
            levelIndex++;
        }

        OpenStatisticPanel();
    }
    private double CalculateAverage(List<int> scores)
    {
        double average = scores.Average();
        return average;
    }

    private void ChangeName()
    {
        if (newNameInput.text == "")
        {
            OpenStatisticPanel();
            return;
        }
        SaveManagerSingleton.Instance.UpdateName(newNameInput.text);
        playerNameText.text = "Nama Pemain : " + newNameInput.text;

        OpenStatisticPanel();
    }
    private void ResetProgress()
    {
        SaveManagerSingleton.Instance.ResetProfileData("Profile");
        InitializeProfile(SaveManagerSingleton.Instance.GetSelectedSaveData());
    }

    private void OpenQuizPanel()
    {
        quizPanel.gameObject.SetActive(true);
        statisticPanel.gameObject.SetActive(false);
        changeNamePanel.gameObject.SetActive(false);
    }
    private void OpenStatisticPanel()
    {
        quizPanel.gameObject.SetActive(false);
        statisticPanel.gameObject.SetActive(true);
        changeNamePanel.gameObject.SetActive(false);
        newNameInput.text = "";
    }
    private void OpenChangeNamePanel()
    {
        quizPanel.gameObject.SetActive(false);
        statisticPanel.gameObject.SetActive(false);
        changeNamePanel.gameObject.SetActive(true);
    }
    private void OnReturnButtonPressed()
    {
        ReturnButtonPressed?.Invoke();
    }
    private void OnDestroy()
    {
        returnButton.onClick.RemoveListener(OnReturnButtonPressed);
        quizMenuButton.onClick.RemoveListener(OpenQuizPanel);
        statisticMenuButton.onClick.RemoveListener(OpenStatisticPanel);
        changeNameButton.onClick.RemoveListener(OpenChangeNamePanel);
        cancelButton.onClick.RemoveListener(OpenStatisticPanel);
        confirmButton.onClick.RemoveListener(ChangeName);
        deleteProgressButton.onClick.RemoveListener(ResetProgress);
    }
}
