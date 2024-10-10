#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 오브젝트 데이터를 관리하는 데이터베이스
    /// </summary>
    public class ObjectDatabase
    {
        /// <summary> ObjectData 리스트 </summary>
        public List<ObjectData> objectData;

        public ObjectDatabase()
        {
            objectData = new List<ObjectData>();
        }
    }
}

#endif
