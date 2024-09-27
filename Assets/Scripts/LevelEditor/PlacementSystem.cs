#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    public class PlacementSystem : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Grid grid;
        [SerializeField] private ObjectDatabase database;
        [SerializeField] private GameObject gridVisualization;

        [Header("Systems")]
        [SerializeField] private PlacementInputSystem inputSystem;
        [SerializeField] private PreviewSystem previewSystem;
        [SerializeField] private ObjectPlacer objectPlacer;

        private Vector3Int gridPosition;
        private Vector3Int lastDetectedPosition = Vector3Int.zero;
        private Vector3 mousePosition;

        private GridData floorData;
        private GridData furnitureData;

        private IBuildingState buildingState;

        private void Start()
        {
            gridVisualization.SetActive(false);
            floorData = new GridData();
            furnitureData = new GridData();
        }

        public void StartPlacement(int ID)
        {
            StopPlacement();
            gridVisualization.SetActive(true);
            buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, furnitureData, objectPlacer);
            inputSystem.OnClicked += PlaceStructure;
            inputSystem.OnExit += StopPlacement;
        }

        public void StartRemoving()
        {
            StopPlacement();
            gridVisualization.SetActive(true);
            buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);
            inputSystem.OnClicked += PlaceStructure;
            inputSystem.OnExit += StopPlacement;
        }

        private void PlaceStructure()
        {
            if (inputSystem.IsPointerOverUI())
            {
                return;
            }

            mousePosition = inputSystem.GetSelectedMapPosition();
            gridPosition = grid.WorldToCell(mousePosition);
            buildingState.OnAction(gridPosition);
        }

        private void StopPlacement()
        {
            if (buildingState == null)
            {
                return;
            }

            gridVisualization.SetActive(false);
            buildingState.EndState();
            inputSystem.OnClicked -= PlaceStructure;
            inputSystem.OnExit -= StopPlacement;
            lastDetectedPosition = Vector3Int.zero;
            buildingState = null;
        }

        private void Update()
        {
            if (buildingState == null)
            {
                return;
            }

            mousePosition = inputSystem.GetSelectedMapPosition();
            gridPosition = grid.WorldToCell(mousePosition);
            if (lastDetectedPosition != gridPosition)
            {
                buildingState.UpdateState(gridPosition);
                lastDetectedPosition = gridPosition;
            }
        }
    }
}

#endif
