#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor
{
    public class EditingTransformPosition : MonoBehaviour
    {
        [Header("Main Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Position X")]
        [SerializeField] private Transform quadX;
        [SerializeField] private Transform arrowX;

        [Header("Position Y")]
        [SerializeField] private Transform quadY;
        [SerializeField] private Transform arrowY;

        [Header("Position Z")]
        [SerializeField] private Transform quadZ;
        [SerializeField] private Transform arrowZ;

        [Header("Layer Mask")]
        [SerializeField] private LayerMask placementMask;

        private Plane plane = new Plane(Vector3.one, Vector3.zero);

        /// <summary>
        /// 위치가 변경된다면 호출될 함수
        /// </summary>
        private System.Action onTransformChanged;
        
        private bool isDragging;
        private Vector3 offset;
        private Transform draggedObject;
        private float rayDistance;

        private Vector3 mousePos;
        private Vector3 newPosition;
        private RaycastHit hit;
        private Ray ray;

        private void Update()
        {
            // 마우스 버튼이 눌렸는지 확인
            if (Input.GetMouseButtonDown(0))
            {
                if (IsEditTransformPosition())
                {
                    isDragging = true;
                    offset = draggedObject.position - GetMouseWorldPosition(plane);
                }
            }

            // 마우스 버튼이 떼어졌는지 확인
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (onTransformChanged != null && isDragging)
            {
                // 드래그 중이면서 콜백 함수가 있다면, 트랜스폼이 변경되었음을 알린다.
                onTransformChanged();
            }

            // 드래그 중일 때 오브젝트 이동
            if (isDragging && draggedObject != null)
            {
                GetConstrainedPosition(hit);
                draggedObject.position = newPosition;
                transform.position = newPosition;
            }
        }

        /// <summary>
        /// 위치 에디터를 선택하였는지 검사
        /// </summary>
        public bool IsEditTransformPosition()
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
        public void SetObjectTransform(Transform objectTR, System.Action _cbTransformChanged = null)
        {
            if (objectTR == null)
            {
                gameObject.SetActive(false);
                draggedObject = null;
                return;
            }

            onTransformChanged = _cbTransformChanged;
            gameObject.SetActive(true);
            draggedObject = objectTR;
            transform.position = draggedObject.position;
        }

        /// <summary>
        /// 마우스의 월드 위치 반환
        /// </summary>
        private Vector3 GetMouseWorldPosition(Plane plane)
        {
            mousePos = Input.mousePosition;
            ray = mainCamera.ScreenPointToRay(mousePos);
            if (plane.Raycast(ray, out rayDistance))
            {
                return ray.GetPoint(rayDistance);
            }

            return Vector3.zero;
        }

        /// <summary>
        /// 축에 따라 위치 제한
        /// </summary>
        private void GetConstrainedPosition(RaycastHit hit)
        {
            newPosition = GetMouseWorldPosition(plane) + offset;

            if (hit.transform.IsChildOf(quadX))
            {
                newPosition.x = draggedObject.position.x;
            }
            else if (hit.transform.IsChildOf(quadY))
            {
                newPosition.y = draggedObject.position.y;
            }
            else if (hit.transform.IsChildOf(quadZ))
            {
                newPosition.z = draggedObject.position.z;
            }
            else if (hit.transform.IsChildOf(arrowX))
            {
                newPosition.y = draggedObject.position.y;
                newPosition.z = draggedObject.position.z;
            }
            else if (hit.transform.IsChildOf(arrowY))
            {
                newPosition.x = draggedObject.position.x;
                newPosition.z = draggedObject.position.z;
            }
            else if (hit.transform.IsChildOf(arrowZ))
            {
                newPosition.x = draggedObject.position.x;
                newPosition.y = draggedObject.position.y;
            }
        }
    }
}

#endif
