using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointData : GimmickDataBase
{
    [GimmickData("자동 저장 영역")]
    [field: SerializeField]
    public Vector3 AreaSize { get; set; } = new Vector3(5f, 3f, 5f);

    [GimmickData("플레이어 리스폰 위치")]
    [field: SerializeField]
    public Vector3 RespawnPoint { get; set; } = Vector3.zero;

    [GimmickData("추락 높이 (y좌표 값 0 기준)")]
    [field: SerializeField]
    public float DropRespawnHeight { get; set; } = -10f;

    [GimmickData("추락 시 플레이어가 받는 데미지")]
    [field: SerializeField]
    public int DropDamage { get; set; } = 2;
}
