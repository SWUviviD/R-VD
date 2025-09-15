using Defines;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.CanvasScaler;

public class GameManager : MonoSingleton<GameManager>
{
    [field:SerializeField] public GameObject Camera { get; private set; }
    [field: SerializeField] public GameObject Player { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsStageClear { get; private set; }
    public bool IsLastScene => SceneLoadManager.Instance.GetActiveScene()
        == SceneDefines.Scene.MAX - 1;

    [SerializeField] public GameObject clearEffectPrefab1;
    [SerializeField] public GameObject clearEffectPrefab2;
    [SerializeField] private AudioSource backgroundSFX;

    public GameDataManager GameDataManager { get; private set; }

    private bool isInit = false;

    protected override void Init()
    {
        GameDataManager = new GameDataManager();
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if(isInit == false)
        {
            Player = GameObject.FindWithTag("Player");
            if (Player != null)
            {
                Player.GetComponent<PlayerHp>().OnDeath.RemoveListener(OnGameOver);
                Player.GetComponent<PlayerHp>().OnDeath.AddListener(OnGameOver);
                OnGameStart();
            }
            isInit = true;
        }
#endif
        //ResumeGame();
    }

    public int GetCurrentCheckPointNumber()
    {
        return GameDataManager.GameData.CheckPointID;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Player = GameObject.FindWithTag("Player");
        if (Player != null)
        {
            PlayerHp hp = Player.GetComponent<PlayerHp>();
            hp.OnDeath.RemoveListener(OnGameOver);
            hp.OnDeath.AddListener(OnGameOver);
            hp.SetHealth(GameDataManager.GameData.PlayerHealth);

            PlayerMove move = Player.GetComponent<PlayerMove>();
            move.SetPosition(GameDataManager.GameData.PlayerPosition);
            move.SetRotation(GameDataManager.GameData.PlayerRotation);
        }

        // todo 밖으로 빼기
        LevitateAroundPlayer siro = GameObject.FindAnyObjectByType<LevitateAroundPlayer>();
        if (siro != null)
        {
            int id = GameDataManager.GameData.CheckPointID;
            if (id/ 100 == 1 && id % 100 > 0)
            {
                siro.SetTargetPlayer(Player.transform);
            }
        }

        Camera = FindFirstObjectByType<OrbitCamera>()?.gameObject;
        if (Camera != null)
        {
            CameraController.Instance.OnLoadCameraSetting(Camera.transform);
            CameraController.Instance.SetCameraMode(CameraController.CameraMode.Orbit);
            CameraController.Instance.SetCameraPositionAndRotation(GameDataManager.GameData.camRotation, Vector3.zero);
        }

        SetMovementInput(true);
        ResumeGame();
        SetCameraInput(true);

    }

    public void OnGameStart()
    {
        SetMovementInput(true);
    }

    private void OnGameOver()
    {
        GameOver();
        SetMovementInput(false);
        SetCameraInput(false);
        StopGame();
    }

    public void ShowCursor(bool isShow = true)
    {
        Cursor.lockState = isShow ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isShow;
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (IsGameOver || IsStageClear) return;

        GameManager.Instance.SetCameraInput(false);

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

        GameManager.Instance.SetCameraInput(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;

        IsPaused = false;
        Time.timeScale = 1f;
        if (backgroundSFX != null)
        {
            backgroundSFX.Play();
        }
    }

    public void NewGameStart()
    {
        GameDataManager.ResetGameData();

        SceneLoadManager.Instance.LoadScene(SceneDefines.Scene.Stage1, true, OnSceneLoaded);
    }

    public void GameRestart()
    {
        IsPaused = false;
        IsGameOver = false;
        IsStageClear = false;
        SetMovementInput(false);

        ResetStageGameData();

        LoadData();
    }

    public void ResetStageGameData()
    {
        StageID stage = SceneLoadManager.Instance.GetActiveStage();
        GameDataManager.SaveGameData(
            stage, (int)stage * 100, 10,
            Vector3.zero, Vector3.zero, Vector3.right * 180f,
            (int)stage > 1, (int)stage > 2, (int)stage > 3);
    }

    public void SaveData()
    {
        PlayerHp playerHp = Player.GetComponent<PlayerHp>();
        SkillSwap skillSwap = Player.GetComponent<SkillSwap>();

        // 스테이지, 체크포인트, 현재 체력 저장
        // TODO: 스테이지, 체크포인트, 현재 체력 검사 필요
        GameDataManager.SaveGameData(
            SceneLoadManager.Instance.GetActiveStage(),
            CheckpointGimmick.CurrentCheckpointIndex,
            Player ? playerHp.CurrentHp : 10,
            Player ? playerHp.RespawnPoint : Vector3.zero,
            Player ? playerHp.RespawnRotation : Vector3.zero,
            CheckpointGimmick.CheckpointList.ContainsKey(CheckpointGimmick.CurrentCheckpointIndex) == true ?
                CheckpointGimmick.CheckpointList[CheckpointGimmick.CurrentCheckpointIndex].GimmickData.CamRotation :
                Vector3.right * 180f,
            skillSwap.SkillUnlocked[(int)Defines.InputDefines.SkillType.StarHunt],
            skillSwap.SkillUnlocked[(int)Defines.InputDefines.SkillType.StarFusion],
            skillSwap.SkillUnlocked[(int)Defines.InputDefines.SkillType.WaterVase]);
    }

    public void LoadData()
    {
        SceneLoadManager.Instance.LoadScene(GameDataManager.GameData.StageID, true, OnSceneLoaded);
    }

    public void GameExit(bool saveFile = true)
    {
        if(saveFile)
        {
            SaveData();
        }
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

        PlayerHp playerHp = Player?.GetComponent<PlayerHp>();

        StageID stage = SceneLoadManager.Instance.GetActiveStage();
        GameDataManager.SaveGameData(
            SceneLoadManager.Instance.GetActiveStage() + 1,
            (int) SceneLoadManager.Instance.GetActiveStage() * 100,
            playerHp ? playerHp.CurrentHp : 10,
            Vector3.zero,
            Vector3.zero,
            Vector3.right * 180f,
            (int)stage > 1, (int)stage > 2, (int)stage > 3);

        // Todo. 엔딩일 경우 처리 필요
        if (IsLastScene)
        {
            //CutSceneManager.Instance.PlayCutScene(
            //    CutSceneDefines.CutSceneNumber.End,
            //    () => SceneLoadManager.Instance.LoadScene(SceneDefines.Scene.Title));
            SetMovementInput(false);

            // 게임 끝까지 완료한 경우 게임 데이터 완전 초기화(삭제) 필요
            GameDataManager.DeleteGameData();
            SceneLoadManager.Instance.LoadScene(SceneDefines.Scene.Title );
            ShowCursor(true);
            return;
        }

        SceneLoadManager.Instance.LoadScene(
            GameDataManager.GameData.StageID,
            true, OnSceneLoaded);
    }

    public void LoadTitle()
    {
        SetMovementInput(false);
        SceneLoadManager.Instance.LoadScene(SceneDefines.Scene.Title);
    }
}
