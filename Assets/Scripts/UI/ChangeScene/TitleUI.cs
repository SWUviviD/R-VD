using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private Button ExitBtn;

    private GameData gameData;

    private void Start()
    {
        UIHelper.OnClick(StartBtn, GameManager.Instance.NewGameStart);
        UIHelper.OnClick(ContinueBtn, GameManager.Instance.LoadData);
        UIHelper.OnClick(ExitBtn, GameManager.Instance.GameExit);

        gameData = GameManager.Instance.GameData;
        if (gameData != null && gameData.stageID != 0)
        {
            ContinueBtn.GetComponent<Image>().raycastTarget = true;
            ContinueBtn.GetComponent<Image>().color = Color.white;
            ContinueBtn.GetComponentInChildren<Text>().color = Color.white;
        }
    }
}
