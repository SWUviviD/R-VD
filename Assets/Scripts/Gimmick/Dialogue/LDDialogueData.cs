using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalData
{
    [MemoryPackable]
    [Serializable]
    public partial class LDDialogueData : LDGimmickDataBase
    {
        [MemoryPackInclude]
        public int DialogueID { get; set; } = -1;

        [MemoryPackInclude]
        public Vector3 CameraPosition { get; set; } = Vector3.zero;

        [MemoryPackInclude]
        public Vector3 CameraRotation { get; set; } = Vector3.zero;
    }
}
