using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraSetup : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    /// <summary>
    /// 카메라가 플레이어를 인식하는 영역 조절 변수
    /// </summary>
    [SerializeField] private float deadZoneWidth = 0.1f; // 가로 10%
    [SerializeField] private float deadZoneHeight = 0.1f; // 세로 10%

    /// <summary>
    /// 초기화 확인
    /// <summary>
    void Start()
    {
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera == null)
            {
                LogManager.LogError("CinemachineVirtualCamera가 할당되지 않았거나 이 GameObject에 존재하지 않습니다.");
                return;
            }
        }

        /// <summary>
        /// 카메라 위치 기본 세팅
        /// <summary>
        CinemachineTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer == null)
        {
            transposer = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
        }
        transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace; // 바인딩 모드 설정
        transposer.m_FollowOffset = new Vector3(0, 6, -10); // 초기 오프셋 설정

        /// <summary>
        /// 카메라 에임 기본 세팅
        /// <summary>
        CinemachineGroupComposer groupComposer = virtualCamera.GetCinemachineComponent<CinemachineGroupComposer>();
        if (groupComposer == null)
        {
            groupComposer = virtualCamera.AddCinemachineComponent<CinemachineGroupComposer>();
        }
        groupComposer.m_TrackedObjectOffset = new Vector3(0, 2, 0); // 추적 객체 오프셋 설정

        // DeadZone 설정
        groupComposer.m_DeadZoneWidth = deadZoneWidth;
        groupComposer.m_DeadZoneHeight = deadZoneHeight;
    }
}