using System;
using UnityEngine;
using MemoryPack;
using LocalData;

namespace StaticData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDPinMapData : DataBase
    {
        //[MemoryPackIgnore] public int X { get => x; set => x = value; }
        //[MemoryPackInclude][SerializeField] private int x;
        //[MemoryPackIgnore] public int Y { get => y; set => y = value; }
        //[MemoryPackInclude][SerializeField] private int y;

        //[MemoryPackIgnore] public int Dir { get => dir; set => dir = value; }
        //[MemoryPackInclude][SerializeField] private int dir;
        //[MemoryPackIgnore] public int Type { get => type; set => type = value; }
        //[MemoryPackInclude][SerializeField] private int type;
        [MemoryPackInclude] public int X { get; set; }
        [MemoryPackInclude] public int Y { get; set; }

        [MemoryPackInclude] public int Dir { get; set; }
        [MemoryPackInclude] public int Type { get; set; }

        //[MemoryPackConstructor]
        //public LDPinMapData(int x, int y, int dir, int type)
        //{
        //    this.x = x;
        //    this.y = y;
        //    this.dir = dir;
        //    this.type = type;
        //}
        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Dir: {Dir}, Type: {Type}";
        }
    }
}