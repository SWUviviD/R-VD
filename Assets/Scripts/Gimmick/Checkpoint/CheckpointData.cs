using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cristal;

public class CheckpointData : GimmickDataBase
{
    [GimmickData("자동 저장 영역")]
    [field: SerializeField]
    public Vector3 AreaSize { get; set; } = new Vector3(5f, 3f, 5f);

    [GimmickData("플레이어 리스폰 위치")]
    [field: SerializeField]
    public Vector3 RespawnPoint { get; set; } = Vector3.zero;

    [GimmickData("플레이어 리스폰 방향")]
    [field: SerializeField]
    public Vector3 RespawnRotation { get; set; } = Vector3.zero;

    [GimmickData("추락 높이 (y좌표 값 0 기준)")]
    [field: SerializeField]
    public float DropRespawnHeight { get; set; } = -10f;

    [GimmickData("추락 시 플레이어가 받는 데미지")]
    [field: SerializeField]
    public int DropDamage { get; set; } = 2;

    [GimmickData("추락 시 플레이어가 받는 데미지")]
    [field: SerializeField]
    public bool FullHealWhenFirstTouched{ get; set; } = true;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        base.SaveGimmickData(_mapData);

        var sdCheckpointData = new LDCheckpointData();

        sdCheckpointData.Position = trGimmick.position;
        sdCheckpointData.Rotation = trGimmick.rotation.eulerAngles;
        sdCheckpointData.Scale = trGimmick.localScale;
        sdCheckpointData.Address = address;

        sdCheckpointData.AreaSize = AreaSize;
        sdCheckpointData.RespawnPoint = RespawnPoint;
        sdCheckpointData.RespawnRotation = RespawnRotation;
        sdCheckpointData.DropRespawnHeight = DropRespawnHeight;
        sdCheckpointData.DropDamage = DropDamage;
        sdCheckpointData.FullHealWhenFirstTouched = FullHealWhenFirstTouched;

        _mapData.CheckpointList.Add(sdCheckpointData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var ldCheckpointData = (LDCheckpointData) _ldData;

        trGimmick.position = ldCheckpointData.Position;
        trGimmick.rotation = Quaternion.Euler(ldCheckpointData.Rotation);
        trGimmick.localScale = ldCheckpointData.Scale;

        AreaSize = ldCheckpointData.AreaSize;
        RespawnPoint = ldCheckpointData.RespawnPoint;
        RespawnRotation = ldCheckpointData.RespawnRotation;
        DropRespawnHeight = ldCheckpointData.DropRespawnHeight;
        DropDamage = ldCheckpointData.DropDamage;
        FullHealWhenFirstTouched = ldCheckpointData.FullHealWhenFirstTouched;
    }
}
