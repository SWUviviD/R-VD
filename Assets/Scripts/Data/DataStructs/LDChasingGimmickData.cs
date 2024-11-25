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
        public float StarFallSpeed { get; set; } = 4f;

        [MemoryPackInclude]
        public float ResponeTime { get; set; } = 2f;

        [MemoryPackInclude]
        public int TotalNum { get; set; } = 4;

        [MemoryPackInclude]
        public float Damage { get; set; } = 20;

        [MemoryPackInclude]
        public float KnockbackForce { get; set; } = 120f;

        [MemoryPackInclude]
        public List<ChasingStarProp> starListPrefab;
    }

}