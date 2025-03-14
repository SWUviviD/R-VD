using LocalData;
using MemoryPack;
using System;
using UnityEngine;

namespace StaticData
{
    [MemoryPackable]
    [Serializable]
    public partial class CutSceneInfo : DataBase
    {
        [MemoryPackInclude] public int Number { get; set; }
        [MemoryPackInclude] public string Image { get; set; }
        [MemoryPackInclude] public string Text { get; set; }

    }

}


