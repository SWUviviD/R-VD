using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private Button ExitBtn;

    private void Start()
    {
        UIHelper.OnClick(StartBtn, GameManager.Instance.GameStart);
        UIHelper.OnClick(ContinueBtn, GameManager.Instance.LoadData);
        UIHelper.OnClick(ExitBtn, GameManager.Instance.GameExit);
    }
}
