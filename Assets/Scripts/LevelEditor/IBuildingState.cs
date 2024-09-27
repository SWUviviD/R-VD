#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    public interface IBuildingState
    {
        void EndState();
        void OnAction(Vector3Int gridPosition);
        void UpdateState(Vector3Int gridPosition);
    }
}

#endif
