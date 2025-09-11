using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private Button CreditBtn;
    [SerializeField] private Button ExitBtn;

    private void Start()
    {
        UIHelper.OnClick(StartBtn, NewGameStart);
        UIHelper.OnClick(ExitBtn, GameExit);
        UIHelper.OnClick(CreditBtn, LoadCredit);

        if (GameManager.Instance.GameDataManager.LoadGameData() == true)
        {
            ContinueBtn.GetComponent<Image>().raycastTarget = true;
            ContinueBtn.GetComponent<Image>().color = Color.white;
            ContinueBtn.GetComponentInChildren<Text>().color = Color.white;
        }

        UIHelper.OnClick(ContinueBtn, GameManager.Instance.LoadData);
    }

    private void NewGameStart()
    {
        CutSceneManager.Instance.PlayCutScene(
            Defines.CutSceneDefines.CutSceneNumber.Intro,
            GameManager.Instance.NewGameStart);
    }

    private void LoadCredit()
    {
        SceneLoadManager.Instance.LoadScene(SceneDefines.Scene.Credit);
    }

    private void GameExit()
    {
        GameManager.Instance.GameExit(false);
    }
}
