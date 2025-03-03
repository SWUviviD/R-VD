using Defines;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [field: SerializeField] public GameObject Player { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsStageClear { get; private set; }

    [SerializeField] public GameObject clearEffectPrefab1;
    [SerializeField] public GameObject clearEffectPrefab2;
    [SerializeField] private AudioSource backgroundSFX;

    public GameData GameData { get; private set; } = new GameData();
    private string savedDataPath;
    private bool isInit = false;

    private void Awake()
    {
        savedDataPath = Application.persistentDataPath + "/save";
        FileInfo fileInfo = new FileInfo(savedDataPath);
        if (fileInfo.Exists)
        {
            GameData = JsonUtility.FromJson<GameData>(File.ReadAllText(savedDataPath));
        }
        else
        {
            GameData.stageID = 0;
            GameData.checkpointID = -1;
            GameData.playerHealth = 10;
            File.WriteAllText(savedDataPath, JsonUtility.ToJson(GameData));
        }

        if (Player != null)
        {
            Player.GetComponent<PlayerHp>().OnDeath.RemoveListener(OnGameOver);
            Player.GetComponent<PlayerHp>().OnDeath.AddListener(OnGameOver);
        }
    }

    private void OnEnable()
    {
        if (isInit == true)
            return;

#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name != "LevelEditor" &&
            SceneManager.GetActiveScene().name != "TitleScene" &&
            SceneManager.GetActiveScene().name != "GimmickTest" &&
            SceneManager.GetActiveScene().name != "kyh")
        {
#endif
            // 맵을 로드한다.
            //Player = MapLoadManager.Instance.LoadMap("map");
            //LDMapData mapData = MapLoadManager.Instance.MapData;

#if UNITY_EDITOR
        }
#endif

        if (Player != null)
        {
            Player.GetComponent<PlayerHp>().OnDeath.RemoveListener(OnGameOver);
            Player.GetComponent<PlayerHp>().OnDeath.AddListener(OnGameOver);
            OnGameStart();
        }

        isInit = true;
        ResumeGame();
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

    public void NewGameStart()
    {
        IsPaused = false;
        IsGameOver = false;
        IsStageClear = false;

        GameData.stageID = 0;
        GameData.checkpointID = -1;
        GameData.playerHealth = 10;
        File.WriteAllText(savedDataPath, JsonUtility.ToJson(GameData));

        SceneManager.LoadScene("Stage1");
    }

    public void GameRestart()
    {
        IsPaused = false;
        IsGameOver = false;
        IsStageClear = false;

        LoadData();
    }

    public void SaveData()
    {
        // TODO: 스테이지, 체크포인트, 현재 체력 검사 필요
        GameData.stageID = SceneManager.GetActiveScene().buildIndex;
        GameData.checkpointID = CheckpointGimmick.CurrentCheckpointIndex;
        GameData.playerHealth = Player ? Player.GetComponent<PlayerHp>().CurrentHp : 10;

        // 스테이지, 체크포인트, 현재 체력 저장
        File.WriteAllText(savedDataPath, JsonUtility.ToJson(GameData));
    }

    public void LoadData()
    {
        // 스테이지, 체크포인트, 현재 체력 불러오기
        GameData = JsonUtility.FromJson<GameData>(File.ReadAllText(savedDataPath));

        // 게임 시작 및 씬 로드
        if (GameData.stageID != SceneManager.GetActiveScene().buildIndex)
        {
            SceneManager.LoadScene(GameData.stageID);
        }

        if (GameData.checkpointID != -1)
        {
            StartCoroutine(IWaitCheckpoint());
        }
    }

    private IEnumerator IWaitCheckpoint()
    {
        while (CheckpointGimmick.CheckpointList.Count == 0)
        {
            yield return null;
        }
        CheckpointGimmick.LoadCheckpoint(GameData.checkpointID);
        Player.GetComponent<PlayerHp>().SetHealth(GameData.playerHealth);
        yield return null;
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
