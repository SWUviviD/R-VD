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
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private AudioSource panelSFX;

    private bool isActive = false;

    private void Start()
    {
        UIHelper.OnClick(RestartBtn, GameManager.Instance.GameRestart);
        UIHelper.OnClick(ExitBtn, GameManager.Instance.GameExit);

        background.SetActive(isActive);
        settingPanel.SetActive(isActive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(settingKey))
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
}
