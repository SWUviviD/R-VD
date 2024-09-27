#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    public class PreviewSystem : MonoBehaviour
    {
        [SerializeField] private float previewOffset = 0.06f;
        [SerializeField] private GameObject cellIndicator;
        [SerializeField] private Material previewMaterialPrefab;

        private GameObject previewObject;
        private Material previewMaterialInstance;
        private Renderer cellIndicatorRenderer;

        private Renderer[] renderers;
        private Material[] materials;
        private Color color;

        private void Start()
        {
            previewMaterialInstance = new Material(previewMaterialPrefab);
            cellIndicator.SetActive(false);
            cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        }

        public void StartShowingPlacementPreview(GameObject prefab, Vector3Int size)
        {
            previewObject = Instantiate(prefab);
            PreparePreview(previewObject);
            PrepareCursor(size);
            cellIndicator.SetActive(true);
        }

        private void PrepareCursor(Vector3Int size)
        {
            if (size.x > 0 || size.y > 0 || size.z > 0)
            {
                cellIndicator.transform.localScale = new Vector3(size.x, size.y, size.z);
                cellIndicatorRenderer.material.mainTextureScale = new Vector2(size.x, size.z);
            }
        }

        private void PreparePreview(GameObject previewObject)
        {
            renderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                materials = renderer.materials;
                for (int i = 0; i < materials.Length; ++i)
                {
                    materials[i] = previewMaterialInstance;
                }
                renderer.materials = materials;
            }
        }

        public void StopShowingPreview()
        {
            cellIndicator.SetActive(false);
            if (previewObject != null)
            {
                Destroy(previewObject);
            }
        }

        public void StartShowingRemovePreview()
        {
            cellIndicator.SetActive(true);
            PrepareCursor(Vector3Int.one);
            ApplyFeedbackToCursor(false);
        }

        public void UpdatePosition(Vector3 position, bool validity)
        {
            if (previewObject != null)
            {
                MovePreview(position);
                ApplyFeedbackToPreview(validity);
            }
            MoveCursor(position);
            ApplyFeedbackToCursor(validity);
        }

        private void MovePreview(Vector3 position)
        {
            previewObject.transform.position = new Vector3(position.x, position.y + previewOffset, position.z);
        }

        private void MoveCursor(Vector3 position)
        {
            cellIndicator.transform.position = position;
        }

        private void ApplyFeedback(bool validity)
        {
            Color c = validity ? Color.white : Color.red;
            cellIndicatorRenderer.material.color = c;
            c.a = 0.5f;
            previewMaterialInstance.color = c;
        }

        private void ApplyFeedbackToPreview(bool validity)
        {
            color = validity ? Color.white : Color.red;
            color.a = 0.5f;
            previewMaterialInstance.color = color;
        }

        private void ApplyFeedbackToCursor(bool validity)
        {
            color = validity ? Color.white : Color.red;
            color.a = 0.5f;
            cellIndicatorRenderer.material.color = color;
        }
    }
}

#endif
