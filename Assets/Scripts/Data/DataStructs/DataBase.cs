using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticData
{
    [MemoryPackable]
    [Serializable]
    public partial class DataBase
    {
        [MemoryPackInclude] public int ID { get; set; }
        [MemoryPackInclude] public string ID_str { get; set; }
    }
}
