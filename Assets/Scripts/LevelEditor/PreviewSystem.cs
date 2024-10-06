#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 배치 미리보기 시스템을 관리하는 클래스
    /// </summary>
    public class PreviewSystem : MonoBehaviour
    {
        /// <summary> 미리보기 오프셋 값 </summary>
        [SerializeField] private float previewOffset = 0.06f;

        /// <summary> 오브젝트 위치와 설치 가능 유무를 색으로 나타내는 셀 </summary>
        [SerializeField] private GameObject cellIndicator;

        /// <summary> 미리보기용 반투명 머터리얼 </summary>
        [SerializeField] private Material previewMaterialPrefab;

        private GameObject previewObject;
        private Material previewMaterialInstance;
        private Renderer cellIndicatorRenderer;

        private Renderer[] renderers;
        private Material[] materials;
        private Collider[] colliders;
        private Color color;

        private void Start()
        {
            previewMaterialInstance = new Material(previewMaterialPrefab);
            cellIndicator.SetActive(false);
            cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        }

        /// <summary>
        /// 오브젝트 배치 미리보기 시작
        /// </summary>
        public void StartShowingPlacementPreview(GameObject prefab, Vector3Int size)
        {
            previewObject = Instantiate(prefab);
            PreparePreview(previewObject);
            PrepareCollider(previewObject);
            PrepareCursor(size);
            cellIndicator.SetActive(true);
        }

        /// <summary>
        /// 오브젝트의 크기에 맞게 셀 크기 준비
        /// </summary>
        private void PrepareCursor(Vector3Int size)
        {
            if (size.x > 0 || size.y > 0 || size.z > 0)
            {
                cellIndicator.transform.localScale = new Vector3(size.x, size.y, size.z);
                //cellIndicatorRenderer.material.mainTextureScale = new Vector2(size.x, size.z);
            }
        }

        /// <summary>
        /// 미리보기 오브젝트를 반투명하게 나타나도록 준비
        /// </summary>
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

        /// <summary>
        /// 미리보기 오브젝트 충돌처리 무시
        /// </summary>
        private void PrepareCollider(GameObject previewObject)
        {
            colliders = previewObject.GetComponentsInChildren<Collider>();
            foreach (Collider child in colliders)
            {
                child.enabled = false;
            }
        }

        /// <summary>
        /// 오브젝트 미리보기 중지
        /// </summary>
        public void StopShowingPreview()
        {
            cellIndicator.SetActive(false);
            if (previewObject != null)
            {
                Destroy(previewObject);
            }
        }

        /// <summary>
        /// 오브젝트 제거 미리보기
        /// </summary>
        public void StartShowingRemovePreview()
        {
            cellIndicator.SetActive(true);
            PrepareCursor(Vector3Int.one);
            ApplyFeedbackToCursor(false);
        }

        /// <summary>
        /// 미리보기 위치 업데이트
        /// </summary>
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

        /// <summary>
        /// 미리보기 오브젝트를 지정된 위치로 이동
        /// </summary>
        private void MovePreview(Vector3 position)
        {
            previewObject.transform.position = new Vector3(position.x, position.y + previewOffset, position.z);
        }

        /// <summary>
        /// 셀을 지정된 위치로 이동
        /// </summary>
        private void MoveCursor(Vector3 position)
        {
            cellIndicator.transform.position = position;
        }

        /// <summary>
        /// 미리보기 오브젝트에 오브젝트 설치 가능 유무에 따른 반투명 색 적용
        /// </summary>
        private void ApplyFeedbackToPreview(bool validity)
        {
            color = validity ? Color.white : Color.red;
            color.a = 0.5f;
            previewMaterialInstance.color = color;
        }

        /// <summary>
        /// 셀에 오브젝트 설치 가능 유무에 따른 반투명 색 적용
        /// </summary>
        private void ApplyFeedbackToCursor(bool validity)
        {
            color = validity ? Color.white : Color.red;
            color.a = 0.5f;
            cellIndicatorRenderer.material.color = color;
        }
    }
}

#endif
