using LocalData;
using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticData
{
    //DialogNumber	TextNumber	Name	Text	NextTextNumber	Op1Txt	Op1Num	Op2Txt	Op2Num

    [MemoryPackable]
    [Serializable]
    public partial class DialogInfo : DataBase
    {
        [MemoryPackInclude] public int DialogNumber { get; set; }
        [MemoryPackInclude] public int TextNumber { get; set; }
        [MemoryPackInclude] public string Name { get; set; }
        [MemoryPackInclude] public string Text { get; set; }
        [MemoryPackInclude] public int NextTextNumber { get; set; }
        [MemoryPackInclude] public string Op1Txt { get; set; }
        [MemoryPackInclude] public int Op1Num { get; set; }
        [MemoryPackInclude] public string Op2Txt { get; set; }
        [MemoryPackInclude] public int Op2Num { get; set; }
    }

}


