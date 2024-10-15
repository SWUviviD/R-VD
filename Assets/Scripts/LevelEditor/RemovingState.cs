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
        private PreviewSystem previewSystem;
        private GridData selectedData;
        private GridData placementData;
        private ObjectPlacer objectPlacer;

        private Collider[] colliders;
        private Transform objectTransform;
        private Vector3 objectPosition;
        private bool validity;

        public RemovingState(PreviewSystem previewSystem,
                             GridData placementData,
                             ObjectPlacer objectPlacer)
        {
            this.previewSystem = previewSystem;
            this.placementData = placementData;
            this.objectPlacer = objectPlacer;

            previewSystem.StartShowingRemovePreview();
        }

        public void EndState()
        {
            previewSystem.StopShowingPreview();
        }

        public void OnAction(Vector3 position)
        {
            selectedData = null;
            if (placementData.IsPlacedObjectAt(position))
            {
                selectedData = placementData;
            }

            //if (selectedData.IsPlacedObjectAt(position))
            if (selectedData != null)
            {
                objectTransform = selectedData.GetObjectTransformAt(position);
                gameObjectIndex = objectPlacer.PlacedObjectIndexs[objectTransform];
                // 오브젝트 인덱스가 유효하지 않으면 종료
                if (gameObjectIndex == -1)
                {
                    return;
                }

                // 배치된 오브젝트 및 배치 시스템의 오브젝트 데이터 제거
                selectedData.RemoveObjectAt(gameObjectIndex);
                objectPlacer.RemoveObjectAt(gameObjectIndex);
            }
        }

        /// <summary>
        /// 오브젝트 선택이 유효한지 검사
        /// </summary>
        private bool CheckIfSelectionIsValid(Vector3 position)
        {
            return placementData.IsPlacedObjectAt(position);
        }

        public void UpdateState(Vector3 position, Vector3 objectNormal)
        {
            validity = CheckIfSelectionIsValid(position);
            previewSystem.UpdatePosition(position, validity);
        }
    }
}

#endif
