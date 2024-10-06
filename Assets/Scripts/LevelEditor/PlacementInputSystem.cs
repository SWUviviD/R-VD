#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor
{
    /// <summary>
    /// 배치 입력 시스템을 관리하는 클래스
    /// </summary>
    public class PlacementInputSystem : MonoBehaviour
    {
        /// <summary> 에디터 씬 카메라 </summary>
        [SerializeField] private Camera sceneCamera;

        /// <summary> 배치할 레이어 마스크 </summary>
        [SerializeField] private LayerMask placementMask;

        public event Action OnClicked;
        public event Action OnExit;

        private Vector3 lastPosition;
        private Vector3 mousePos;
        private RaycastHit hit;
        private Ray ray;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClicked?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnExit?.Invoke();
            }
        }

        /// <summary>
        /// 마우스 포인터가 UI 위에 있는지 여부 확인
        /// </summary>
        public bool IsPointerOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// 마우스로 선택된 맵 위치 반환
        /// </summary>
        public Vector3 GetSelectedMapPosition()
        {
            mousePos = Input.mousePosition;
            mousePos.z = sceneCamera.nearClipPlane;
            ray = sceneCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementMask))
            {
                lastPosition = hit.point;
            }

            return lastPosition;
        }

        /// <summary>
        /// 마우스로 설치된 오브젝트 반환
        /// </summary>
        public int GetSelectedPlacedObjectIndex()
        {
            mousePos = Input.mousePosition;
            mousePos.z = sceneCamera.nearClipPlane;
            ray = sceneCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementMask))
            {
                return hit.transform.GetComponent<PlacementData>().PlacedObjectIndex;
            }

            return -1;
        }
    }
}

#endif
