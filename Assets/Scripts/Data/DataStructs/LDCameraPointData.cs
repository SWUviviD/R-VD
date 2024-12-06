using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDCameraPointData : DataBase
    {
        [MemoryPackInclude]
        public Vector3 Position { get; set; }
        
        [MemoryPackInclude]
        public Vector3 PointV1 { get; set; }
        [MemoryPackInclude]
        public Vector3 PointV2 { get; set; }
        [MemoryPackInclude]
        public Vector3 PointV3 { get; set; }
        [MemoryPackInclude]
        public Vector3 PointV4 { get; set; }
    
        [MemoryPackInclude]
        public float RatioV1V2 { get; set; }
        [MemoryPackInclude]
        public float RatioV2V3 { get; set; }
        [MemoryPackInclude]
        public float RatioV3V4 { get; set; }

        public CameraPathPoint ToCameraPathPoint()
        {
            CameraPathPoint result = new CameraPathPoint();

            result.Position = Position;
            
            result.PointV1 = PointV1;
            result.PointV2 = PointV2;
            result.PointV3 = PointV3;
            result.PointV4 = PointV4;
            
            result.RatioV1V2 = RatioV1V2;
            result.RatioV2V3 = RatioV2V3;
            result.RatioV3V4 = RatioV3V4;
            
            return result;
        }
    }
}