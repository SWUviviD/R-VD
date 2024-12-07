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
    [SerializeField] private GameObject panel;

    private bool isActive = false;

    private void Start()
    {
        UIHelper.OnClick(RestartBtn, OnRestart);
        UIHelper.OnClick(ExitBtn, OnExit);

        panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(settingKey))
        {
            isActive = !isActive;
            panel.SetActive(isActive);
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

    private void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnExit()
    {
        Application.Quit();
    }
}
