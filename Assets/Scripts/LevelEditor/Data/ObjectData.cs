#if UNITY_EDITOR

using System;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 레벨 에디터에서 사용할 오브젝트의 데이터를 저장하는 클래스
    /// </summary>
    [Serializable]
    public class ObjectData
    {
        /// <summary> 오브젝트의 이름 </summary>
        [field: SerializeField] public string Name { get; private set; }

        /// <summary> 오브젝트의 고유 ID </summary>
        [field: SerializeField] public int ID { get; private set; }

        /// <summary> 오브젝트의 크기 (기본값: Vector3Int(1, 1, 1)) </summary>
        [field: SerializeField] public Vector3Int Size { get; private set; } = Vector3Int.one;

        /// <summary> 오브젝트의 프리팹 (미리 정의된 오브젝트) </summary>
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}

#endif
