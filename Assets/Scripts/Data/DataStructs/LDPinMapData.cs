using System;
using UnityEngine;
using MemoryPack;
using LocalData;
using System.Collections.Generic;

namespace StaticData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDPinMapData : DataBase
    {
        [MemoryPackInclude] public int Index { get; set; }
        [MemoryPackInclude] public int X { get; set; }
        [MemoryPackInclude] public int Y { get; set; }

        [MemoryPackInclude] public int Dir { get; set; }
        [MemoryPackInclude] public int Type { get; set; }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Dir: {Dir}, Type: {Type}";
        }
    }
}