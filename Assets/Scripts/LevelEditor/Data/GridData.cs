#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class GridData
    {
        private Dictionary<Vector3Int, PlacementData> placedObjects = new Dictionary<Vector3Int, PlacementData>();
        private List<Vector3Int> positionToOccupy;
        private List<Vector3Int> returnVal;
        private PlacementData data;

        public void AddObjectAt(Vector3Int gridPosition, Vector3Int objectSize, int ID, int placedObjectIndex)
        {
            positionToOccupy = CalculatePositions(gridPosition, objectSize);
            data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

            foreach (var pos in positionToOccupy)
            {
                if (placedObjects.ContainsKey(pos))
                {
                    LogManager.LogError($"Dictionary already contains this cell position {pos}");
                }
                placedObjects.Add(pos, data);
            }
        }

        private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
        {
            List<Vector3Int> returnVal = new();
            for (int x = 0; x < objectSize.x; x++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
                }
            }
            return returnVal;
        }

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

        internal int GetRepresentationIndex(Vector3Int gridPosition)
        {
            if (!placedObjects.ContainsKey(gridPosition))
            {
                return -1;
            }
            return placedObjects[gridPosition].PlacedObjectIndex;
        }

        internal void RemoveObjectAt(Vector3Int gridPosition)
        {
            foreach (var pos in placedObjects[gridPosition].occupiedPositions)
            {
                placedObjects.Remove(pos);
            }
        }
    }
}

#endif
