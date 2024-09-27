#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor
{
    public class PlacementInputSystem : MonoBehaviour
    {
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private LayerMask placementMask;

        private Vector3 lastPosition;

        public event Action OnClicked;
        public event Action OnExit;

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

        public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

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
    }
}

#endif
