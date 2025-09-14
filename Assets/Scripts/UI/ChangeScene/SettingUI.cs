using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private KeyCode settingKey = KeyCode.Escape;
    [SerializeField] private Button RestartBtn;
    [SerializeField] private Button ExitBtn;
    [SerializeField] private Button ResumeBtn;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private AudioSource panelSFX;

    private bool isActive = false;

    private void Start()
    {
        UIHelper.OnClick(RestartBtn, GameManager.Instance.GameRestart);
        UIHelper.OnClick(ExitBtn, Exit);
        UIHelper.OnClick(ResumeBtn, CloseUI);

        background.SetActive(isActive);
        settingPanel.SetActive(isActive);
    }

    private void Exit()
    {
        GameManager.Instance.GameExit(true);
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
        }
    }
}
