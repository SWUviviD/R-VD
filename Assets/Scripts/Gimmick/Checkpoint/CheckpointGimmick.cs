using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointGimmick : GimmickBase<CheckpointData>
{
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

    [Header("Player Data")]
    /// <summary> 플레이어 레이어마스크 </summary>
    [SerializeField] private LayerMask playerMask;
    /// <summary> 플레이어 스테이터스 </summary>
    private PlayerStatus playerStatus;

    private const string RespawnPointName = "RespawnPoint";

    protected override void Init()
    {
        boxCollider.center = new Vector3(0f, gimmickData.AreaSize.y / 2f, 0f);
        boxCollider.size = gimmickData.AreaSize;

        cubeArea.SetActive(false);
        prevRespawnPoint.SetActive(false);
#if UNITY_EDITOR
        cubeArea.SetActive(true);
        prevRespawnPoint.SetActive(true);
#endif
    }

    public override void SetGimmick()
    {
        // 체크 포인트 크기 및 위치 설정
        checkpointArea.localScale = gimmickData.AreaSize;
        respawnPoint.localPosition = gimmickData.RespawnPoint;

        // 박스 콜라이더 크기 및 위치 설정
        boxCollider.center = new Vector3(0f, gimmickData.AreaSize.y / 2f, 0f);
        boxCollider.size = gimmickData.AreaSize;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerMask.value == (1 << other.gameObject.layer))
        {
            if (playerStatus == null)
            {
                playerStatus = other.GetComponentInParent<PlayerStatus>();
                playerStatus.SetRespawnPoint(respawnPoint.position);
            }
        }
    }

    private void Update()
    {
        if (playerStatus == null)
        {
            return;
        }    

        // 플레이어 추락 시 데미지 및 리스폰 이동
        if (playerStatus.transform.position.y < gimmickData.DropRespawnHeight)
        {
            playerStatus.TakeDamage(gimmickData.DropDamage);
            playerStatus.transform.position = respawnPoint.position;
        }
    }
}
