using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private KeyCode settingKey = KeyCode.Escape;
    [SerializeField] private Button RestartBtn;
    [SerializeField] private Button SettingBtn;
    [SerializeField] private Button ReturnToMainBtn;
    [SerializeField] private Button ResumeBtn;
    [SerializeField] private Button HowToBtn;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private AudioSource panelSFX;

    [SerializeField] private GameObject btnPanel;

    [Header("SoundSetting")]
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Image masterVolumeImage;
    [SerializeField] private Slider bgmVolume;
    [SerializeField] private Image bgmVolumeImage;
    [SerializeField] private Slider sfxVolume;
    [SerializeField] private Image sfxVolumeImage;
    [SerializeField] private Button settingExitBtn;

    private bool isActive = false;

    private void Start()
    {
        UIHelper.OnClick(SettingBtn, ShowSetting);
        UIHelper.OnClick(RestartBtn, GameManager.Instance.GameRestart);
        UIHelper.OnClick(ReturnToMainBtn, GameManager.Instance.LoadTitle);
        UIHelper.OnClick(ResumeBtn, CloseUI);

        UIHelper.OnClick(settingExitBtn, CloseSetting);

        UIHelper.OnClick(HowToBtn, HowToUI.Instance.ShowHowToUI);

        background.SetActive(isActive);
        settingPanel.SetActive(isActive);

    }

    private void ShowSetting()
    {
        masterVolume.value = SoundManager.Instance.MasterVolume;
        masterVolumeImage.fillAmount = SoundManager.Instance.MasterVolume;
        masterVolume.onValueChanged.AddListener(SetMasterVolume);

        bgmVolume.value = SoundManager.Instance.BgmVolume;
        bgmVolumeImage.fillAmount = SoundManager.Instance.BgmVolume;
        bgmVolume.onValueChanged.AddListener(SetBGMrVolume);


        sfxVolume.value = SoundManager.Instance.SfxVolume;
        sfxVolumeImage.fillAmount = SoundManager.Instance.SfxVolume;
        sfxVolume.onValueChanged.AddListener(SetSFXVolume);

        btnPanel.SetActive(false);
        soundPanel.SetActive(true);
    }

    private void SetMasterVolume(float volume)
    {
        masterVolumeImage.fillAmount = volume;
        SoundManager.Instance.SetMaterVolume(volume);
    }

    private void SetBGMrVolume(float volume)
    {
        bgmVolumeImage.fillAmount = volume;
        SoundManager.Instance.SetBGMVolume(volume);
    }

    private void SetSFXVolume(float volume)
    {
        sfxVolumeImage.fillAmount = volume;
        SoundManager.Instance.SetSFXVolume(volume);
    }

    private void CloseSetting()
    {
        masterVolume.onValueChanged.RemoveAllListeners();
        bgmVolume.onValueChanged.RemoveAllListeners();
        sfxVolume.onValueChanged.RemoveAllListeners();

        SoundManager.Instance.SaveSoundSetting();

        btnPanel.SetActive(true);
        soundPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(settingKey))
        {
            if (GameManager.Instance.IsGameOver || 
                GameManager.Instance.IsStageClear ||
                DialogueManager.Instance.IsDialogueActive) return;

            CloseUI();
        }
    }

    private void CloseUI()
    {
        isActive = !isActive;
        background.SetActive(isActive);
        settingPanel.SetActive(isActive);
        panelSFX.Play();
        GameManager.Instance.SetSkillInput(isActive == false);
        if (isActive)
        {
            GameManager.Instance.StopGame();
        }
        else
        {
            GameManager.Instance.ResumeGame();
            CloseSetting();
        }
    }
}
