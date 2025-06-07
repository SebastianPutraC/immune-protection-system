using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuController : MonoBehaviour
{
    [SerializeField] private Transform levelContent;

    [SerializeField] private LevelButton buttonPrefab;
    private List<LevelButton> activeLevelButtons = new List<LevelButton>();

    [SerializeField] private Button returnButton;

    [SerializeField] private GameObject epiloguePanel;
    [SerializeField] private Button epilogueButton;
    [SerializeField] private Button backToLevelButton;

    public Action<bool> ReturnButtonPressed;
    public Action<LevelId> LevelSelected;
    public Action ButtonPressed;

    private void Awake()
    {
        returnButton.onClick.AddListener(OnReturnButtonPressed);
        epilogueButton.onClick.AddListener(OpenEpilog);
        backToLevelButton.onClick.AddListener(BackToLevel);
    }

    public void InitializeLevelMenu(List<LevelData> unlockedLevel, List<LevelInfo> levelReferences, bool isWonAllLevel)
    {
        for (int i = 0; i < unlockedLevel.Count; i++)
        {
            LevelInfo newLevel = levelReferences.Where(x => x.levelId.Equals((LevelId)unlockedLevel[i].LevelId)).FirstOrDefault();
            LevelButton newButton = Instantiate(buttonPrefab, levelContent);

            newButton.InitializeButton(newLevel, unlockedLevel[i].Unlocked);
            newButton.buttonClicked += PlayLevel;

            activeLevelButtons.Add(newButton);
        }

        epilogueButton.interactable = isWonAllLevel;

        BackToLevel();
    }
    private void ClearLevelButton()
    {
        for (int i = 0; i < activeLevelButtons.Count; i++)
        {
            activeLevelButtons[i].buttonClicked -= PlayLevel;
            Destroy(activeLevelButtons[i].gameObject);
        }

        activeLevelButtons.Clear();
    }

    private void PlayLevel(LevelId LevelId)
    {
        LevelSelected?.Invoke(LevelId);
        ButtonPressed?.Invoke();
    }

    private void OpenEpilog()
    {
        epiloguePanel.gameObject.SetActive(true);
        ButtonPressed?.Invoke();
    }
    private void BackToLevel()
    {
        epiloguePanel.gameObject.SetActive(false);
        ButtonPressed?.Invoke();
    }

    private void OnReturnButtonPressed()
    {
        ReturnButtonPressed?.Invoke(true);
    }
    private void OnDisable()
    {
        ClearLevelButton();
    }
    private void OnDestroy()
    {
        returnButton.onClick.RemoveListener(OnReturnButtonPressed);
        epilogueButton.onClick.RemoveListener(OpenEpilog);
        backToLevelButton.onClick.RemoveListener(BackToLevel);
        ClearLevelButton();
    }
}
