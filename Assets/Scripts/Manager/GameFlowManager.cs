using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public enum GameState
    {
        CutScene,
        WaitingClick,
        Playing
    }

    public GameState currentState;

    private void Start()
    {
        CutSceneManager.Instance.PlayCutScene(0, OnStartCutSceneFinished);
    }

    private void OnStartCutSceneFinished()
    {
        // GameManager.Instance.SetMovementInput(true);
        currentState = GameState.WaitingClick;
    }

    private void Update()
    {
        if (currentState == GameState.WaitingClick && Input.GetMouseButtonDown(0))
        {
            currentState = GameState.CutScene;
            Finish();
        }
    }

    private void StartCutScene(int cutSceneNumber)
    {
        currentState = GameState.CutScene;
        CutSceneManager.Instance.PlayCutScene(cutSceneNumber, OnEndCutSceneFinished);
    }

    public void Finish()
    {
        GameManager.Instance.SetMovementInput(false);
        CutSceneManager.Instance.PlayCutScene(2, OnEndCutSceneFinished);
    }

    private void OnEndCutSceneFinished()
    {
        RestartGame();
    }

    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }
}
