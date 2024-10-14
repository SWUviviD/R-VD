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

        private Vector3 collisionPosition;
        private Vector3 collisionObjectSize;
        private Vector3 collisionNormal;
        private int collisionObjectID;
        private bool placementValidity;
        private int collisionObjectIndex;
        private int index;

        public PlacementState(int ID,
                              PreviewSystem previewSystem,
                              ObjectDatabase database,
                              GridData placementData,
                              ObjectPlacer objectPlacer)
        {
            this.ID = ID;
            this.previewSystem = previewSystem;
            this.database = database;
            this.placementData = placementData;
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
            // 오브젝트 설치가 불가능한 경우
            placementValidity = CheckPlacementValidity(position, selectedObjectIndex);
            if (!placementValidity)
            {
                // 측면 설치
                position = UpdatePosition(position, collisionNormal);
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
                // if (TryGetComponent)
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

        public void UpdateState(Vector3 position, Vector3 objectNormal)
        {
            // 오브젝트 배치 가능 유무 검사
            placementValidity = CheckPlacementValidity(position, selectedObjectIndex);

            if (!placementValidity)
            {
                position = UpdatePosition(position, objectNormal);
                if (CheckPlacementValidity(position, selectedObjectIndex) == false)
                {
                    previewSystem.UpdatePosition(position, false);
                    return;
                }
            }

            // 미리보기 오브젝트 갱신
            previewSystem.UpdatePosition(position, true);
        }

        /// <summary>
        /// 충돌한 오브젝트의 법선으로 배치할 오브젝트 위치 계산
        /// </summary>
        private Vector3 UpdatePosition(Vector3 position, Vector3 objectNormal)
        {
            collisionNormal = objectNormal;

            // 충돌한 오브젝트 갱신
            collisionPosition = placementData.GetPlaceObjectPosition();
            collisionObjectID = placementData.GetPlacedObjectID(collisionPosition);
            collisionObjectIndex = database.objectData.FindIndex(data => data.ID == collisionObjectID);
            if (collisionObjectIndex == -1)
            {
                return position;
            }

            // 측면 충돌 시 오브젝트 위치 갱신
            position += new Vector3(database.objectData[selectedObjectIndex].Size.x * objectNormal.x / 2,
                                    0f,
                                    database.objectData[selectedObjectIndex].Size.z * objectNormal.z / 2);

            return position;
        }
    }
}

#endif
