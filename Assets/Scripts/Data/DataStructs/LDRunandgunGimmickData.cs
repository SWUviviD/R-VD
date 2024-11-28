using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]

    public partial class LDRunandgunGimmickData : LDGimmickDataBase
    {
        [MemoryPackInclude]
        public int DamageAmount { get; set; } = 1; // 데미지 존에서 틱당 감소되는 체력 양

        [MemoryPackInclude]
        public float HealAmount { get; set; } = 100f; // 힐 존에서 회복되는 체력 양

        [MemoryPackInclude]
        public float DamageTickInterval { get; set; } = 2f; // 데미지 틱 간격 시간 (초)
    }
}
