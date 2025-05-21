using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endGameText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button quitButton;

    public Action NextLevelPressed;
    public Action QuitPressed;

    private void Awake()
    {
        nextLevelButton.onClick.AddListener(OnNextLevelPressed);
        quitButton.onClick.AddListener(OnQuitPressed);
    }

    public void GameLost()
    {
        endGameText.text = "Defeat";
        nextLevelButton.gameObject.SetActive(false);
    }
    public void GameWon()
    {
        endGameText.text = "Victory";
        nextLevelButton.gameObject.SetActive(true);
    }

    private void OnQuitPressed()
    {
        QuitPressed?.Invoke();
    }
    private void OnNextLevelPressed()
    {
        NextLevelPressed?.Invoke();
    }

    private void OnDestroy()
    {
        nextLevelButton.onClick.RemoveListener(OnNextLevelPressed);
        quitButton.onClick.RemoveListener(OnQuitPressed);
    }
}
