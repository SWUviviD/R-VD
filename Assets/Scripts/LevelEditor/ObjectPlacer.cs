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
        /// <summary> 배치된 오브젝트들의 리스트 </summary>
        [SerializeField] private List<GameObject> placedGameObjects = new List<GameObject>();

        private GameObject newObject;

        /// <summary>
        /// 새로운 오브젝트 배치
        /// </summary>
        public int PlaceObject(GameObject prefab, Vector3 position)
        {
            newObject = Instantiate(prefab);
            newObject.transform.position = position;
            placedGameObjects.Add(newObject);

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
    }
}

#endif
