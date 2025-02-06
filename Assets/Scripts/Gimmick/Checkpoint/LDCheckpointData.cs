using LocalData;
using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDCheckpointData : LDGimmickDataBase
    {
        [MemoryPackInclude]
        public Vector3 AreaSize { get; set; } = new Vector3(5f, 3f, 5f);

        [MemoryPackInclude]
        public Vector3 RespawnPoint { get; set; } = Vector3.zero;

        [MemoryPackInclude]
        public Vector3 RespawnRotation { get; set; } = Vector3.zero;

        [MemoryPackInclude]
        public float DropRespawnHeight { get; set; } = -10f;

        [MemoryPackInclude]
        public int DropDamage { get; set; } = 2;

        [MemoryPackInclude]
        public bool FullHealWhenFirstTouched { get; set; } = true;

    }

}