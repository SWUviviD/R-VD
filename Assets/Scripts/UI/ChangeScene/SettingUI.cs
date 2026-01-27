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
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private AudioSource panelSFX;

    [SerializeField] private GameObject btnPanel;

    [Header("SoundSetting")]
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider bgmVolume;
    [SerializeField] private Slider sfxVolume;
    [SerializeField] private Button settingExitBtn;

    private bool isActive = false;

    private void Start()
    {
        UIHelper.OnClick(SettingBtn, ShowSetting);
        UIHelper.OnClick(RestartBtn, GameManager.Instance.GameRestart);
        UIHelper.OnClick(ReturnToMainBtn, GameManager.Instance.LoadTitle);
        UIHelper.OnClick(ResumeBtn, CloseUI);

        UIHelper.OnClick(settingExitBtn, CloseSetting);

        background.SetActive(isActive);
        settingPanel.SetActive(isActive);
    }

    private void ShowSetting()
    {
        masterVolume.value = SoundManager.Instance.MasterVolume;
        masterVolume.onValueChanged.AddListener((value) => SoundManager.Instance.SetMaterVolume(value));

        bgmVolume.value = SoundManager.Instance.BgmVolume;
        bgmVolume.onValueChanged.AddListener((value) =>
        {
            Debug.Log("BGMChanged");
            SoundManager.Instance.SetBGMVolume(value);
        });


        sfxVolume.value = SoundManager.Instance.SfxVolume;
        sfxVolume.onValueChanged.AddListener((value) => SoundManager.Instance.SetSFXVolume(value));

        btnPanel.SetActive(false);
        soundPanel.SetActive(true);
    }

    private void CloseSetting()
    {
        masterVolume.onValueChanged.RemoveAllListeners();
        bgmVolume.onValueChanged.RemoveAllListeners();
        sfxVolume.onValueChanged.RemoveAllListeners();

        btnPanel.SetActive(true);
        soundPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(settingKey))
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsStageClear) return;

            CloseUI();
        }
    }

    private void CloseUI()
    {
        isActive = !isActive;
        background.SetActive(isActive);
        settingPanel.SetActive(isActive);
        panelSFX.Play();
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
