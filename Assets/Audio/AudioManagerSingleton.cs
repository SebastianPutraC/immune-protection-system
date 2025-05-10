using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerSingleton : MonoBehaviour
{
    public static AudioManagerSingleton Instance;

    [SerializeField] private AudioMixer audioMixer;

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
        audioMixer.SetFloat(MUSIC_PARAM, Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat(SFX_PARAM, Mathf.Log10(sfxVolume) * 20);
    }
}
