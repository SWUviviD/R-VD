#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 오브젝트 배치를 관리하는 클래스
    /// </summary>
    public class ObjectPlacer : MonoBehaviour
    {
        [SerializeField] private GameObject placedArea;

        /// <summary> 배치된 오브젝트들의 리스트 </summary>
        [SerializeField] private List<GameObject> placedGameObjects = new List<GameObject>();
        private Dictionary<Transform, int> placedObjectIndexs = new Dictionary<Transform, int>();
        private List<Renderer> placedAreaRenderers = new List<Renderer>();

        public List<GameObject> PlacedGameObjects => placedGameObjects;
        public Dictionary<Transform, int> PlacedObjectIndexs => placedObjectIndexs;

        private GameObject newObject;
        private GameObject areaObject;
        private bool isShowArea;

        /// <summary>
        /// 새로운 오브젝트 배치
        /// </summary>
        public int PlaceObject(Vector3 position, GameObject prefab, Vector3 scale)
        {
            if (scale == Vector3.zero)
            {
                return -1;
            }

            newObject = Instantiate(prefab);
            newObject.transform.position = position;
            placedGameObjects.Add(newObject);

            areaObject = Instantiate(placedArea);
            areaObject.transform.position = position;
            areaObject.transform.localScale = scale;
            placedAreaRenderers.Add(areaObject.GetComponentInChildren<Renderer>());
            placedAreaRenderers[placedGameObjects.Count - 1].enabled = isShowArea;
            areaObject.transform.SetParent(newObject.transform);

            placedObjectIndexs[areaObject.transform.parent] = placedGameObjects.Count - 1;

            return placedGameObjects.Count - 1;
        }

        /// <summary>
        /// 특정 인덱스의 오브젝트 제거
        /// </summary>
        public void RemoveObjectAt(int gameObjectIndex)
        {
            if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
            {
                return;
            }

            Destroy(placedGameObjects[gameObjectIndex]);
            placedGameObjects[gameObjectIndex] = null;
        }

        /// <summary>
        /// 설치된 오브젝트 영역 표시 설정
        /// </summary>
        public void ShowPlacedAreas(bool active)
        {
            isShowArea = active;
            placedAreaRenderers.ForEach(_ => _.enabled = active);
        }
    }
}

#endif
