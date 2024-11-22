#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 오브젝트 배치를 관리하는 클래스
    /// </summary>
    public class ObjectPlacer : MonoBehaviour
    {
        [SerializeField] private GameObject placedArea;
        [SerializeField] private GameObject checkpointArea;

        /// <summary> 배치된 오브젝트 리스트 </summary>
        public List<GameObject> PlacedGameObjects { get; private set; } = new List<GameObject>();
        /// <summary> 배치된 기믹 오브젝트 리스트 </summary>
        public List<GameObject> PlacedGimmickObjects { get; private set; } = new List<GameObject>();
        /// <summary> 배치된 체크포인트 리스트 </summary>
        public List<GameObject> PlacedCheckpoints { get; private set; } = new List<GameObject>();
        /// <summary> 배치된 오브젝트 인덱스 딕셔너리 </summary>
        public Dictionary<Transform, int> PlacedObjectIndexs { get; private set; } = new Dictionary<Transform, int>();
        /// <summary> 배치된 기믹 오브젝트 콜라이더 딕셔너리 </summary>
        private Dictionary<GameObject, Collider> placedAreaColliders = new Dictionary<GameObject, Collider>();

        private List<Renderer> placedAreaRenderers = new List<Renderer>();
        private GameObject newObject;
        private GameObject areaObject;
        private bool isShowArea;

        /// <summary>
        /// 새로운 오브젝트 배치
        /// </summary>
        public int PlaceObject(string name, Vector3 position, Vector3 rotation, Vector3 scale, Vector3 center, Vector3 objScale, GameObject prefab)
        {
            if (objScale == Vector3.zero)
            {
                return -1;
            }

            newObject = Instantiate(prefab);
            newObject.transform.position = position - center + new Vector3(0f, objScale.y / 2f, 0f);
            newObject.transform.rotation = Quaternion.Euler(rotation);
            newObject.transform.localScale = scale;
            PlacedGameObjects.Add(newObject);

            areaObject = Instantiate(placedArea);
            areaObject.transform.position = position;
            areaObject.transform.localScale = objScale;
            placedAreaRenderers.Add(areaObject.GetComponentInChildren<Renderer>());
            placedAreaRenderers[PlacedGameObjects.Count - 1].enabled = isShowArea;
            areaObject.transform.SetParent(newObject.transform);

            if (name == checkpointArea.name)
            {
                PlacedCheckpoints.Add(newObject);
            }
            else if (newObject.TryGetComponent(out IGimmickBase iGimmickBase))
            {
                PlacedGimmickObjects.Add(newObject);
                placedAreaColliders.Add(areaObject, areaObject.GetComponentInChildren<Collider>());
            }

            PlacedObjectIndexs[newObject.transform] = PlacedGameObjects.Count - 1;

            return PlacedGameObjects.Count - 1;
        }

        /// <summary>
        /// 특정 인덱스의 오브젝트 제거
        /// </summary>
        public void RemoveObjectAt(int objectIndex)
        {
            if (PlacedGameObjects.Count <= objectIndex || PlacedGameObjects[objectIndex] == null)
            {
                return;
            }

            Destroy(PlacedGameObjects[objectIndex]);
            placedAreaRenderers[objectIndex] = null;
            PlacedGameObjects[objectIndex] = null;
        }

        /// <summary>
        /// 설치된 오브젝트 영역 표시 설정
        /// </summary>
        public void ShowPlacedAreas(bool active)
        {
            isShowArea = active;
            foreach (var area in placedAreaRenderers)
            {
                if (area == null) continue;
                area.enabled = active;
            }
        }

        /// <summary>
        /// 설치된 기믹 오브젝트의 콜라이더 활성화 토글
        /// </summary>
        public void PlacedCollidersActiveToggle(bool active)
        {
            foreach (var collider in placedAreaColliders.Values)
            {
                if (collider == null) continue;
                collider.enabled = !active;
            }

            foreach (var checkpoint in PlacedCheckpoints)
            {
                if (checkpoint == null) continue;
                checkpoint.SetActive(active);
            }
        }
    }
}

#endif
