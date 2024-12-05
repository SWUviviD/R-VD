using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDCristalData : LDGimmickDataBase
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
        public float PlateSize { get; set; } = 1f;
    }
}
