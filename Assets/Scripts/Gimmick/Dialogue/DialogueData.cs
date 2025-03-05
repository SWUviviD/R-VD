using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueData : GimmickDataBase
{
    [GimmickData("대화 내용 ID")]
    [field: SerializeField]
    public int DialogueID { get; set; } = -1;

    [GimmickData("카메라 Position")]
    [field: SerializeField]
    public Vector3 CameraPosition { get; set; } = Vector3.zero;

    [GimmickData("카메라 Rotation")]
    [field: SerializeField]
    public Vector3 CameraRotation { get; set; } = Vector3.zero;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        base.SaveGimmickData(_mapData);

        var sdDialogueData = new LDDialogueData();

        sdDialogueData.DialogueID = DialogueID;

        _mapData.DialogueList.Add(sdDialogueData);

    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var ldDialogueData = (LDDialogueData)_ldData;

        trGimmick.position = ldDialogueData.Position;
        trGimmick.rotation = Quaternion.Euler(ldDialogueData.Rotation);
        trGimmick.localScale = ldDialogueData.Scale;

        DialogueID = ldDialogueData.DialogueID;
    }
}
