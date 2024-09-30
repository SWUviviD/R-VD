#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 오브젝트 배치 상태를 관리하는 클래스
    /// </summary>
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
            // 오브젝트 설치가 불가능하면 종료
            if (!placementValidity)
            {
                return;
            }

            // 오브젝트 배치 및 데이터 추가
            index = objectPlacer.PlaceObject(database.objectData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));
            selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
            selectedData.AddObjectAt(gridPosition,
                                     database.objectData[selectedObjectIndex].Size,
                                     database.objectData[selectedObjectIndex].ID,
                                     index);

            // 미리보기 오브젝트 갱신
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        }

        /// <summary>
        /// 오브젝트 배치가 가능한지 검사
        /// </summary>
        private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
        {
            selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;

            return selectedData.CanPlaceObjectAt(gridPosition, database.objectData[selectedObjectIndex].Size);
        }

        public void UpdateState(Vector3Int gridPosition)
        {
            // 오브젝트 배치 가능 유무 검사
            placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

            // 미리보기 오브젝트 갱신
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        }
    }
}

#endif
