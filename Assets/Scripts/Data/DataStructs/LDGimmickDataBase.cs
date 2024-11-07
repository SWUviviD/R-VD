using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDGimmickDataBase : DataBase
    {
        /// <summary> 기믹의 위치 </summary>
        [MemoryPackInclude] public Vector3 Position { get; set; }
        /// <summary> 기믹의 회전 </summary>
        [MemoryPackInclude] public Vector3 Rotation { get; set; }
        /// <summary> 기믹의 스케일 </summary>
        [MemoryPackInclude] public Vector3 Scale { get; set; }
        
        /// <summary>
        /// 기믹의 어드레스
        /// </summary>
        [MemoryPackInclude] public string Address { get; set; }
        
        /// <summary>
        /// 이름으로 접근할 수 있는 위치값.
        /// </summary>
        [MemoryPackInclude] public Dictionary<string, Vector3> DictPoint { get; set; }

        public LDGimmickDataBase()
        {
            DictPoint = new Dictionary<string, Vector3>();
        }
    }
}