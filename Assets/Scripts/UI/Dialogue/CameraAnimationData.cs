using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CamAnim.CameraAnimationData;

namespace CamAnim
{
    public enum EaseMode
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }

    [MemoryPackable]
    [Serializable]
    public partial class CameraState
    {
        [MemoryPackIgnore][field: SerializeField] public Transform BasePosition { get; set; }
        [MemoryPackInclude][field: SerializeField] public Vector3 LocalPosition { get; set; }
        [MemoryPackInclude][field: SerializeField] public Vector3 LocalRotation { get; set; }
        [MemoryPackInclude][field: SerializeField] public float Zoom { get; set; }
    }

    [MemoryPackable]
    [Serializable]
    public partial class Step
    {
        [MemoryPackInclude][field:SerializeField] public EaseMode Mode { get; set; }

        [MemoryPackInclude][field: SerializeField] public float MoveTime { get; set; }
        [MemoryPackInclude][field: SerializeField] public bool WaitForFinish { get; set; }

        [MemoryPackInclude][field: SerializeField] public CameraState[] CameraStates { get; set; }

        [MemoryPackInclude][field: SerializeField] public bool Shack { get; set; }
        [MemoryPackInclude][field: SerializeField] public float ShackTime { get; set; }

        [MemoryPackInclude][field: SerializeField] public bool IsLoop { get; set; }

        [MemoryPackIgnore][field: SerializeField] public AchieveData AchieveData { get; set; }
    }

    [Serializable]
    public class CameraAnimationData : MonoBehaviour
    {

        [SerializeField] private string animationName = "DialogueCam1";
        [SerializeField][field: SerializeField] public List<Step> Steps { get; set; }

        [ContextMenu("MakeAnimation")]
        public void MakeAnimation()
        {
            if (Steps == null)
            {
                return;
            }

            var byteArray = MemoryPackSerializer.Serialize(Steps);
            SerializeManager.Instance.SaveDataFile(animationName, byteArray, "Data/RawData/DialogCamAnimation");
        }
    }
}
