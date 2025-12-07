using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class CheckpointGimmick : GimmickBase<CheckpointData>
{

    [field: SerializeField, Range(0, 100)] public int checkPointNumber { get; set; } = 0;
    [Header("Checkpoint Area")]
    /// <summary> 체크 포인트 영역 </summary>
    [SerializeField] private Transform checkpointArea;
    /// <summary> 체크 포인트 영역을 나타내는 박스 콜라이더 </summary>
    [SerializeField] private BoxCollider boxCollider;
    /// <summary> 플레이어 리스폰 위치를 나타내는 임시 오브젝트 </summary>
    [SerializeField] private GameObject cubeArea;

    [Header("Respawn point")]
    /// <summary> 플레이어 리스폰 위치 </summary>
    [SerializeField] private Transform respawnPoint;
    /// <summary> 플레이어 리스폰 위치를 나타내는 임시 오브젝트 </summary>
    [SerializeField] private GameObject prevRespawnPoint;
    /// <summary> 배치된 영역 오브젝트 </summary>
    [SerializeField] private GameObject placedArea;

    [Header("FallTime")]
    [SerializeField] private float fallLimitTime = 5f;
    private float fallTimer = 0f;

    [Header("Player Data")]
    /// <summary> 플레이어 레이어마스크 </summary>
    [SerializeField] private LayerMask playerMask;

    /// <summary> 플레이어 스테이터스 </summary>
    private PlayerHp playerHp;
    private PlayerMove move;

    [SerializeField] private AudioSource audioSource;
    private const string RespawnPointName = "RespawnPoint";

    private int myIndex = -1;
    private bool isActive = false;
    private bool isVisited = false;
    private bool isWaitingToRespawn = false;

    [SerializeField] private DialogueGimmick[] ConnectedDialogue;
    [SerializeField] private TutorialStartTrigger[] ConnectedTutorial;
    [SerializeField] private UnityEvent OnCheckPointLoad;

    public Vector3 RespawanPosition { get; private set; }
    public Vector3 RespawnRotation { get; private set; }

    protected override void Init()
    {
        myIndex = CheckpointGimmick.AddCheckpoint(this, checkPointNumber);

        boxCollider.center = new Vector3(0f, gimmickData.AreaSize.y / 2f, 0f);
        boxCollider.size = gimmickData.AreaSize;

        cubeArea.SetActive(false);
        prevRespawnPoint.SetActive(false);
#if UNITY_EDITOR
        cubeArea.SetActive(true);
        prevRespawnPoint.SetActive(true);
#endif

        if (GameManager.Instance.GetCurrentCheckPointNumber() == myIndex)
        {
            Debug.Log($"{GameManager.Instance.GetCurrentCheckPointNumber()} CheckPointSet");
            CheckPointLoad();
        }

        SetGimmick();
    }

    private void CheckPointLoad()
    {
        if (ConnectedDialogue != null)
        {
            foreach (var checkDialogue in ConnectedDialogue)
            {
                checkDialogue.gameObject.SetActive(false);
            }
        }

        if (ConnectedTutorial != null)
        {
            foreach(var checkPoint in ConnectedTutorial)
            {
                checkPoint.gameObject.SetActive(false);
            }
        }

        OnCheckPointLoad?.Invoke();
        isVisited = true;
    }

    private void Start()
    {
        move = GameManager.Instance.Player.GetComponent<PlayerMove>();
    }

    public override void SetGimmick()
    {
        // 체크 포인트 크기 및 위치 설정
        //checkpointArea.localScale = gimmickData.AreaSize;
        respawnPoint.position = gimmickData.DictPoint["RespawnPoint"].position;
        respawnPoint.localRotation = Quaternion.Euler(gimmickData.RespawnRotation);

        RespawanPosition = respawnPoint.position;
        RespawnRotation = respawnPoint.rotation.eulerAngles;

        // 박스 콜라이더 크기 및 위치 설정
        boxCollider.center = new Vector3(0f, gimmickData.AreaSize.y / 2f, 0f);
        boxCollider.size = gimmickData.AreaSize;

        placedArea.transform.localScale = gimmickData.AreaSize;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Transform parent = other.transform.parent.parent;
        //if (parent != null && parent.TryGetComponent<PlayerHp>(out var hp))
        if (other.TryGetComponent<PlayerHp>(out var hp))
        {
            if (playerHp == null)
            {
                playerHp = hp;
            }

            CheckpointGimmick.SetCheckpoint(myIndex);

            playerHp.RespawnPoint = respawnPoint.position;
            playerHp.RespawnRotation = respawnPoint.rotation.eulerAngles;
            isActive = true;

            if(gimmickData.FullHealWhenFirstTouched == true && isVisited == false)
            {
                audioSource.Play();
                playerHp.FullHeal();
                GameManager.Instance.SaveData();
                isVisited = true;
            }
        }
    }

    private void Update()
    {
        if (isActive == false)
        {
            return;
        }

        bool grounded = move != null && move.IsGrounded;

        if(grounded == false)
        {
            fallTimer += Time.deltaTime;

            if(fallTimer >= fallLimitTime && isWaitingToRespawn == false)
            {
                Debug.Log($"{myIndex} {isWaitingToRespawn} {fallTimer}");
                playerHp.Fall(gimmickData.DropDamage);
                isWaitingToRespawn = true;
                fallTimer = 0f;
            }
        }
        else
        {
            isWaitingToRespawn = false;
            fallTimer = 0f;
        }
    }

    private void CheckPointDisable()
    {
        isActive = false;
    }

    protected void LoadCheckpoint()
    {
        SetCheckpoint(myIndex);
        isVisited = true;

        playerHp = GameManager.Instance.Player.GetComponent<PlayerHp>();
        playerHp.RespawnPoint = respawnPoint.position;
        playerHp.RespawnRotation = respawnPoint.rotation.eulerAngles;
        playerHp.Respawn();
    }
}

public partial class CheckpointGimmick : GimmickBase<CheckpointData>
{
    public static Dictionary<int, CheckpointGimmick> CheckpointList { get; private set; } = new Dictionary<int, CheckpointGimmick>();
    public static int CurrentCheckpointIndex { get; private set; } = -1;

    private static int AddCheckpoint(CheckpointGimmick _checkPoint, int number)
    {
        CheckpointList[number + (int)SceneLoadManager.Instance.GetActiveStage() * 100] = _checkPoint;
        return number + (int)SceneLoadManager.Instance.GetActiveStage() * 100;
    }

    private static void SetCheckpoint(int _index)
    {
        if (CurrentCheckpointIndex == _index)
            return;

        if (CurrentCheckpointIndex >= 0 && CheckpointList.ContainsKey(CurrentCheckpointIndex) == true)
        {
            CheckpointList[CurrentCheckpointIndex].CheckPointDisable();
        }

        CurrentCheckpointIndex = _index;
    }

    public static void LoadCheckpoint(int _index)
    {
        CurrentCheckpointIndex = _index;
        CheckpointList[CurrentCheckpointIndex].LoadCheckpoint();
    }
}
