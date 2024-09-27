#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    [CreateAssetMenu(fileName = "Object Database", menuName = "Scriptable Object/Object Database")]
    public class ObjectDatabase : ScriptableObject
    {
        public List<ObjectData> objectData;
    }
}

#endif
