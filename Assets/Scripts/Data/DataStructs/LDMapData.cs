using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDMapData : DataBase
    {
        [MemoryPackInclude] public List<LDBlinkBoardData> BlinkBoardDataList { get; set; }
    }
}