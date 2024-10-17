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
        private GimmickStatusData gimmickStatusData;
        private PreviewSystem previewSystem;
        private ObjectDatabase database;
        private GridData placementData;
        private ObjectPlacer objectPlacer;
        private float gridSize;
        private bool isGridMod;

        private Vector3 collisionPosition;
        private Vector3 collisionObjectSize;
        private int collisionObjectID;
        private bool placementValidity;
        private int index;

        private Vector3 placedSize;
        private Vector3 placedScale;
        private Vector3 collisionedScale;

        private float distanceX;
        private float distanceY;
        private float distanceZ;
        private float moveX;
        private float moveY;
        private float moveZ;

        public PlacementState(int ID,
                              PreviewSystem previewSystem,
                              ObjectDatabase database,
                              GridData placementData,
                              ObjectPlacer objectPlacer,
                              float gridSize,
                              bool isGridMod)
        {
            this.ID = ID;
            this.previewSystem = previewSystem;
            this.database = database;
            this.placementData = placementData;
            this.objectPlacer = objectPlacer;
            this.gridSize = gridSize;
            this.isGridMod = isGridMod;

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
            // 그리드 모드 검사
            if (isGridMod)
            {
                position = new Vector3((int)(position.x / gridSize) * gridSize,
                                       (int)(position.y / gridSize) * gridSize,
                                       (int)(position.z / gridSize) * gridSize);
            }

            // 오브젝트 설치가 불가능한 경우
            if (!CheckPlacementValidity(position, selectedObjectIndex))
            {
                // 위치 보정
                position = GetFixedPosition(position, database.objectData[selectedObjectIndex].Size);
                if (CheckPlacementValidity(position, selectedObjectIndex) == false)
                {
                    return;
                }
            }

            // 오브젝트 배치 및 데이터 추가
            index = objectPlacer.PlaceObject(position,
                                             database.objectData[selectedObjectIndex].Prefab,
                                             database.objectData[selectedObjectIndex].Size);
            if (index != -1)
            {
                // 기믹 상태 데이터 생성
                gimmickStatusData = null;
                if (objectPlacer.PlacedGameObjects[index].TryGetComponent(out GimmickDataBase gimmickDataBase) &&
                    objectPlacer.PlacedGameObjects[index].TryGetComponent(out IGimmickBase iGimmickBase))
                {
                    gimmickStatusData = new GimmickStatusData(database.objectData[selectedObjectIndex].Name,
                                                              gimmickDataBase,
                                                              iGimmickBase.SetGimmick);
                }

                // PlacementData 기믹 상태 데이터를 포함한 오브젝트 정보 생성
                placementData.AddObjectAt(gimmickStatusData,
                                          position,
                                          database.objectData[selectedObjectIndex].ID,
                                          index);
            }
        }

        /// <summary>
        /// 오브젝트 배치가 가능한지 검사
        /// </summary>
        private bool CheckPlacementValidity(Vector3 position, int selectedObjectIndex)
        {
            return placementData.CanPlaceObjectAt(position, database.objectData[selectedObjectIndex].Size);
        }

        public void UpdateState(Vector3 position)
        {
            // 오브젝트 배치 가능 유무 검사
            if (!CheckPlacementValidity(position, selectedObjectIndex))
            {
                position = GetFixedPosition(position, database.objectData[selectedObjectIndex].Size);
            }

            // 그리드 모드 검사
            if (isGridMod)
            {
                position = new Vector3((int)(position.x / gridSize) * gridSize,
                                       (int)(position.y / gridSize) * gridSize,
                                       (int)(position.z / gridSize) * gridSize);
            }

            // 미리보기 오브젝트 갱신
            previewSystem.UpdatePosition(position, true);
        }

        /// <summary>
        /// 충돌한 오브젝트의 법선으로 배치할 오브젝트 위치 계산
        /// </summary>
        private Vector3 GetFixedPosition(Vector3 position, Vector3 size)
        {
            // TODO: 위치값 보정 개선 필요
            position = GetFixedPositionXZ(position, size);

            return position;
        }

        /// <summary>
        /// 배치할 오브젝트 X축, Z축 보정
        /// </summary>
        private Vector3 GetFixedPositionXZ(Vector3 position, Vector3 size)
        {
            if (!placementData.TryGetCollisionedObjects(position, size, out Transform[] transforms))
            {
                return position;
            }

            foreach (Transform objectTR in transforms)
            {
                if (!objectPlacer.PlacedObjectIndexs.TryGetValue(objectTR, out int gameObjectIndex))
                {
                    LogManager.LogError("감지된 오브젝트의 Index를 알 수 없습니다.");
                    continue;
                }

                // 저장된 ID 탐색
                collisionObjectID = database.objectData.FindIndex(data => data.ID == placementData.GetPlacedObjectID(gameObjectIndex));

                // 크기 계산
                placedSize = database.objectData[collisionObjectID].Size;
                placedScale = objectPlacer.PlacedGameObjects[gameObjectIndex].transform.localScale;
                collisionedScale = new Vector3(placedSize.x * placedScale.x, placedSize.y * placedScale.y, placedSize.z * placedScale.z);

                // X축 보정
                distanceX = objectTR.position.x - position.x;
                moveX = (size.x + collisionedScale.x) / 2 - Mathf.Abs(distanceX);

                // Z축 보정
                distanceZ = objectTR.position.z - position.z;
                moveZ = (size.z + collisionedScale.z) / 2 - Mathf.Abs(distanceZ);

                // 보정된 위치로 위치 이동
                if (moveX < moveZ && moveX > 0)
                {
                    position.x += distanceX < 0 ? moveX : -moveX;
                }
                else if (moveX > moveZ && moveZ > 0)
                {
                    position.z += distanceZ < 0 ? moveZ : -moveZ;
                }

            }

            return position;
        }
    }
}

#endif
