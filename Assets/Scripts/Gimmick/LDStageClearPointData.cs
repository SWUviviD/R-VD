using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDStageClearPointData : LDGimmickDataBase
    {
        [MemoryPackInclude]
        public Vector3 AreaScale { get; set; }
    }

}
