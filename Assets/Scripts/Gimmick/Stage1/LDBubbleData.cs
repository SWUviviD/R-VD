using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDBubbleData : LDGimmickDataBase
    {
        [MemoryPackInclude]
        public float PopOffsetTime;

        [MemoryPackInclude]
        public float ResetOffsetTime;

        [MemoryPackInclude]
        public float BubbleSize;

        [MemoryPackInclude]
        public float JumpForce = 150f;

    }

}

