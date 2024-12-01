using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditObject : GimmickBase<LevelEditObjectData>
{
    [SerializeField] private GameObject editorObject;

    public override void SetGimmick()
    {
        //editorObject.SetActive(false);
    }

    protected override void Init()
    {
    }
}
