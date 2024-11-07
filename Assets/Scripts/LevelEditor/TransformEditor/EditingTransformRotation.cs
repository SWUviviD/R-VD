#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor
{
    public class EditingTransformRotation : MonoBehaviour
    {
        [Header("Main Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Rotation")]
        [SerializeField] private Transform sphere;
        [SerializeField] private Transform rotationX;
        [SerializeField] private Transform rotationY;
        [SerializeField] private Transform rotationZ;

        [Header("Layer Mask")]
        [SerializeField] private LayerMask placementMask;

        private List<Transform> rotationObjects = new List<Transform>();
        private float editorDistance = 7f;

        private bool isRotating = false;
        private Transform selectedObject;
        private Vector3 lastMousePosition;
        private Transform rotatedObject;

        private Vector3 mouseDelta;
        private Vector3 mousePos;
        private Vector3 newRotation;
        private RaycastHit hit;
        private Ray ray;

        private void Awake()
        {
            rotationObjects.Add(sphere);
            rotationObjects.Add(rotationX);
            rotationObjects.Add(rotationY);
            rotationObjects.Add(rotationZ);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsEditTransformRotation())
                {
                    isRotating = true;
                    selectedObject = hit.transform;
                    SetSelectedObjectsActive(selectedObject);
                    lastMousePosition = Input.mousePosition;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isRotating = false;
                selectedObject = null;
                SetSelectedObjectsActive(selectedObject);
            }

            if (isRotating && selectedObject != null)
            {
                mouseDelta = Input.mousePosition - lastMousePosition;
                lastMousePosition = Input.mousePosition;

                if (selectedObject == sphere)
                {
                    if (Vector3.Dot(rotatedObject.up, Vector3.up) >= 0)
                    {
                        rotatedObject.Rotate(rotatedObject.up, -Vector3.Dot(mouseDelta, mainCamera.transform.right), Space.World);
                    }
                    else
                    {
                        rotatedObject.Rotate(rotatedObject.up, Vector3.Dot(mouseDelta, mainCamera.transform.right), Space.World);
                    }
                    rotatedObject.Rotate(mainCamera.transform.right, Vector3.Dot(mouseDelta, mainCamera.transform.up), Space.World);
                }
                else
                {
                    newRotation = Vector3.zero;
                    if (selectedObject == rotationX)
                    {
                        newRotation.x = mouseDelta.x;
                    }
                    else if (selectedObject == rotationY)
                    {
                        newRotation.y = -mouseDelta.x;
                    }
                    else if (selectedObject == rotationZ)
                    {
                        newRotation.z = mouseDelta.x;
                    }
                    rotatedObject.Rotate(100f * Time.deltaTime * newRotation, Space.Self);
                }
                transform.rotation = rotatedObject.rotation;
            }

            if (rotatedObject != null)
            {
                transform.position = UpdateEditorPosition(rotatedObject.position);
            }
        }

        /// <summary>
        /// 회전 에디터를 선택하였는지 검사
        /// </summary>
        public bool IsEditTransformRotation()
        {
            // UI가 있을 경우
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            mousePos = Input.mousePosition;
            ray = mainCamera.ScreenPointToRay(mousePos);

            // Transform Rotation 오브젝트가 선택된 경우
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
                rotatedObject = null;
                return;
            }

            gameObject.SetActive(true);
            rotatedObject = objectTR;
            transform.position = rotatedObject.position;
            transform.rotation = rotatedObject.rotation;
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
        /// 가시성을 위한 에디터 오브젝트 방향별 활성화
        /// </summary>
        private void SetSelectedObjectsActive(Transform selected)
        {
            if (selected == null)
            {
                rotationObjects.ForEach(_ => _.gameObject.SetActive(true));
                return;
            }
            
            if (selected == sphere)
            {
                rotationObjects.ForEach(_ => _.gameObject.SetActive(true));
                selected.gameObject.SetActive(false);
            }
            else // selected == (rotationX, rotationY, rotationZ)
            {
                rotationObjects.ForEach(_ => _.gameObject.SetActive(false));
                selected.gameObject.SetActive(true);
            }
        }
    }
}

#endif
