using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDGreenCristalGimmickData : LDGimmickDataBase
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
        public int PlateCount { get; set; } = 3;

        [MemoryPackInclude]
        public float GimmickAppearTime { get; set; } = 0.3f;

        [MemoryPackInclude]
        public float GimmickShowTime { get; set; } = 3f;

        [MemoryPackInclude]
        public float NextPlateShowTime { get; set; } = 2f;

        [MemoryPackInclude]
        public float GimmickDissappearTime { get; set; } = 0.3f;

        [MemoryPackInclude]
        public float GimmickHideTime { get; set; } = 3f;

    }
}

