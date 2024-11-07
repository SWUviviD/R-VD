#if UNITY_EDITOR

using System.Collections.Generic;
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
        private List<Transform> scaleObjects = new List<Transform>();
        private float editorDistance = 7f;

        private bool isDragging;
        private Transform scaledObject;
        private Transform selectedObject;
        private Vector3 initialMousePos;
        private Vector3 offset;
        private Vector3 delta;
        private Vector3 mousePos;
        private Vector3 curScale;
        private Vector3 newScale;
        private Vector3 editorScale;
        private RaycastHit hit;
        private Ray ray;

        private void Awake()
        {
            scaleObjects.Add(cubeCenter);
            scaleObjects.Add(scaleX);
            scaleObjects.Add(scaleY);
            scaleObjects.Add(scaleZ);
        }

        private void Update()
        {
            // 마우스 버튼이 눌렸는지 확인
            if (Input.GetMouseButtonDown(0))
            {
                if (IsEditTransformScale() && scaledObject != null)
                {
                    isDragging = true;
                    selectedObject = GetSelectedObject(hit);
                    SetSelectedObjectsActive(selectedObject);
                    curScale = scaledObject.localScale;
                    initialMousePos = Input.mousePosition;
                    offset = -GetMouseWorldPosition(transform);
                }
            }

            // 마우스 버튼이 떼어졌는지 확인
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                SetSelectedObjectsActive(null);
                transform.localScale = Vector3.one;
            }

            // 드래그 중일 때 오브젝트 이동
            if (isDragging && scaledObject != null)
            {
                GetConstrainedScale(hit);
                scaledObject.localScale = newScale;
                transform.localScale = editorScale;
            }

            if (scaledObject != null)
            {
                transform.position = UpdateEditorPosition(scaledObject.position);
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
        /// 선택된 에디터 오브젝트 반환
        /// </summary>
        private Transform GetSelectedObject(RaycastHit hit)
        {
            foreach (Transform tr in scaleObjects)
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
        /// 클릭한 오브젝트의 마우스 월드 위치 반환
        /// </summary>
        private Vector3 GetMouseWorldPosition(Transform objectTR)
        {
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(objectTR.position).z);

            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        /// <summary>
        /// 축에 따라 크기 제한
        /// </summary>
        private void GetConstrainedScale(RaycastHit hit)
        {
            mousePos = GetMouseWorldPosition(transform);
            delta = mousePos + offset;

            newScale = scaledObject.localScale;
            editorScale = Vector3.one;
            if (hit.transform == cubeCenter)
            {
                newScale = curScale * (1 + (Input.mousePosition - initialMousePos).x * 0.01f);
                if (newScale.x < limitScale.x)
                {
                    newScale = limitScale;
                }
                editorScale = Vector3.one * (1 + (Input.mousePosition - initialMousePos).x * 0.01f);
                if (editorScale.x < limitScale.x)
                {
                    editorScale = limitScale;
                }
            }
            else if (hit.transform.IsChildOf(scaleX))
            {
                newScale.x = Mathf.Max(limitScale.x, curScale.x + delta.x);
                editorScale.x = Mathf.Max(limitScale.x, 1f + delta.x);
            }
            else if (hit.transform.IsChildOf(scaleY))
            {
                newScale.y = Mathf.Max(limitScale.y, curScale.y + delta.y);
                editorScale.y = Mathf.Max(limitScale.y, 1f + delta.y);
            }
            else if (hit.transform.IsChildOf(scaleZ))
            {
                newScale.z = Mathf.Max(limitScale.z, curScale.z + delta.z);
                editorScale.z = Mathf.Max(limitScale.z, 1f + delta.z);
            }
        }

        /// <summary>
        /// 가시성을 위한 에디터 오브젝트 방향별 활성화
        /// </summary>
        private void SetSelectedObjectsActive(Transform selected)
        {
            if (selected == null || selected == cubeCenter)
            {
                scaleObjects.ForEach(_ => _.gameObject.SetActive(true));
            }
            else // selected == (scaleX, scaleY, scaleZ)
            {
                scaleObjects.ForEach(_ => _.gameObject.SetActive(false));
                selected.gameObject.SetActive(true);
            }
        }
    }
}

#endif
