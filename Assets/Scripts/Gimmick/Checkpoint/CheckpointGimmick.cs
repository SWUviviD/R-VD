using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CheckpointGimmick : GimmickBase<CheckpointData>
{

    [SerializeField, Range(0, 100)] private int checkPointNumber = 0;
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
            isVisited = true;
        }

        SetGimmick();
    }

    public override void SetGimmick()
    {
        // 체크 포인트 크기 및 위치 설정
        //checkpointArea.localScale = gimmickData.AreaSize;
        respawnPoint.localPosition = gimmickData.RespawnPoint;
        respawnPoint.localRotation = Quaternion.Euler(gimmickData.RespawnRotation);

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
                move = other.GetComponentInParent <PlayerMove>();
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

        // 플레이어 추락 시 데미지 및 리스폰 이동
        if (playerHp.transform.position.y < gimmickData.DropRespawnHeight)
        {
            playerHp.Fall(gimmickData.DropDamage);
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

        if (CurrentCheckpointIndex >= 0 && CurrentCheckpointIndex < CheckpointList.Count)
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
