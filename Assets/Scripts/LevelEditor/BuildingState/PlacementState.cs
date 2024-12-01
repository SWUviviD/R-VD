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
        private float gridSizeX;
        private float gridSizeY;
        private float gridSizeZ;
        private bool isGridMod;

        private Vector3 collisionPosition;
        private Vector3 collisionObjectSize;
        private int collisionObjectID;
        private bool placementValidity;
        private int index;

        private Renderer[] renderers;
        private Bounds totalBounds;
        private Vector3 placedSize;
        private Vector3 placedScale;
        private Vector3 collisionedScale;
        private Vector3 objScale;

        private float distanceX;
        private float distanceY;
        private float distanceZ;
        private float moveMaxX;
        private float moveMaxY;
        private float moveMaxZ;
        private float moveX;
        private float moveY;
        private float moveZ;
        private bool isPlacementValid;

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

            // 오브젝트 배치 미리보기 시작
            totalBounds = GetPrefabBounds(database.objectData[selectedObjectIndex].Prefab);
            previewSystem.StartShowingPlacementPreview(database.objectData[selectedObjectIndex].Prefab,
                                                       database.objectData[selectedObjectIndex].Size,
                                                       totalBounds.center);

            gridSizeX = Mathf.Min(gridSize, database.objectData[selectedObjectIndex].Size.x);
            gridSizeY = Mathf.Min(gridSize, database.objectData[selectedObjectIndex].Size.y);
            gridSizeZ = Mathf.Min(gridSize, database.objectData[selectedObjectIndex].Size.z);
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
                position = new Vector3((int)(position.x / gridSizeX) * gridSizeX,
                                       (int)(position.y / gridSizeY) * gridSizeY,
                                       (int)(position.z / gridSizeZ) * gridSizeZ);
            }

            // 오브젝트 설치가 불가능한 경우
            if (!CheckPlacementValidity(position, selectedObjectIndex))
            {
                // 위치 보정
                position = GetFixedPosition(position, database.objectData[selectedObjectIndex].Size);
                //if (CheckPlacementValidity(position, selectedObjectIndex) == false)
                //{
                //    return;
                //}
            }

            // 오브젝트 배치 및 데이터 추가
            index = objectPlacer.PlaceObject(database.objectData[selectedObjectIndex].Name,
                                             position,
                                             Vector3.zero,
                                             Vector3.one,
                                             totalBounds.center,
                                             database.objectData[selectedObjectIndex].Size,
                                             database.objectData[selectedObjectIndex].Prefab);
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
                                          Vector3.zero,
                                          Vector3.one,
                                          database.objectData[selectedObjectIndex].ID);
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
            // 그리드 모드 검사
            if (isGridMod)
            {
                position = new Vector3((int)(position.x / gridSizeX) * gridSizeX,
                                       (int)(position.y / gridSizeY) * gridSizeY,
                                       (int)(position.z / gridSizeZ) * gridSizeZ);
            }

            // 오브젝트 배치 가능 유무 검사
            isPlacementValid = CheckPlacementValidity(position, selectedObjectIndex);
            if (!isPlacementValid)
            {
                position = GetFixedPosition(position, database.objectData[selectedObjectIndex].Size);
                //isPlacementValid = CheckPlacementValidity(position, selectedObjectIndex);
            }

            // 미리보기 오브젝트 갱신
            previewSystem.UpdatePosition(position, true);
        }

        /// <summary>
        /// 충돌한 오브젝트의 법선으로 배치할 오브젝트 위치 계산
        /// </summary>
        private Vector3 GetFixedPosition(Vector3 position, Vector3 size)
        {
            position = GetFixedPositionY(position, size);
            position = GetFixedPositionXZ(position, size);

            return position;
        }

        /// <summary>
        /// 배치할 오브젝트 Y축 보정
        /// </summary>
        private Vector3 GetFixedPositionY(Vector3 position, Vector3 size)
        {
            if (!placementData.TryGetCollisionedObjects(position, size, out Transform[] transforms))
            {
                return position;
            }

            moveMaxY = 0;
            foreach (Transform objectTransform in transforms)
            {
                if (!objectPlacer.PlacedObjectIndexs.TryGetValue(objectTransform, out int gameObjectIndex))
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

                // Y축 보정
                distanceY = objectTransform.position.y - position.y;
                moveY = (size.y + collisionedScale.y) / 2 - Mathf.Abs(distanceY);
                moveMaxY = Mathf.Max(moveMaxY, moveY);
            }
            position.y += isGridMod ? (Mathf.CeilToInt(moveMaxY / gridSize) + 1) * gridSize : moveMaxY;

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

            Vector3 oldPosition = position;
            bool isLeft = false, isRight = false;
            bool isBackward = false, isForward = false;
            moveMaxX = 0f;
            moveMaxZ = 0f;
            foreach (Transform objectTransform in transforms)
            {
                if (!objectPlacer.PlacedObjectIndexs.TryGetValue(objectTransform, out int gameObjectIndex))
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
                distanceX = objectTransform.position.x - position.x;
                moveX = (size.x + collisionedScale.x) / 2 - Mathf.Abs(distanceX);
                moveMaxX = Mathf.Max(moveMaxX, moveX);

                // Z축 보정
                distanceZ = objectTransform.position.z - position.z;
                moveZ = (size.z + collisionedScale.z) / 2 - Mathf.Abs(distanceZ);
                moveMaxZ = Mathf.Max(moveMaxZ, moveZ);

                if (distanceX < 0)
                    isLeft = true;
                else
                    isRight = true;

                if (distanceZ < 0)
                    isBackward = true;
                else
                    isForward = true;
            }

            // 보정된 위치로 위치 이동
            if (isGridMod) // 그리드 모드 (O)
            {
                if (moveMaxX < moveMaxZ && moveMaxX > 0)
                {
                    position.x += distanceX < 0 ? moveMaxX : -moveMaxX;
                }
                else if (moveMaxX > moveMaxZ && moveMaxZ > 0)
                {
                    position.z += distanceZ < 0 ? moveMaxZ : -moveMaxZ;
                }
            }
            else // 그리드 모드 (X)
            {
                if (moveX < moveZ && moveX > 0)
                {
                    position.x += distanceX < 0 ? moveMaxX : -moveMaxX;
                }
                else if (moveX > moveZ && moveZ > 0)
                {
                    position.z += distanceZ < 0 ? moveMaxZ : -moveMaxZ;
                }
            }

            if (isLeft && isRight)
            {
                position.x = oldPosition.x;
            }
            if (isBackward && isForward)
            {
                position.z = oldPosition.z;
            }

            return position;
        }

        /// <summary>
        /// 프리팹 전체 크기를 계산할 Bounds 반환
        /// </summary>
        private Bounds GetPrefabBounds(GameObject prefab)
        {
            // 모든 자식들의 Renderer 컴포넌트를 가져오기
            renderers = prefab.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                // 프리팹 전체 Bounds 크기
                totalBounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; ++i)
                {
                    totalBounds.Encapsulate(renderers[i].bounds);
                }
            }
            return totalBounds;
        }
    }
}

#endif
