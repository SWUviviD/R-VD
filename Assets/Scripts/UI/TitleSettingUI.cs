using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSettingUI : MonoBehaviour
{
    [SerializeField] private KeyCode settingKey = KeyCode.Escape;

    [Header("BtnPanel")]
    [SerializeField] private GameObject btnPanel;
    [SerializeField] private Button SoundBtn;
    [SerializeField] private Button ResolutionBtn;
    [SerializeField] private Button ExitBtn;

    [Header("SoundSetting")]
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider bgmVolume;
    [SerializeField] private Slider sfxVolume;
    [SerializeField] private Button soundExitBtn;

    [Header("ResolutionSetting")]
    [SerializeField] private GameObject resolutionPanel;

    private void Start()
    {
        UIHelper.OnClick(SoundBtn, ShowSoundSetting);
        UIHelper.OnClick(ResolutionBtn, ShowSoundSetting);
        UIHelper.OnClick(ExitBtn, () => ShowPanel(false));

        UIHelper.OnClick(soundExitBtn, CloseSoundSetting);
    }

    public void ShowPanel(bool isActive)
    {
        gameObject.SetActive(isActive);

        btnPanel.SetActive(isActive == false);
        soundPanel.SetActive(isActive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(settingKey))
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsStageClear) return;

            ShowPanel(false);
        }
    }

    private void ShowSoundSetting()
    {
        masterVolume.value = SoundManager.Instance.MasterVolume;
        masterVolume.onValueChanged.AddListener((value) => SoundManager.Instance.SetMaterVolume(value));

        bgmVolume.value = SoundManager.Instance.BgmVolume;
        bgmVolume.onValueChanged.AddListener((value) => SoundManager.Instance.SetBGMVolume(value));

        sfxVolume.value = SoundManager.Instance.SfxVolume;
        sfxVolume.onValueChanged.AddListener((value) => SoundManager.Instance.SetSFXVolume(value));

        btnPanel.SetActive(false);
        soundPanel.SetActive(true);
    }

    private void CloseSoundSetting()
    {
        masterVolume.onValueChanged.RemoveAllListeners();
        bgmVolume.onValueChanged.RemoveAllListeners();
        sfxVolume.onValueChanged.RemoveAllListeners();

        btnPanel.SetActive(true);
        soundPanel.SetActive(false);
    }
}
