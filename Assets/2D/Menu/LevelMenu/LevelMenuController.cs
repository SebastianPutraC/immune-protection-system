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

    public Action ReturnButtonPressed;
    public Action<LevelId> LevelSelected;

    private void Awake()
    {
        returnButton.onClick.AddListener(OnReturnButtonPressed);
    }

    public void InitializeLevelMenu(List<LevelData> unlockedLevel, List<LevelInfo> levelReferences)
    {
        for (int i = 0; i < unlockedLevel.Count; i++)
        {
            LevelInfo newLevel = levelReferences.Where(x => x.levelId.Equals((LevelId)unlockedLevel[i].LevelId)).FirstOrDefault();
            LevelButton newButton = Instantiate(buttonPrefab, levelContent);

            newButton.InitializeButton(newLevel, unlockedLevel[i].Unlocked);

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

    private void PlayLevel(LevelId LevelId)
    {
        LevelSelected?.Invoke(LevelId);
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
