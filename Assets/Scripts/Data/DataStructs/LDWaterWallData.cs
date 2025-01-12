using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDWaterWallData : LDGimmickDataBase
{
    [GimmickData("블록 메테리얼")]
    [field: SerializeField]
    public Material[] blockMaterials;
}
