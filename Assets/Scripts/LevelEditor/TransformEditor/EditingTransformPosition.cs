#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor
{
    public class EditingTransformPosition : MonoBehaviour
    {
        [Header("Main Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Quad")]
        [SerializeField] private Transform quad;
        [SerializeField] private Transform quadX;
        [SerializeField] private Transform quadY;
        [SerializeField] private Transform quadZ;

        [Header("Position")]
        [SerializeField] private Transform arrowX;
        [SerializeField] private Transform arrowY;
        [SerializeField] private Transform arrowZ;

        [Header("Layer Mask")]
        [SerializeField] private LayerMask placementMask;

        private List<Transform> positionObjects = new List<Transform>();
        private Vector3 quadScale = Vector3.one;
        private Transform selectedObject;
        private float editorDistance = 7f;

        /// <summary>
        /// 위치가 변경된다면 호출될 함수
        /// </summary>
        private System.Action onTransformChanged;

        private bool isDragging;
        private Transform draggedObject;
        private Vector3 mousePos;
        private Vector3 curPosition;
        private Vector3 newPosition;
        private Vector3 offset;
        private RaycastHit hit;
        private Ray ray;

        private void Awake()
        {
            positionObjects.Add(quadX);
            positionObjects.Add(quadY);
            positionObjects.Add(quadZ);
            positionObjects.Add(arrowX);
            positionObjects.Add(arrowY);
            positionObjects.Add(arrowZ);
        }

        private void OnEnable()
        {
            UpdateQuadDirection();
        }

        private void Update()
        {
            // 마우스 좌클릭이 눌렸는지 확인
            if (Input.GetMouseButtonDown(0))
            {
                if (IsEditTransformPosition())
                {
                    isDragging = true;
                    selectedObject = GetSelectedObject(hit);
                    SetSelectedObjectsActive(selectedObject);
                    curPosition = draggedObject.position;
                    offset = -GetMouseWorldPosition(transform);
                }
            }

            // 마우스 좌클릭이 떼어졌는지 확인
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                SetSelectedObjectsActive(null);
            }

            // 카메라와 오브젝트 위치에 따른 쿼드 갱신
            if (Input.GetMouseButtonUp(1))
            {
                UpdateQuadDirection();
            }

            if (onTransformChanged != null && isDragging)
            {
                // 드래그 중이면서 콜백 함수가 있다면, 트랜스폼이 변경되었음을 알린다.
                onTransformChanged();
            }

            // 드래그 중일 때 오브젝트 이동
            if (isDragging && draggedObject != null)
            {
                GetConstrainedPosition(selectedObject);
                draggedObject.position = newPosition;
            }

            if (draggedObject != null)
            {
                transform.position = UpdateEditorPosition(draggedObject.position);
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
        /// 선택된 에디터 오브젝트 반환
        /// </summary>
        private Transform GetSelectedObject(RaycastHit hit)
        {
            foreach (Transform tr in positionObjects)
            {
                if (hit.transform.IsChildOf(tr))
                {
                    return tr;
                }
            }
            return null;
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
            transform.position = UpdateEditorPosition(draggedObject.position);
        }

        /// <summary>
        /// 카메라와 오브젝트 위치에 따른 쿼드 갱신
        /// </summary>
        private void UpdateQuadDirection()
        {
            quadScale.x = mainCamera.transform.position.x > transform.position.x ? 1f : -1f;
            quadScale.z = mainCamera.transform.position.z > transform.position.z ? 1f : -1f;
            quad.localScale = quadScale;
        }

        /// <summary>
        /// 클릭한 오브젝트의 마우스 월드 위치 반환
        /// </summary>
        private Vector3 GetMouseWorldPosition(Transform objectTR)
        {
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(objectTR.position).z);

            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        /// <summary>
        /// 에디터 오브젝트 위치 반환
        /// </summary>
        private Vector3 UpdateEditorPosition(Vector3 objectPos)
        {
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(objectPos);
            if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(objectPos);
                screenPosition.z = editorDistance;

                return Camera.main.ScreenToWorldPoint(screenPosition);
            }
            return objectPos;
        }

        /// <summary>
        /// 축에 따라 위치 제한
        /// </summary>
        private void GetConstrainedPosition(Transform selected)
        {
            newPosition = curPosition + (GetMouseWorldPosition(transform) + offset) *
                          (Vector3.Distance(mainCamera.transform.position, draggedObject.position) /
                           Vector3.Distance(mainCamera.transform.position, transform.position));

            if (selected == quadX)
            {
                newPosition.x = draggedObject.position.x;
            }
            else if (selected == quadY)
            {
                newPosition.y = draggedObject.position.y;
            }
            else if (selected == quadZ)
            {
                newPosition.z = draggedObject.position.z;
            }
            else if (selected == arrowX)
            {
                newPosition.y = draggedObject.position.y;
                newPosition.z = draggedObject.position.z;
            }
            else if (selected == arrowY)
            {
                newPosition.x = draggedObject.position.x;
                newPosition.z = draggedObject.position.z;
            }
            else if (selected == arrowZ)
            {
                newPosition.x = draggedObject.position.x;
                newPosition.y = draggedObject.position.y;
            }
        }

        /// <summary>
        /// 가시성을 위한 에디터 오브젝트 방향별 활성화
        /// </summary>
        private void SetSelectedObjectsActive(Transform selected)
        {
            if (selected == null)
            {
                positionObjects.ForEach(_ => _.gameObject.SetActive(true));
                return;
            }

            positionObjects.ForEach(_ => _.gameObject.SetActive(false));
            if (selected == quadX)
            {
                quadX.gameObject.SetActive(true);
                arrowY.gameObject.SetActive(true);
                arrowZ.gameObject.SetActive(true);
            }
            else if (selected == quadY)
            {
                quadY.gameObject.SetActive(true);
                arrowZ.gameObject.SetActive(true);
                arrowX.gameObject.SetActive(true);
            }
            else if (selected == quadZ)
            {
                quadZ.gameObject.SetActive(true);
                arrowX.gameObject.SetActive(true);
                arrowY.gameObject.SetActive(true);
            }
            else // selected == (arrowX, arrowY, arrowZ)
            {
                selected.gameObject.SetActive(true);
            }
        }
    }
}

#endif
