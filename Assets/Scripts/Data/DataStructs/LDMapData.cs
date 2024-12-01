using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDMapData : DataBase
    {
        [MemoryPackInclude] public List<LDBlinkBoardData> BlinkBoardDataList { get; set; }
        [MemoryPackInclude] public List<LDGalaxyGimmickData> GalaxyGimmickDataList { get; set; }
        [MemoryPackInclude] public List<LDRunandgunGimmickData> RunandgunGimmickDataList { get; set; }
        [MemoryPackInclude] public List<LDChasingGimmickData> ChasingGimmickDataList { get; set; }

        [MemoryPackInclude] public List<LDBalancePlateData> BalancePlateDataList { get; set; }
        [MemoryPackInclude] public List<LDCristalData> CristalGimmickDataList { get; set; }
        [MemoryPackInclude] public List<LDBlueCristalGimmickData> BlueCristalGimmickDataList { get; set; }
        [MemoryPackInclude] public List<LDGreenCristalGimmickData> GreenCristalGimmickDataList { get; set; }
        [MemoryPackInclude] public List<LDBubbleData> BubbleDataList { get; set; }

        [MemoryPackInclude] public List<CameraPathPoint> CameraPathList { get; set; }

        [MemoryPackInclude] public List<LDLevelEditObject> LevelEditObjectList { get; set; }

        [MemoryPackInclude] public LDPlayerPositionSettor PlayerPositionSettor { get; set; }    

        public LDMapData()
        {
            BlinkBoardDataList = new List<LDBlinkBoardData>();
            GalaxyGimmickDataList = new List<LDGalaxyGimmickData>();
            RunandgunGimmickDataList = new List<LDRunandgunGimmickData>();
            ChasingGimmickDataList = new List<LDChasingGimmickData>();

            BalancePlateDataList = new List<LDBalancePlateData>();
            CristalGimmickDataList = new List<LDCristalData>();
            BlueCristalGimmickDataList = new List<LDBlueCristalGimmickData>();
            GreenCristalGimmickDataList = new List<LDGreenCristalGimmickData> ();
            BubbleDataList = new List<LDBubbleData>();

            CameraPathList = new List<CameraPathPoint>();
            LevelEditObjectList = new List<LDLevelEditObject>();
        }
    }
}