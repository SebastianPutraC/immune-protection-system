using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionMenuController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Button returnButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public Action OnReturnButtonPressed;
    public const string MUSIC_PARAM = "musicVolume";
    public const string SFX_PARAM = "sfxVolume";

    private void Awake()
    {
        returnButton.onClick.AddListener(ReturnButtonPressed);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    public void InitializeSetting(float musicVolume, float sfxVolume)
    {
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }

    private void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(MUSIC_PARAM, Mathf.Log10(value) * 20);
    }
    private void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(SFX_PARAM, Mathf.Log10(value) * 20);
    }
    private void ConfirmChanges()
    {
        SaveManagerSingleton.Instance.UpdateSoundData(musicSlider.value, sfxSlider.value);
        AudioManagerSingleton.Instance.LoadVolume(musicSlider.value, sfxSlider.value);
    }

    private void ReturnButtonPressed()
    {
        ConfirmChanges();
        OnReturnButtonPressed?.Invoke();
    }
    private void OnDestroy()
    {
        returnButton.onClick.RemoveListener(ReturnButtonPressed);
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }
}
