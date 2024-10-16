#if UNITY_EDITOR

using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// 빌딩 상태를 정의하는 인터페이스
    /// </summary>
    public interface IBuildingState
    {
        /// <summary> 상태 종료 함수 </summary>
        void EndState();

        /// <summary> 액션이 발생했을 때 실행되는 함수 </summary>
        void OnAction(Vector3 position);

        /// <summary> 상태가 업데이트될 때 실행되는 함수 </summary>
        void UpdateState(Vector3 position, Vector3 objectNormal);
    }
}

#endif
