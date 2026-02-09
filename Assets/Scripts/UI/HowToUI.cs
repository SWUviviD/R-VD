using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToUI : MonoSingleton<HowToUI>
{
    [SerializeField] private GameObject ui;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private Button returnBtn;

    private void Start()
    {
        CloseUI();

        UIHelper.OnClick(returnBtn, CloseUI);
    }

    private void OnDisable()
    {
        scrollView.verticalNormalizedPosition = 1f;
    }

    public void ShowHowToUI()
    {
        ui.SetActive(true);
    }

    public void CloseUI()
    {
        ui.SetActive(false);
    }
}
