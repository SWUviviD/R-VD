using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]

    public partial class LDChasingGimmickData : LDGimmickDataBase
    {
        [MemoryPackInclude]
        public Vector3 ChaseFloorScale { get; set; } = Vector3.one;


        [MemoryPackInclude]
        public int SmallStarCount { get; set; } = 3;

        [MemoryPackInclude]
        public float SmallStarStartDelay { get; set; } = 0.5f;

        [MemoryPackInclude]
        public float SmallStarFallingIntervalTime { get; set; } = 0.5f;

        [MemoryPackInclude]
        public float SmallStarCoolTime { get; set; } = 3f;



        [MemoryPackInclude]
        public int MediumStarCount { get; set; } = 2;

        [MemoryPackInclude]
        public float MediumStarStartDelay { get; set; } = 2.5f;

        [MemoryPackInclude]
        public float MediumStarFallingIntervalTime { get; set; } = 1f;

        [MemoryPackInclude]
        public float MediumStarCoolTime { get; set; } = 4f;



        [MemoryPackInclude]
        public int BigStarCount { get; set; } = 1;

        [MemoryPackInclude]
        public float BigStarStartDelay { get; set; } = 5f;

        [MemoryPackInclude]
        public float BigStarFallingIntervalTime { get; set; } = 0f;

        [MemoryPackInclude]
        public float BigStarCoolTime { get; set; } = 5f;
    }

}