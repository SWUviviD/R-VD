using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDBalancePlateData : LDGimmickDataBase
    {
        [MemoryPackInclude]
        public float Radius { get; set; } = 10f;

        [MemoryPackInclude]
        public float ReturnToNormalTime { get; set; } = 5f;

        [MemoryPackInclude]
        public float Level1_Roate { get; set; }

        [MemoryPackInclude]
        public float Level2_Roate { get; set; }

        [MemoryPackInclude]
        public float Level3_Roate { get; set; }

    }
}