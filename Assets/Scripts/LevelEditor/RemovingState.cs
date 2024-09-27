#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    public class RemovingState : IBuildingState
    {
        private int gameObjectIndex = -1;
        private Grid grid;
        private PreviewSystem previewSystem;
        private GridData floorData;
        private GridData furnitureData;
        private ObjectPlacer objectPlacer;

        private GridData selectedData;
        private Vector3 cellPosition;
        private bool validity;

        public RemovingState(Grid grid,
                             PreviewSystem previewSystem,
                             GridData floorData,
                             GridData furnitureData,
                             ObjectPlacer objectPlacer)
        {
            this.grid = grid;
            this.previewSystem = previewSystem;
            this.floorData = floorData;
            this.furnitureData = furnitureData;
            this.objectPlacer = objectPlacer;
            previewSystem.StartShowingRemovePreview();
        }

        public void EndState()
        {
            previewSystem.StopShowingPreview();
        }

        public void OnAction(Vector3Int gridPosition)
        {
            selectedData = null;
            if (furnitureData.CanPlaceObjectAt(gridPosition, Vector3Int.one) == false)
            {
                selectedData = furnitureData;
            }
            else if (floorData.CanPlaceObjectAt(gridPosition, Vector3Int.one) == false)
            {
                selectedData = floorData;
            }

            if (selectedData != null)
            {
                gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
                if (gameObjectIndex == -1)
                {
                    return;
                }
                selectedData.RemoveObjectAt(gridPosition);
                objectPlacer.RemoveObjectAt(gameObjectIndex);
            }

            cellPosition = grid.CellToWorld(gridPosition);
            previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
        }

        private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
        {
            return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector3Int.one)
                     && floorData.CanPlaceObjectAt(gridPosition, Vector3Int.one));
        }

        public void UpdateState(Vector3Int gridPosition)
        {
            validity = CheckIfSelectionIsValid(gridPosition);
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
        }
    }
}

#endif
