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
        /// <summary>
        /// 이름으로 접근할 수 있는 위치값.
        /// </summary>
        [MemoryPackInclude] public Dictionary<string, Vector3> DictPoint;
    }
}