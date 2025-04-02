using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuController : MonoBehaviour
{
    [SerializeField] private Transform levelContent;

    [SerializeField] private List<LevelInfo> levelInfos;
    [SerializeField] private LevelButton buttonPrefab;
    private List<LevelButton> activeLevelButtons = new List<LevelButton>();

    [SerializeField] private Button returnButton;

    public Action ReturnButtonPressed;
    public Action<LevelInfo> LevelSelected;

    private void Awake()
    {
        returnButton.onClick.AddListener(OnReturnButtonPressed);
    }

    public void InitializeLevelMenu(List<bool> unlockedLevel)
    {
        for (int i = 0; i < unlockedLevel.Count; i++)
        {
            LevelButton newButton = Instantiate(buttonPrefab, levelContent);
            newButton.InitializeButton(levelInfos[i]);
            if (unlockedLevel[i] == true)
            {
                newButton.UnlockedLevel();
            }
            else
            {
                newButton.LockedLevel();
            }

            activeLevelButtons.Add(newButton);
            newButton.buttonClicked += PlayLevel;
        }
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

    private void PlayLevel(LevelInfo levelInfo)
    {
        LevelSelected?.Invoke(levelInfo);
    }

    private void OnReturnButtonPressed()
    {
        ReturnButtonPressed?.Invoke();
    }
    private void OnDisable()
    {
        ClearLevelButton();
    }
    private void OnDestroy()
    {
        returnButton.onClick.RemoveListener(OnReturnButtonPressed);
        ClearLevelButton();
    }
}
