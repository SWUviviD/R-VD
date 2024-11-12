#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 배치 시스템을 관리하는 클래스
    /// </summary>
    public class PlacementSystem : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ObjectDatabase database = new ObjectDatabase();
        [SerializeField] private GameObject gridVisualization;
        [SerializeField] private GameObject checkpointArea;

        [Header("Systems")]
        [SerializeField] private PlacementInputSystem inputSystem;
        [SerializeField] private PreviewSystem previewSystem;
        [SerializeField] private ObjectPlacer objectPlacer;
        [SerializeField] private GimmickStatus gimmickStatus;

        [Header("Transform Editor")]
        [SerializeField] private EditingTransformPosition editingTransformPosition;
        [SerializeField] private EditingTransformRotation editingTransformRotation;
        [SerializeField] private EditingTransformScale editingTransformScale;

        private string path = "Prefabs/Gimmick";
        private Dictionary<string, int> objectIDs = new Dictionary<string, int>();
        private KeyCode keyCode = KeyCode.Q;
        private int objectID = 0;

        private Vector3Int gridPosition;
        private Vector3Int lastDetectedPosition = Vector3Int.zero;
        private Vector3 mousePosition;
        private Vector3 lastMousePosition;
        private Vector3 objectNormal;

        private GameObject prefab;
        private Vector3 prefabSize;
        private GridData selectedData;
        private IBuildingState currentState;
        private IBuildingState modifyState;

        private Renderer[] renderers;
        private Bounds totalBounds;

        private string lastPrefabAddress;
        private float gridSize;
        private bool isGridMod;
        private bool isCheckpointMod;

        private void Start()
        {
            gimmickStatus.gameObject.SetActive(false);
            gridVisualization.SetActive(false);
            selectedData = GridData.Instance;

            inputSystem.OnClicked += PlaceStructure;
            inputSystem.OnExit += StopPlacement;
            inputSystem.OnModify += StartModify;
        }

        /// <summary>
        /// 오브젝트 ID를 통한 오브젝트 배치 시작
        /// </summary>
        public void StartPlacement(string prefabAddress)
        {
            if (gimmickStatus.gameObject.activeSelf)
            {
                return;
            }

            StopPlacement();
            lastPrefabAddress = prefabAddress;
            if (prefabAddress.IsNullOrEmpty())
            {
                return;
            }

            if (!objectIDs.ContainsKey(prefabAddress))
            {
                prefab = AddressableAssetsManager.Instance.SyncLoadObject(
                    AddressableAssetsManager.Instance.GetPrefabPath(path, $"{prefabAddress}.prefab"),
                    prefabAddress) as GameObject;

                objectID++;
                objectIDs[prefabAddress] = objectID;

                prefabSize = CalculatePrefabSize(prefab);
                database.objectData.Add(new ObjectData(prefabAddress, objectID, prefabSize, prefab));
            }

            if (!isCheckpointMod && prefabAddress == checkpointArea.name)
            {
                return;
            }

            currentState = new PlacementState(objectIDs[prefabAddress],
                                               previewSystem,
                                               database,
                                               selectedData,
                                               objectPlacer,
                                               gridSize,
                                               isGridMod);
        }

        /// <summary>
        /// 설치된 오브젝트 제거 시작
        /// </summary>
        public void StartRemoving()
        {
            StopPlacement();
            currentState = new RemovingState(previewSystem,
                                              selectedData,
                                              objectPlacer);
        }

        /// <summary>
        /// 오브젝트 수정 시작
        /// </summary>
        public void StartModify(KeyCode key)
        {
            StopPlacement();
            currentState = new ModifyState(previewSystem,
                                            selectedData,
                                            objectPlacer,
                                            gimmickStatus,
                                            editingTransformPosition,
                                            editingTransformRotation,
                                            editingTransformScale,
                                            key);

            keyCode = key;
            modifyState = currentState;
            currentState.OnAction(lastMousePosition);
        }

        /// <summary>
        /// 오브젝트 배치 및 수정 (마우스 클릭할 경우)
        /// </summary>
        private void PlaceStructure()
        {
            if (inputSystem.IsPointerOverUI() || inputSystem.IsTransformEditorAt())
            {
                return;
            }

            mousePosition = inputSystem.GetSelectedMapPosition();
            lastMousePosition = mousePosition;

            // 오브젝트 수정 상태 조건
            if (currentState == null || currentState == modifyState)
            {
                StopPlacement();
                if (selectedData.IsPlacedObjectAt(mousePosition))
                {
                    StartModify(keyCode);
                }
                return;
            }

            currentState.OnAction(mousePosition);
        }

        /// <summary>
        /// 오브젝트 배치 중지
        /// </summary>
        private void StopPlacement()
        {
            if (currentState == null)
            {
                return;
            }

            currentState.EndState();
            currentState = null;
        }

        /// <summary>
        /// 프리팹 전체 크기 반환 (Box Collider 형태)
        /// </summary>
        private Vector3 CalculatePrefabSize(GameObject prefab)
        {
            // 모든 자식들의 Renderer 컴포넌트를 가져오기
            renderers = prefab.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                // 프리팹 전체 Bounds 크기
                totalBounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    totalBounds.Encapsulate(renderers[i].bounds);
                }
                return totalBounds.size;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 그리드 형태로 오브젝트 배치 설정
        /// </summary>
        public void GridModToggle(float size, bool active)
        {
            gridSize = size;
            isGridMod = active;
            StartPlacement(lastPrefabAddress);
        }

        /// <summary>
        /// 설치된 기믹 오브젝트의 콜라이더 활성화 토글
        /// </summary>
        public void PlacedCollidersActiveToggle(bool active)
        {
            isCheckpointMod = active;
            objectPlacer.PlacedCollidersActiveToggle(active);
            StartPlacement(lastPrefabAddress);
        }

        private void Update()
        {
            // 오브젝트가 없을 경우 종료
            if (currentState == null)
            {
                return;
            }

            // 마우스 위치 및 마우스 위치에 따른 오브젝트 위치 갱신
            mousePosition = inputSystem.GetSelectedMapPosition();
            currentState.UpdateState(mousePosition);
        }

        /// <summary>
        /// 에디터에서 기믹을 로드할 때만 사용되는 함수
        /// </summary>
        public void CreateGimmick(string address, Vector3 position, Vector3 rotation, Vector3 scale, GimmickDataBase gimmick)
        {
            string gimmickName = address.Split("/")[^1].Split(".")[0];

            if (!objectIDs.ContainsKey(gimmickName))
            {
                prefab = AddressableAssetsManager.Instance.SyncLoadObject(address, address) as GameObject;

                objectIDs[gimmickName] = objectID;
                objectID++;

                prefabSize = CalculatePrefabSize(prefab);
                database.objectData.Add(new ObjectData(gimmickName, objectID, prefabSize, prefab));
            }

            prefab = database.objectData[objectIDs[gimmickName]].Prefab;
            int index = objectPlacer.PlaceObject(gimmickName,
                                                 position,
                                                 rotation,
                                                 scale,
                                                 database.objectData[objectIDs[gimmickName]].Size,
                                                 prefab);

            if (index != -1)
            {
                // 기믹 상태 데이터 생성
                GimmickStatusData statusData = null;
                if (objectPlacer.PlacedGameObjects[index].TryGetComponent(out IGimmickBase iGimmickBase))
                {
                    statusData = new GimmickStatusData(gimmickName, gimmick, iGimmickBase.SetGimmick);
                }

                // PlacementData 기믹 상태 데이터를 포함한 오브젝트 정보 생성
                GridData.Instance.AddObjectAt(statusData,
                                              position,
                                              rotation,
                                              scale,
                                              database.objectData[objectIDs[gimmickName]].ID);

            }


            //var instance = Instantiate(prefab);
            //instance.transform.position = position;
            //instance.transform.rotation = Quaternion.Euler(rotation);
            //instance.transform.localScale = scale;

            //if (instance.TryGetComponent(out IGimmickBase gimmickBase))
            //{
            //    var statusData = new GimmickStatusData(gimmickName, gimmick, gimmickBase.SetGimmick);
            //    GridData.Instance.AddObjectAt(statusData, position, rotation, scale, objectIDs[gimmickName]);
            //}
        }
    }
}

#endif
