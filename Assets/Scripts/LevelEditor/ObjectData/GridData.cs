#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 그리드에 오브젝트 배치를 관리하는 클래스
    /// </summary>
    public class GridData : Singleton<GridData>
    {
        /// <summary> 배치된 오브젝트 데이터를 저장하는 리스트 </summary>
        private List<PlacementData> placedObjects = new List<PlacementData>();

        /// <summary> 반환값 리스트 </summary>
        private List<Vector3> returnVal;

        /// <summary> 현재 배치할 오브젝트의 데이터 </summary>
        private PlacementData data;

        private LayerMask placedArea = LayerMask.GetMask("PlacedArea");
        private Collider[] colliders;
        private Transform[] transforms;
        private Renderer lateRenderer;
        private int count;


        /// <summary>
        /// 주어진 위치에 오브젝트를 추가
        /// </summary>
        public void AddObjectAt(GimmickStatusData gimmickStatusData, Vector3 position, Vector3 rotation, Vector3 scale, int ID)
        {
            data = new PlacementData(gimmickStatusData, position, rotation, scale, ID);
            placedObjects.Add(data);
        }

        /// <summary>
        /// 해당 위치에 오브젝트를 배치할 수 있는지 확인
        /// </summary>
        public bool CanPlaceObjectAt(Vector3 position, Vector3 objectSize)
        {
            colliders = Physics.OverlapBox(position + objectSize.y * Vector3.up / 2,
                                           objectSize * 0.99f / 2,
                                           Quaternion.identity,
                                           placedArea);
            if (colliders.Length > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 해당 위치에 오브젝트를 배치할 수 있는지 확인
        /// </summary>
        public bool TryGetCollisionedObjects(Vector3 position, Vector3 objectSize, out Transform[] transforms)
        {
            count = Physics.OverlapBoxNonAlloc(position + objectSize.y * Vector3.up / 2,
                                               objectSize * 0.99f / 2,
                                               colliders,
                                               Quaternion.identity,
                                               placedArea);

            if (count > 0)
            {
                transforms = new Transform[count];
                for (int i = 0; i < count; ++i)
                {
                    transforms[i] = colliders[i].transform.parent.parent;
                }
                return true;
            }
            transforms = null;

            return false;
        }

        /// <summary>
        /// 해당 마우스 위치에 오브젝트가 존재하는지 확인
        /// </summary>
        public bool IsPlacedObjectAt(Vector3 position)
        {
            colliders = Physics.OverlapSphere(position, 0.1f, placedArea);
            if (colliders.Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 해당 마우스 위치의 오브젝트 트랜스폼 반환
        /// </summary>
        public Transform GetObjectTransformAt(Vector3 position)
        {
            colliders = Physics.OverlapSphere(position, 0.1f, placedArea);
            if (colliders.Length > 0)
            {
                return colliders[0].transform.parent.parent;
            }
            return null;
        }

        /// <summary>
        /// 해당 오브젝트의 배치된 범위를 보여주는 토글
        /// </summary>
        public void PlacedAreaToggle(bool active)
        {
            if (lateRenderer != null)
            {
                lateRenderer.enabled = false;
            }

            if (active && colliders.Length > 0)
            {
                lateRenderer = colliders[0].gameObject.GetComponent<Renderer>();
                lateRenderer.enabled = true;
            }
        }

        /// <summary>
        /// 주어진 위치에서 오브젝트 제거
        /// </summary>
        public void RemoveObjectAt(int placedIndex)
        {
            placedObjects[placedIndex] = null;
        }

        /// <summary>
        /// 주어진 위치의 기믹 수치를 반환
        /// </summary>
        public GimmickStatusData GetGimmickStatus(int placedIndex)
        {
            return placedObjects[placedIndex].GimmickStatusData;
        }

        public int GetPlacedObjectID(int placedIndex)
        {
            return placedObjects[placedIndex].ID;
        }

        /// <summary>
        /// 기믹 데이터 반환
        /// </summary>
        public List<GimmickDataBase> GetGimmickDataBaseList()
        {
            List<GimmickDataBase> result = new List<GimmickDataBase>();
            foreach (var obj in placedObjects)
            {
                if (obj != null && obj.GimmickStatusData != null)
                {
                    result.Add(obj.GimmickStatusData.GimmickDataBase);
                }
            }

            return result;
        }
    }
}

#endif
