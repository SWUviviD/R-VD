#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 그리드에 오브젝트 배치를 관리하는 클래스
    /// </summary>
    public class GridData
    {
        /// <summary> 그리드에 배치된 오브젝트 데이터를 저장하는 딕셔너리 </summary>
        private Dictionary<Vector3, PlacementData> placedObjects = new Dictionary<Vector3, PlacementData>();

        /// <summary> 반환값 리스트 </summary>
        private List<Vector3> returnVal;

        /// <summary> 현재 배치할 오브젝트의 데이터 </summary>
        private PlacementData data;

        private Collider[] colliders;

        /// <summary>
        /// 주어진 위치에 오브젝트를 추가
        /// </summary>
        public void AddObjectAt(Vector3 position, int ID, int placedObjectIndex)
        {
            data = new PlacementData(position, ID, placedObjectIndex);
            placedObjects.Add(position, data);
        }

        /// <summary>
        /// 해당 위치에 오브젝트를 배치할 수 있는지 확인
        /// </summary>
        public bool CanPlaceObjectAt(Vector3 position, Vector3 objectSize)
        {
            colliders = Physics.OverlapBox(position + objectSize.y * Vector3.up / 2,
                                           objectSize * 0.99f / 2,
                                           Quaternion.identity,
                                           LayerMask.GetMask("PlacedArea"));
            if (colliders.Length > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 해당 위치에 오브젝트를 배치할 수 있는지 확인
        /// </summary>
        public bool CanRemoveObjectAt(Vector3 position)
        {
            colliders = Physics.OverlapSphere(position, 0.1f, LayerMask.GetMask("PlacedArea"));
            if (colliders.Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 삭제할 오브젝트 위치 반환
        /// </summary>
        public Vector3 GetRemoveObjectPosition()
        {
            return colliders[0].transform.root.position;
        }

        /// <summary>
        /// 오브젝트가 차지할 3차원 좌표 계산
        /// </summary>
        private List<Vector3> CalculatePositions(Vector3 position, Vector3 objectSize)
        {
            returnVal = new List<Vector3>();
            for (int x = 0; x < objectSize.x; ++x)
            {
                for (int y = 0; y < objectSize.y; ++y)
                {
                    for (int z = 0; z < objectSize.z; ++z)
                    {
                        returnVal.Add(position + new Vector3(x, y, z));
                    }
                }
            }
            return returnVal;
        }

        /// <summary>
        /// 주어진 위치에서 배치된 오브젝트의 인덱스를 반환
        /// </summary>
        public int GetRepresentationIndex(Vector3 position)
        {
            if (!placedObjects.ContainsKey(position))
            {
                return -1;
            }
            return placedObjects[position].PlacedObjectIndex;
        }

        /// <summary>
        /// 주어진 위치에서 오브젝트 제거
        /// </summary>
        public void RemoveObjectAt(Vector3 position)
        {
            placedObjects.Remove(placedObjects[position].PlacedPosition);
        }
    }
}

#endif
