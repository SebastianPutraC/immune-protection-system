using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LevelButton : MonoBehaviour
{
    private LevelInfo levelInfo;

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button levelButton;

    public Action<LevelId> buttonClicked;

    private void Awake()
    {
        levelButton.onClick.AddListener(OnClick);
    }

    public void InitializeButton(LevelInfo info, bool unlocked)
    {
        this.levelInfo = info;
        levelText.text = levelInfo.name;
        if (unlocked)
        {
            UnlockLevel();
        }
        else
        {
            LockLevel();
        }
    }

    public void LockLevel()
    {
        this.levelButton.interactable = false;
    }
    public void UnlockLevel()
    {
        this.levelButton.interactable = true;
    }

    private void OnClick()
    {
        buttonClicked?.Invoke(this.levelInfo.levelId);
    }
}
