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
        private PreviewSystem previewSystem;
        private ObjectDatabase database;
        private GridData selectedData;
        private ObjectPlacer objectPlacer;

        private bool placementValidity;
        private int index;

        public PlacementState(int ID,
                              PreviewSystem previewSystem,
                              ObjectDatabase database,
                              GridData selectedData,
                              ObjectPlacer objectPlacer)
        {
            this.ID = ID;
            this.previewSystem = previewSystem;
            this.database = database;
            this.selectedData = selectedData;
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

        public void OnAction(Vector3 position)
        {
            placementValidity = CheckPlacementValidity(position, selectedObjectIndex);
            // 오브젝트 설치가 불가능하면 종료
            if (!placementValidity)
            {
                return;
            }

            // 오브젝트 배치 및 데이터 추가
            index = objectPlacer.PlaceObject(position,
                                             database.objectData[selectedObjectIndex].Prefab,
                                             database.objectData[selectedObjectIndex].Size);
            selectedData.AddObjectAt(position,
                                     database.objectData[selectedObjectIndex].ID,
                                     index);

            // 미리보기 오브젝트 갱신
            previewSystem.UpdatePosition(position, false);
        }

        /// <summary>
        /// 오브젝트 배치가 가능한지 검사
        /// </summary>
        private bool CheckPlacementValidity(Vector3 position, int selectedObjectIndex)
        {
            return selectedData.CanPlaceObjectAt(position, database.objectData[selectedObjectIndex].Size);
        }

        public void UpdateState(Vector3 position)
        {
            // 오브젝트 배치 가능 유무 검사
            placementValidity = CheckPlacementValidity(position, selectedObjectIndex);

            // 미리보기 오브젝트 갱신
            previewSystem.UpdatePosition(position, placementValidity);
        }
    }
}

#endif
