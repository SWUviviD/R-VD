using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]

    public partial class LDGalaxyGimmickData : LDGimmickDataBase
    {
        [MemoryPackInclude]
        public float VisibleDuration { get; set; } = 4f;

        [MemoryPackInclude]
        public float KnockbackForce { get; set; } = 120f;

        [MemoryPackInclude]
        public float Damage { get; set; } = 20f;
    }
}
