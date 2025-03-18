using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LevelButton : MonoBehaviour
{
    private LevelInfo levelInfo;

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button levelButton;

    public Action<LevelInfo> buttonClicked;

    private void Awake()
    {
        levelButton.onClick.AddListener(OnClick);
    }

    public void InitializeButton(LevelInfo info)
    {
        this.levelInfo = info;
        levelText.text = levelInfo.name;
    }

    public void LockedLevel()
    {
        this.levelButton.interactable = false;
    }
    public void UnlockedLevel()
    {
        this.levelButton.interactable = true;
    }

    private void OnClick()
    {
        buttonClicked?.Invoke(this.levelInfo);
    }
}
