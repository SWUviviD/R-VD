using LocalData;
using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDWaterBlockData : LDGimmickDataBase
{
    [MemoryPackInclude]
    public int maxWaterCapacity { get; set; } = 3;

    [MemoryPackInclude]
    public int maxWaterUsage { get; set; } = 3;

    [MemoryPackInclude]
    public Material[] blockMaterials;
}
