using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    private readonly string MASTER_VOLUME_KEY = "Master Volume";
    private readonly string BGM_VOLUME_KEY = "BGM Volume";
    private readonly string SFX_VOLUME_KEY = "SFX Volume";

    [SerializeField] private AudioClip bgm;

    private AudioSource bgmSource;
    private List<AudioSource> sfxSources;

    public float MasterVolume { get; private set; }
    public float BgmVolume { get; private set; }
    public float SfxVolume { get; private set; }

    protected override void Init()
    {
        InitializeSoundSetting();
        GetSoundSetting();

        var bgmObj = new GameObject("BGM");
        bgmObj.transform.parent = transform;
        bgmSource = bgmObj.AddComponent<AudioSource>();

        SetMaterVolume(MasterVolume);
        PlayBGM(bgm);

        base.Init();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneLoadManager.Instance.PermanentOnSceneLoadedAction((Scene, LoadSceneMode) =>GetAllAudioSource());
    }

    private void GetAllAudioSource()
    {
        var sources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        sfxSources = sources.Where(_ => _ != bgmSource).ToList();
        SetSFXVolume(SfxVolume);
    }

    #region SoundSettings
    private void InitializeSoundSetting(bool initializeCompletely = false)
    {
        if (initializeCompletely == false &&
            PlayerPrefs.HasKey(MASTER_VOLUME_KEY) == true &&
            PlayerPrefs.HasKey(BGM_VOLUME_KEY) == true &&
            PlayerPrefs.HasKey(SFX_VOLUME_KEY) == true)
        {
            return;
        }

        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, 1f);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, 1f);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, 1f);

        return;
    }

    private void GetSoundSetting()
    {
        MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
        BgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY);
        SfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);

        print($"{MasterVolume} {BgmVolume} {SfxVolume}");
    }

    public void SaveSoundSetting()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, MasterVolume);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, BgmVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, SfxVolume);
    }
    #endregion

    #region GetSoundVolume
    private float GetBGMVolume() => MasterVolume * BgmVolume;
    private float GetSFXVolume() => MasterVolume * SfxVolume;
    #endregion

    #region SetSoundVolume
    public void SetMaterVolume(float value, bool resetAllVolume = true)
    {
        MasterVolume = value;

        if (resetAllVolume == false) return;

        bgmSource.volume = GetBGMVolume();
        print($"Change Volume: bgm {GetBGMVolume()} {GetSFXVolume()}");

        if (sfxSources == null || sfxSources.Count == 0)
            return;

        foreach(AudioSource audio in sfxSources)
        {
            audio.volume = GetSFXVolume();
        }
    }
    public void SetBGMVolume(float value, bool resetAllVolume = true)
    {
        BgmVolume = value;

        if (resetAllVolume == false) return;

        print($"Change Volume: bgm {GetBGMVolume()} {GetSFXVolume()}");

        bgmSource.volume = GetBGMVolume();
    }

    public void SetSFXVolume(float value, bool resetAllVolume = true)
    {
        SfxVolume = value;

        if (resetAllVolume == false) return;
        print($"Change Volume: bgm {GetBGMVolume()} {GetSFXVolume()}");

        if (sfxSources == null || sfxSources.Count == 0)
            return;

        foreach (AudioSource audio in sfxSources)
        {
            audio.volume = GetSFXVolume();
        }
    }
    #endregion

    #region PlaySound
    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayBGM()
    {
        bgmSource.Play();
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
    #endregion


    private void OnDestroy()
    {
        SaveSoundSetting();   
    }
}
