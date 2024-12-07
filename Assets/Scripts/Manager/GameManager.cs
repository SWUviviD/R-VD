using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using LocalData;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject Player { get; private set; }
    public bool IsGamePlaying { get; private set; }

    [SerializeField] public GameObject clearEffectPrefab1;
    [SerializeField] public GameObject clearEffectPrefab2;
    [SerializeField] private AudioSource backgroundSFX;

    private void Awake()
    {
        // 맵을 로드한다.
        Player = MapLoadManager.Instance.LoadMap("map");
        LDMapData mapData = MapLoadManager.Instance.MapData;

        if (Player != null)
        {
            var cameraController = CameraController.Instance;
            // 카메라가 플레이어를 따라가도록 한다.
            cameraController.SetPlayer(Player.transform);
            // 카메라 경로를 넣고 동작시킨다.
            cameraController.Set(mapData.CameraPathList.ConvertAll(_ => _.ToCameraPathPoint()));
            cameraController.Play();

            Player.GetComponent<PlayerHp>().OnDeath.RemoveListener(OnGameOver);
            Player.GetComponent<PlayerHp>().OnDeath.AddListener(OnGameOver);
        }

        OnGameStart();
        ResumeGame();
    }

    public void OnGameStart()
    {
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Move),
            true);
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Jump),
            true);
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Dash),
            true);
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Magic),
            true);
    }

    private void OnGameOver()
    {
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Move),
            false);
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Jump),
            false);
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Dash),
            false);
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Magic),
            false);
    }

    public void GameClear()
    {
        // 추가 동작 필요시 구현

        // 이펙트 출력
        if (clearEffectPrefab1 != null && clearEffectPrefab2 != null)
        {
            GameObject clearEffect1 = Instantiate(clearEffectPrefab1, Player.transform.position, Quaternion.identity);
            GameObject clearEffect2 = Instantiate(clearEffectPrefab1, Player.transform.position, Quaternion.identity);

            clearEffect1.transform.SetParent(Player.transform);
            clearEffect2.transform.SetParent(Player.transform);

            Destroy(clearEffect1, 10f);
            Destroy(clearEffect2, 13f);
        }
    }

    public void StopGame()
    {
        IsGamePlaying = true;
        Time.timeScale = 0f;
        if (backgroundSFX != null)
        {
            backgroundSFX.Pause();
        }
    }

    public void ResumeGame()
    {
        IsGamePlaying = false;
        Time.timeScale = 1f;
        if (backgroundSFX != null)
        {
            backgroundSFX.Play();
        }
    }
}
