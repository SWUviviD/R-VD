#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class PlacementData
    {
        public List<Vector3Int> occupiedPositions;
        public int ID { get; private set; }
        public int PlacedObjectIndex { get; private set; }

        public PlacementData(List<Vector3Int> occupiedPositions, int ID, int placedObjectIndex)
        {
            this.occupiedPositions = occupiedPositions;
            this.ID = ID;
            PlacedObjectIndex = placedObjectIndex;
        }
    }
}

#endif
