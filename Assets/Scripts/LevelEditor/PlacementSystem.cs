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
        private IBuildingState buildingState;

        private Renderer[] renderers;
        private Bounds totalBounds;

        private void Start()
        {
            gimmickStatus.gameObject.SetActive(false);
            gridVisualization.SetActive(false);
            selectedData = new GridData();

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

            buildingState = new PlacementState(objectIDs[prefabAddress],
                                               previewSystem,
                                               database,
                                               selectedData,
                                               objectPlacer);
        }

        /// <summary>
        /// 설치된 오브젝트 제거 시작
        /// </summary>
        public void StartRemoving()
        {
            StopPlacement();
            buildingState = new RemovingState(previewSystem,
                                              selectedData,
                                              objectPlacer);
        }

        public void StartModify(KeyCode key)
        {
            StopPlacement();

            keyCode = key;
            buildingState = new ModifyState(previewSystem,
                                            selectedData,
                                            objectPlacer,
                                            gimmickStatus,
                                            editingTransformPosition,
                                            editingTransformRotation,
                                            editingTransformScale,
                                            keyCode);

            buildingState.OnAction(lastMousePosition);
        }

        /// <summary>
        /// 오브젝트 배치 및 수정
        /// </summary>
        private void PlaceStructure()
        {
            mousePosition = inputSystem.GetSelectedMapPosition();
            lastMousePosition = mousePosition;
            if (selectedData.IsPlacedObjectAt(mousePosition))
            {
                StartModify(keyCode);
                return;
            }

            if (inputSystem.IsPointerOverUI() || buildingState == null)
            {
                return;
            }

            buildingState.OnAction(mousePosition);
        }

        /// <summary>
        /// 오브젝트 배치 중지
        /// </summary>
        private void StopPlacement()
        {
            if (buildingState == null)
            {
                return;
            }

            buildingState.EndState();
            buildingState = null;
        }

        /// <summary>
        /// 프리팹 전체 크기 반환 (Box Collider 형태)
        /// </summary>
        /// <returns></returns>
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

        private void Update()
        {
            // 오브젝트가 없을 경우 종료
            if (buildingState == null)
            {
                return;
            }

            // 마우스 위치 및 마우스 위치에 따른 오브젝트 위치 갱신
            mousePosition = inputSystem.GetSelectedMapPosition();
            objectNormal = inputSystem.GetSelectedMapDirection();
            buildingState.UpdateState(mousePosition, objectNormal);
        }
    }
}

#endif
