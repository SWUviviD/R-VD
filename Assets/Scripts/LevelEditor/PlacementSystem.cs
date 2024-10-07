#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 배치 시스템을 관리하는 클래스
    /// </summary>
    public class PlacementSystem : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ObjectDatabase database;
        [SerializeField] private GameObject gridVisualization;

        [Header("Systems")]
        [SerializeField] private PlacementInputSystem inputSystem;
        [SerializeField] private PreviewSystem previewSystem;
        [SerializeField] private ObjectPlacer objectPlacer;

        private Vector3Int gridPosition;
        private Vector3Int lastDetectedPosition = Vector3Int.zero;
        private Vector3 mousePosition;

        private GridData selectedData;

        private IBuildingState buildingState;

        private void Start()
        {
            gridVisualization.SetActive(false);
            selectedData = new GridData();

            inputSystem.OnClicked += PlaceStructure;
            inputSystem.OnExit += StopPlacement;
        }

        /// <summary>
        /// 오브젝트 ID를 통한 오브젝트 배치 시작
        /// </summary>
        public void StartPlacement(int ID)
        {
            StopPlacement();
            //gridVisualization.SetActive(true);
            buildingState = new PlacementState(ID, previewSystem, database, selectedData, objectPlacer);
        }

        /// <summary>
        /// 설치된 오브젝트 제거 시작
        /// </summary>
        public void StartRemoving()
        {
            StopPlacement();
            //gridVisualization.SetActive(true);
            buildingState = new RemovingState(previewSystem, selectedData, objectPlacer);
        }

        /// <summary>
        /// 오브젝트 배치
        /// </summary>
        private void PlaceStructure()
        {
            if (inputSystem.IsPointerOverUI() || buildingState == null)
            {
                return;
            }

            mousePosition = inputSystem.GetSelectedMapPosition();
            buildingState.OnAction(mousePosition);

            //mousePosition = inputSystem.GetSelectedMapPosition();
            //gridPosition = grid.WorldToCell(mousePosition);
            //buildingState.OnAction(gridPosition);
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

            gridVisualization.SetActive(false);
            buildingState.EndState();
            lastDetectedPosition = Vector3Int.zero;
            buildingState = null;
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
            Vector3 objectNormal = inputSystem.GetSelectedMapDirection();
            buildingState.UpdateState(mousePosition, objectNormal);

            //// 마우스 위치 및 마우스 위치에 따른 오브젝트 위치 갱신
            //mousePosition = inputSystem.GetSelectedMapPosition();
            //gridPosition = grid.WorldToCell(mousePosition);

            //// 마지막 그리드 위치와 현재 그리드 위치가 다른 경우 갱신
            //if (lastDetectedPosition != gridPosition)
            //{
            //    buildingState.UpdateState(gridPosition);
            //    lastDetectedPosition = gridPosition;
            //}
        }
    }
}

#endif
