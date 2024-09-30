#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 오브젝트 데이터를 관리하는 데이터베이스를 ScriptableObject로 정의한 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "Object Database", menuName = "Scriptable Object/Object Database")]
    public class ObjectDatabase : ScriptableObject
    {
        /// <summary> ObjectData 리스트 </summary>
        public List<ObjectData> objectData;
    }
}

#endif
