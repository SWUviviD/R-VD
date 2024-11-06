using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDBlinkBoardData : LDGimmickDataBase
    {
        /// <summary> 빙판의 개수 </summary>
        [MemoryPackInclude] public int BoardCount { get; set; }
        /// <summary> 빙판의 크기 </summary>
        [MemoryPackInclude] public float BoardSize { get; set; }
        /// <summary> 사라졌다가 다시 돌아오는데 걸리는 시간 </summary>
        [MemoryPackInclude] public float BlinkTime { get; set; }
        /// <summary> 빙판이 유지되는 시간 </summary>
        [MemoryPackInclude] public float DurationTime { get; set; }
        /// <summary> 다음 빙판이 사라지는 텀 </summary>
        [MemoryPackInclude] public float NextBlinkTime { get; set; }
    }
}