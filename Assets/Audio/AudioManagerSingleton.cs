using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerSingleton : MonoBehaviour
{
    public static AudioManagerSingleton Instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource buttonSfx;

    public const string MUSIC_PARAM = "musicVolume";
    public const string SFX_PARAM = "sfxVolume";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        SaveData saveData = SaveManagerSingleton.Instance.GetSelectedSaveData();
        LoadVolume(saveData.musicVolume, saveData.sfxVolume);
    }

    public void LoadVolume(float musicVolume, float sfxVolume)
    {
        if (musicVolume > 0)
        {
            audioMixer.SetFloat(MUSIC_PARAM, Mathf.Log10(musicVolume) * 20);
        }
        else
        {
            audioMixer.SetFloat(MUSIC_PARAM, -80);
        }
        if (sfxVolume > 0)
        {
            audioMixer.SetFloat(SFX_PARAM, Mathf.Log10(sfxVolume) * 20);
        }
        else
        {
            audioMixer.SetFloat(SFX_PARAM, -80);
        }
    }

    public void PlayButtonSFX()
    {
        buttonSfx.Play();
    }
}
