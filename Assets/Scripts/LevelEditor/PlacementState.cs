#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    public class PlacementState : IBuildingState
    {
        private int selectedObjectIndex = -1;
        private int ID;
        private Grid grid;
        private PreviewSystem previewSystem;
        private ObjectDatabase database;
        private GridData floorData;
        private GridData furnitureData;
        private ObjectPlacer objectPlacer;

        private GridData selectedData;
        private bool placementValidity;
        private int index;

        public PlacementState(int ID,
                              Grid grid,
                              PreviewSystem previewSystem,
                              ObjectDatabase database,
                              GridData floorData,
                              GridData furnitureData,
                              ObjectPlacer objectPlacer)
        {
            this.ID = ID;
            this.grid = grid;
            this.previewSystem = previewSystem;
            this.database = database;
            this.floorData = floorData;
            this.furnitureData = furnitureData;
            this.objectPlacer = objectPlacer;

            selectedObjectIndex = database.objectData.FindIndex(data => data.ID == this.ID);
            if (selectedObjectIndex < 0)
            {
                LogManager.LogError($"No object with ID {ID}");
                return;
            }

            previewSystem.StartShowingPlacementPreview(database.objectData[selectedObjectIndex].Prefab,
                                                       database.objectData[selectedObjectIndex].Size);
        }

        public void EndState()
        {
            previewSystem.StopShowingPreview();
        }

        public void OnAction(Vector3Int gridPosition)
        {
            placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            if (!placementValidity)
            {
                return;
            }

            index = objectPlacer.PlaceObject(database.objectData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));
            selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
            selectedData.AddObjectAt(gridPosition,
                                     database.objectData[selectedObjectIndex].Size,
                                     database.objectData[selectedObjectIndex].ID,
                                     index);

            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        }

        private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
        {
            selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;

            return selectedData.CanPlaceObjectAt(gridPosition, database.objectData[selectedObjectIndex].Size);
        }

        public void UpdateState(Vector3Int gridPosition)
        {
            placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        }
    }
}

#endif
