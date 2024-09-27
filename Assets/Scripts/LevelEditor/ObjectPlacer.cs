#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class ObjectPlacer : MonoBehaviour
    {
        [SerializeField] private List<GameObject> placedGameObjects = new List<GameObject>();

        private GameObject newObject;

        public int PlaceObject(GameObject prefab, Vector3 position)
        {
            newObject = Instantiate(prefab);
            newObject.transform.position = position;
            placedGameObjects.Add(newObject);

            return placedGameObjects.Count - 1;
        }

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
