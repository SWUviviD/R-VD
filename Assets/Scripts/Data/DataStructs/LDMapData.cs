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

        [MemoryPackInclude] public List<LDWaterBlockData> WaterBlockDataList { get; set; }
        [MemoryPackInclude] public List<LDWaterWallData> WaterWallDataList { get; set; }

        [MemoryPackInclude] public List<LDCameraPointData> CameraPathList { get; set; }

        [MemoryPackInclude] public List<LDLevelEditObject> LevelEditObjectList { get; set; }

        [MemoryPackInclude] public LDPlayerPositionSettor PlayerPositionSettor { get; set; }

        [MemoryPackInclude] public List<LDCheckpointData> CheckpointList { get; set; }

        [MemoryPackInclude] public LDStageClearPointData StageClearPoint { get; set; }

        [MemoryPackInclude] public List<LDDialogueData> DialogueList { get; set; }

        public LDMapData()
        {
            BlinkBoardDataList = new List<LDBlinkBoardData>();
            GalaxyGimmickDataList = new List<LDGalaxyGimmickData>();
            RunandgunGimmickDataList = new List<LDRunandgunGimmickData>();
            ChasingGimmickDataList = new List<LDChasingGimmickData>();

            BalancePlateDataList = new List<LDBalancePlateData>();
            CristalGimmickDataList = new List<LDCristalData>();
            BlueCristalGimmickDataList = new List<LDBlueCristalGimmickData>();
            GreenCristalGimmickDataList = new List<LDGreenCristalGimmickData>();
            BubbleDataList = new List<LDBubbleData>();

            WaterBlockDataList = new List<LDWaterBlockData>();
            WaterWallDataList = new List<LDWaterWallData>();

            CameraPathList = new List<LDCameraPointData>();
            LevelEditObjectList = new List<LDLevelEditObject>();

            CheckpointList = new List<LDCheckpointData>();

            DialogueList = new List<LDDialogueData>();
        }
    }
}