#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 오브젝트가 배치된 위치 및 ID 등의 정보를 저장하는 클래스
    /// </summary>
    public class PlacementData
    {
        /// <summary> 배치된 오브젝트의 위치 </summary>
        public Vector3 PlacedPosition { get; private set; }

        /// <summary> 오브젝트의 고유 ID </summary>
        public int ID { get; private set; }

        /// <summary> 배치된 오브젝트의 인덱스 </summary>
        public int PlacedObjectIndex { get; private set; }

        /// <summary>
        /// PlacementData 클래스의 생성자, 오브젝트의 위치, ID, 인덱스를 설정
        /// </summary>
        public PlacementData(Vector3 placedPosition, int id, int placedObjectIndex)
        {
            PlacedPosition = placedPosition;
            ID = id;
            PlacedObjectIndex = placedObjectIndex;
        }
    }
}

#endif
