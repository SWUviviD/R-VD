using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDBlueCristalGimmickData : LDGimmickDataBase
    {
        [MemoryPackInclude]
        public int CristalType { get; set; }

        [MemoryPackInclude]
        public float BlinkShowTime { get; set; } = 3f;

        [MemoryPackInclude]
        public float BlinkHideTime { get; set; } = 3f;

        [MemoryPackInclude]
        public float MoveMoveTime { get; set; } = 3f;

        [MemoryPackInclude]
        public float ChangeValueTime { get; set; } = 10f;


        [MemoryPackInclude]
        public float RandomMinValue { get; set; } = 1f;


        [MemoryPackInclude]
        public float RandomMaxValue { get; set; } = 3f;


        [MemoryPackInclude]
        public float SphereSize { get; set; } = 10f;

        [MemoryPackInclude]
        public float SphereMoveTime { get; set; } = 16f;

        [MemoryPackInclude]
        public float SphereRotateSpeed { get; set; } = 180f;


        [MemoryPackInclude]
        public float SphereStopTime { get; set; } = 30f;

    }
}