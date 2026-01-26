using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StageBGM
{
    public StageID stageID;
    public AudioClip bgm;
}

public class SoundManager : MonoSingleton<SoundManager>
{
    private readonly string MASTER_VOLUME_KEY = "Master Volume";
    private readonly string BGM_VOLUME_KEY = "BGM Volume";
    private readonly string SFX_VOLUME_KEY = "SFX Volume";

    [Header("Stage BGM Settings")]
    [SerializeField] private StageBGM[] stageBGMs;

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
        bgmSource.loop = true;

        SetMaterVolume(MasterVolume);

        base.Init();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneLoadManager.Instance.PermanentOnSceneLoadedAction(
            (scene, mode) =>
            {
                GetAllAudioSource();

                StageID stage = SceneLoadManager.Instance.GetActiveStage();
                PlayBGMByStage(stage);
            }
        );
    }

    private void GetAllAudioSource()
    {
        var sources = FindObjectsByType<AudioSource>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.InstanceID
        );

        sfxSources = sources.Where(_ => _ != bgmSource).ToList();
        SetSFXVolume(SfxVolume);
    }

    #region Stage BGM
    public void PlayBGMByStage(StageID stageID)
    {
        if (stageBGMs == null || stageBGMs.Length == 0)
            return;

        StageBGM data = stageBGMs.FirstOrDefault(x => x.stageID == stageID);

        if (data == null || data.bgm == null)
            return;

        if (bgmSource.clip == data.bgm)
            return; // 같은 BGM이면 재시작 안 함

        PlayBGM(data.bgm);
    }
    #endregion

    #region SoundSettings
    private void InitializeSoundSetting(bool initializeCompletely = false)
    {
        if (!initializeCompletely &&
            PlayerPrefs.HasKey(MASTER_VOLUME_KEY) &&
            PlayerPrefs.HasKey(BGM_VOLUME_KEY) &&
            PlayerPrefs.HasKey(SFX_VOLUME_KEY))
        {
            return;
        }

        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, 1f);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, 1f);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, 1f);
    }

    private void GetSoundSetting()
    {
        MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
        BgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY);
        SfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
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

        if (!resetAllVolume) return;

        bgmSource.volume = GetBGMVolume();

        if (sfxSources == null || sfxSources.Count == 0)
            return;

        foreach (AudioSource audio in sfxSources)
        {
            audio.volume = GetSFXVolume();
        }
    }

    public void SetBGMVolume(float value, bool resetAllVolume = true)
    {
        BgmVolume = value;

        if (!resetAllVolume) return;

        bgmSource.volume = GetBGMVolume();
    }

    public void SetSFXVolume(float value, bool resetAllVolume = true)
    {
        SfxVolume = value;

        if (!resetAllVolume) return;

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
        bgmSource.volume = GetBGMVolume();
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
