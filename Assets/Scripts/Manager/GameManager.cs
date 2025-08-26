using Defines;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [field:SerializeField] public Transform Camera { get; private set; }
    [field: SerializeField] public GameObject Player { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsStageClear { get; private set; }
    public bool IsLastScene => SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1;

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
            GameData.playerHealth = 10;
            GameData.playerPosition = Vector3.zero;
            GameData.playerRotation = Vector3.zero;
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
        if (SceneManager.GetActiveScene().name != "TitleScene" && 
            SceneManager.GetActiveScene().name != "LevelEditor" &&
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

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (Player != null)
        {
            Player.GetComponent<PlayerHp>().OnDeath.RemoveListener(OnGameOver);
            Player.GetComponent<PlayerHp>().OnDeath.AddListener(OnGameOver);
            OnGameStart();
        }

        isInit = true;
        ResumeGame();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "TitleScene") return;
        if (SceneManager.GetActiveScene().buildIndex != GameData.stageID) return;

        Player = GameObject.FindWithTag("Player");
        //Player.transform.position = GameData.playerPosition;
        //Player.transform.rotation = Quaternion.Euler(GameData.playerRotation);
        Player.GetComponent<PlayerHp>().SetHealth(GameData.playerHealth);

        //CameraController cameraController = Camera.main.GetComponent<CameraController>();
        //cameraController.Respawn(GameData.playerPosition, GameData.playerRotation);
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
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.SkillType.StarFusion.ToString()),
            active);
    }

    public void SetCameraInput(bool active)
    {
        InputManager.Instance.EnableAction(
            new Defines.InputDefines.InputActionName(
                InputDefines.ActionMapType.PlayerActions,
                InputDefines.Camera),
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
        GameData.playerHealth = 10;
        GameData.playerPosition = Vector3.zero;
        GameData.playerRotation = Vector3.zero;

        File.WriteAllText(savedDataPath, JsonUtility.ToJson(GameData));

        SceneManager.LoadScene("Stage1");
    }

    public void GameRestart()
    {
        IsPaused = false;
        IsGameOver = false;
        IsStageClear = false;
        SetMovementInput(false);

        LoadData();
    }

    public void SaveData()
    {
        // TODO: 스테이지, 체크포인트, 현재 체력 검사 필요
        GameData.stageID = SceneManager.GetActiveScene().buildIndex;
        GameData.playerHealth = Player ? Player.GetComponent<PlayerHp>().CurrentHp : 10;
        GameData.playerPosition = Player ? Player.GetComponent<PlayerHp>().RespawnPoint : Vector3.zero;
        GameData.playerRotation = Player ? Player.GetComponent<PlayerHp>().RespawnRotation : Vector3.zero;

        // 스테이지, 체크포인트, 현재 체력 저장
        File.WriteAllText(savedDataPath, JsonUtility.ToJson(GameData));
    }

    public void LoadData()
    {
        // 스테이지, 체크포인트, 현재 체력 불러오기
        GameData = JsonUtility.FromJson<GameData>(File.ReadAllText(savedDataPath));

        SceneManager.LoadScene(GameData.stageID);
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

    public void NextStage()
    {
        SetMovementInput(false);

        GameData.stageID = SceneManager.GetActiveScene().buildIndex + 1;
        GameData.playerHealth = Player ? Player.GetComponent<PlayerHp>().CurrentHp : 10;
        GameData.playerPosition = Vector3.zero;
        GameData.playerRotation = Vector3.zero;
        File.WriteAllText(savedDataPath, JsonUtility.ToJson(GameData));

        SceneManager.LoadScene(GameData.stageID);
    }

    public void LoadTitle()
    {
        SetMovementInput(false);
        SceneManager.LoadScene(0);
    }
}

public class GameData
{
    public int stageID;
    public int playerHealth;
    public Vector3 playerPosition;
    public Vector3 playerRotation;
}
