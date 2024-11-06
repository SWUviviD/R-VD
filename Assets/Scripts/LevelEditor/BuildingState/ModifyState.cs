#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 설치된 오브젝트 수치 값을 관리하는 클래스
    /// </summary>
    public class ModifyState : IBuildingState
    {
        private int gameObjectIndex = -1;
        private PreviewSystem previewSystem;
        private GridData selectedData;
        private GridData placementData;
        private ObjectPlacer objectPlacer;
        private GimmickStatus gimmickStatus;
        private GimmickStatusData gimmickStatusData;
        private EditingTransformPosition editingTransformPosition;
        private EditingTransformRotation editingTransformRotation;
        private EditingTransformScale editingTransformScale;
        private KeyCode keyCode;

        private Transform objectTransform;
        private bool validity;

        public ModifyState(PreviewSystem previewSystem,
                           GridData placementData,
                           ObjectPlacer objectPlacer,
                           GimmickStatus gimmickStatus,
                           EditingTransformPosition editingTransformPosition,
                           EditingTransformRotation editingTransformRotation,
                           EditingTransformScale editingTransformScale,
                           KeyCode keyCode)
        {
            this.previewSystem = previewSystem;
            this.placementData = placementData;
            this.objectPlacer = objectPlacer;
            this.gimmickStatus = gimmickStatus;
            this.editingTransformPosition = editingTransformPosition;
            this.editingTransformRotation = editingTransformRotation;
            this.editingTransformScale = editingTransformScale;
            this.keyCode = keyCode;
        }

        public void EndState()
        {
            gimmickStatus.SetGimmickData(null);
            ModifyTransform(keyCode, null);
        }

        public void OnAction(Vector3 position)
        {
            selectedData = null;
            if (placementData.IsPlacedObjectAt(position))
            {
                selectedData = placementData;
            }

            if (selectedData != null)
            {
                objectTransform = selectedData.GetObjectTransformAt(position);
                ModifyTransform(keyCode, objectTransform);
                if (!objectPlacer.PlacedObjectIndexs.TryGetValue(objectTransform, out gameObjectIndex))
                {
                    LogManager.LogError("감지된 오브젝트의 Index를 알 수 없습니다.");
                    return;
                }

                // 배치된 오브젝트의 기믹 상태 값 출력
                gimmickStatusData = placementData.GetGimmickStatus(gameObjectIndex);
                gimmickStatus.SetGimmickData(gimmickStatusData);
            }
        }

        private void ModifyTransform(KeyCode keyCode, Transform transform)
        {
            if (keyCode == KeyCode.Q)
            {
                editingTransformPosition.SetObjectTransform(null);
                editingTransformRotation.SetObjectTransform(null);
                editingTransformScale.SetObjectTransform(null);
            }
            if (keyCode == KeyCode.W)
            {
                editingTransformPosition.SetObjectTransform(transform);
            }
            else if (keyCode == KeyCode.E)
            {
                editingTransformRotation.SetObjectTransform(transform);
            }
            else if (keyCode == KeyCode.R)
            {
                editingTransformScale.SetObjectTransform(transform);
            }
        }

        /// <summary>
        /// 오브젝트 선택이 유효한지 검사
        /// </summary>
        private bool CheckIfSelectionIsValid(Vector3 position)
        {
            return placementData.IsPlacedObjectAt(position);
        }

        public void UpdateState(Vector3 position)
        {
            validity = CheckIfSelectionIsValid(position);
            previewSystem.UpdatePosition(position, validity);
        }
    }
}

#endif
