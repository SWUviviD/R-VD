#if UNITY_EDITOR

using System;
using UnityEngine;

namespace LevelEditor
{
    [Serializable]
    public class ObjectData
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public Vector3Int Size { get; private set; } = Vector3Int.one;
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}

#endif
