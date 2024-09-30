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
        private Dictionary<Vector3Int, PlacementData> placedObjects = new Dictionary<Vector3Int, PlacementData>();

        /// <summary> 배치할 위치 리스트 </summary>
        private List<Vector3Int> positionToOccupy;

        /// <summary> 반환값 리스트 </summary>
        private List<Vector3Int> returnVal;

        /// <summary> 현재 배치할 오브젝트의 데이터 </summary>
        private PlacementData data;

        /// <summary>
        /// 주어진 위치에 오브젝트를 추가
        /// </summary>
        public void AddObjectAt(Vector3Int gridPosition, Vector3Int objectSize, int ID, int placedObjectIndex)
        {
            positionToOccupy = CalculatePositions(gridPosition, objectSize);
            data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

            // 각 위치에 오브젝트 데이터 추가
            foreach (var pos in positionToOccupy)
            {
                if (placedObjects.ContainsKey(pos))
                {
                    LogManager.LogError($"Dictionary already contains this cell position {pos}");
                }
                placedObjects.Add(pos, data);
            }
        }

        // <summary>
        /// 해당 위치에 오브젝트를 배치할 수 있는지 확인
        /// </summary>
        public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector3Int objectSize)
        {
            positionToOccupy = CalculatePositions(gridPosition, objectSize);
            foreach (var pos in positionToOccupy)
            {
                if (placedObjects.ContainsKey(pos))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 오브젝트가 차지할 3차원 좌표 계산
        /// </summary>
        private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector3Int objectSize)
        {
            returnVal = new List<Vector3Int>();
            for (int x = 0; x < objectSize.x; ++x)
            {
                for (int y = 0; y < objectSize.y; ++y)
                {
                    for (int z = 0; z < objectSize.z; ++z)
                    {
                        returnVal.Add(gridPosition + new Vector3Int(x, y, z));
                    }
                }
            }
            return returnVal;
        }

        /// <summary>
        /// 주어진 위치에서 배치된 오브젝트의 인덱스를 반환
        /// </summary>
        public int GetRepresentationIndex(Vector3Int gridPosition)
        {
            if (!placedObjects.ContainsKey(gridPosition))
            {
                return -1;
            }
            return placedObjects[gridPosition].PlacedObjectIndex;
        }

        /// <summary>
        /// 주어진 위치에서 오브젝트를 제거
        /// </summary>
        public void RemoveObjectAt(Vector3Int gridPosition)
        {
            foreach (var pos in placedObjects[gridPosition].occupiedPositions)
            {
                placedObjects.Remove(pos);
            }
        }
    }
}

#endif
