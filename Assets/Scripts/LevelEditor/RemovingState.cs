#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 설치된 오브젝트 제거 상태를 관리하는 클래스
    /// </summary>
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

        public void OnAction(Vector3 gridPosition)
        {
            // 선택된 데이터 초기화
            selectedData = null;
            if (furnitureData.CanPlaceObjectAt(gridPosition, Vector3Int.one) == false)
            {
                // 가구 데이터 선택
                selectedData = furnitureData;
            }
            else if (floorData.CanPlaceObjectAt(gridPosition, Vector3Int.one) == false)
            {
                // 바닥 데이터 선택
                selectedData = floorData;
            }

            if (selectedData != null)
            {
                gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
                // 오브젝트 인덱스가 유효하지 않으면 종료
                if (gameObjectIndex == -1)
                {
                    return;
                }

                // 배치된 오브젝트 및 배치 시스템의 오브젝트 데이터 제거
                selectedData.RemoveObjectAt(gridPosition);
                objectPlacer.RemoveObjectAt(gameObjectIndex);
            }

            // 셀 위치 및 미리보기 위치 갱신
            previewSystem.UpdatePosition(gridPosition, CheckIfSelectionIsValid(gridPosition));
        }

        /// <summary>
        /// 오브젝트 선택이 유효한지 검사
        /// </summary>
        private bool CheckIfSelectionIsValid(Vector3 gridPosition)
        {
            return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector3.one)
                     && floorData.CanPlaceObjectAt(gridPosition, Vector3.one));
        }

        public void UpdateState(Vector3 gridPosition)
        {
            validity = CheckIfSelectionIsValid(gridPosition);
            previewSystem.UpdatePosition(gridPosition, validity);
        }
    }
}

#endif
