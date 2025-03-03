using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Defines;
using LocalData;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject Player { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsStageClear { get; private set; }

    [SerializeField] public GameObject clearEffectPrefab1;
    [SerializeField] public GameObject clearEffectPrefab2;
    [SerializeField] private AudioSource backgroundSFX;

    private GameData gameData = new GameData();
    private int stageIndex = 0;
    private string savedDataPath;
    private bool isInit = false;

    private void OnEnable()
    {
        if (isInit == true)
            return;

        savedDataPath = Application.persistentDataPath + "/save";

#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name != "LevelEditor" &&
            SceneManager.GetActiveScene().name != "TitleScene" &&
            SceneManager.GetActiveScene().name != "GimmickTest" &&
            SceneManager.GetActiveScene().name != "kyh")
        {
#endif
            // 맵을 로드한다.
            Player = MapLoadManager.Instance.LoadMap("map");
            LDMapData mapData = MapLoadManager.Instance.MapData;

            if (Player != null)
            {
                Player.GetComponent<PlayerHp>().OnDeath.RemoveListener(OnGameOver);
                Player.GetComponent<PlayerHp>().OnDeath.AddListener(OnGameOver);
            }

            OnGameStart();
#if UNITY_EDITOR
        }
#endif

        ResumeGame();

        isInit = true;
    }

    public void OnGameStart()
    {
        SetMovementInput(true);
    }

    private void OnGameOver()
    {
        GameOver();
        SetMovementInput(false);
    }

    public void SetMovementInput(bool active)
    {
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Move),
            active);
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Jump),
            active);
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Dash),
            active);
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.SkillType.StarHunt.ToString()),
            active);
    }

    public void GameClear()
    {
        // 추가 동작 필요시 구현
        StageClear();

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
        if (IsGameOver || IsStageClear) return;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        IsPaused = true;
        Time.timeScale = 0f;
        if (backgroundSFX != null)
        {
            backgroundSFX.Pause();
        }
    }

    public void ResumeGame()
    {
        if (IsGameOver || IsStageClear) return;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        IsPaused = false;
        Time.timeScale = 1f;
        if (backgroundSFX != null)
        {
            backgroundSFX.Play();
        }
    }

    public void GameStart()
    {
        IsPaused = false;
        IsGameOver = false;
        IsStageClear = false;
        // TODO: 1스테이지 처음으로 시작 필요
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameRestart()
    {
        IsPaused = false;
        IsGameOver = false;
        IsStageClear = false;
        // TODO: 스테이지 인덱스로 수정 필요
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveData()
    {
        // TODO: 스테이지, 체크포인트, 현재 체력 검사 필요
        gameData.stageID = stageIndex;
        gameData.checkpointID = CheckpointGimmick.CurrentCheckpointIndex;
        gameData.playerHealth = Player.GetComponent<PlayerHp>().CurrentHp;

        // 스테이지, 체크포인트, 현재 체력 저장
        File.WriteAllText(savedDataPath, JsonUtility.ToJson(gameData));
    }

    public void LoadData()
    {
        // 스테이지, 체크포인트, 현재 체력 불러오기
        gameData = JsonUtility.FromJson<GameData>(File.ReadAllText(savedDataPath));

        // 게임 시작 및 씬 로드
        OnGameStart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // TODO: 스테이지 변경 테스트 필요
        //SceneManager.LoadScene(gameData.stageID);
        CheckpointGimmick.LoadCheckpoint(gameData.checkpointID);
    }

    public void GameExit()
    {
        SaveData();
        Application.Quit();
    }

    public void GameOver()
    {
        IsGameOver = true;
    }

    public void StageClear()
    {
        IsPaused = false;
        IsStageClear = true;
    }
}

public class GameData
{
    public int stageID;
    public int checkpointID;
    public int playerHealth;
}
