using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class DataBase
    {
        [MemoryPackInclude] public int ID { get; set; }
        [MemoryPackInclude] public string ID_str { get; set; }
    }
}
