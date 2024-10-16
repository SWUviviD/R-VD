#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor
{
    public class EditingTransformScale : MonoBehaviour
    {
        [Header("Main Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Scale")]
        [SerializeField] private Transform cubeCenter;
        [SerializeField] private Transform scaleX;
        [SerializeField] private Transform scaleY;
        [SerializeField] private Transform scaleZ;

        [Header("Layer Mask")]
        [SerializeField] private LayerMask placementMask;

        private Vector3 limitScale = Vector3.one * 0.1f;

        private bool isDragging;
        private Transform scaledObject;
        private float deltaX;
        private float deltaY;

        private Vector3 initialMousePos;
        private Vector3 mousePos;
        private Vector3 oldScale;
        private Vector3 newScale;
        private Vector3 editorScale;
        private RaycastHit hit;
        private Ray ray;

        private void Update()
        {
            // 마우스 버튼이 눌렸는지 확인
            if (Input.GetMouseButtonDown(0))
            {
                if (IsEditTransformScale() && scaledObject != null)
                {
                    isDragging = true;
                    oldScale = scaledObject.localScale;
                    initialMousePos = Input.mousePosition;
                }
            }

            // 마우스 버튼이 떼어졌는지 확인
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                transform.localScale = Vector3.one;
            }

            // 드래그 중일 때 오브젝트 이동
            if (isDragging && scaledObject != null)
            {
                GetConstrainedScale(hit);
                scaledObject.localScale = newScale;
                transform.localScale = editorScale;
            }
        }

        /// <summary>
        /// 크기 에디터를 선택하였는지 검사
        /// </summary>
        public bool IsEditTransformScale()
        {
            // UI가 있을 경우
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            mousePos = Input.mousePosition;
            ray = mainCamera.ScreenPointToRay(mousePos);

            // Transform Position 오브젝트가 선택된 경우
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementMask) &&
                hit.transform.IsChildOf(transform))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 수정할 오브젝트 설정
        /// </summary>
        public void SetObjectTransform(Transform objectTR)
        {
            if (objectTR == null)
            {
                gameObject.SetActive(false);
                scaledObject = null;
                return;
            }

            gameObject.SetActive(true);
            scaledObject = objectTR;
            transform.position = scaledObject.position;
        }

        /// <summary>
        /// 축에 따라 크기 제한
        /// </summary>
        private void GetConstrainedScale(RaycastHit hit)
        {
            mousePos = Input.mousePosition;
            deltaX = mainCamera.WorldToScreenPoint(transform.position).x > mainCamera.WorldToScreenPoint(mousePos).x
                ? (mousePos - initialMousePos).x * 0.01f
                : (initialMousePos - mousePos).x * 0.01f;
            deltaY = (mousePos - initialMousePos).y * 0.01f;

            newScale = scaledObject.localScale;
            editorScale = Vector3.one;
            if (hit.transform == cubeCenter)
            {
                newScale = oldScale * (1 + (mousePos - initialMousePos).x * 0.01f);
                if (newScale.x < limitScale.x)
                {
                    newScale = limitScale;
                }
                editorScale = Vector3.one * (1 + (mousePos - initialMousePos).x * 0.01f);
                if (editorScale.x < limitScale.x)
                {
                    editorScale = limitScale;
                }
            }
            else if (hit.transform.IsChildOf(scaleX))
            {
                newScale.x = Mathf.Max(limitScale.x, oldScale.x + deltaX);
                editorScale.x = Mathf.Max(limitScale.x, 1f + deltaX);
            }
            else if (hit.transform.IsChildOf(scaleY))
            {
                newScale.y = Mathf.Max(limitScale.y, oldScale.y + deltaY);
                editorScale.y = Mathf.Max(limitScale.y, 1f + deltaY);
            }
            else if (hit.transform.IsChildOf(scaleZ))
            {
                newScale.z = Mathf.Max(limitScale.z, oldScale.z - deltaX);
                editorScale.z = Mathf.Max(limitScale.z, 1f - deltaX);
            }
        }
    }
}

#endif
