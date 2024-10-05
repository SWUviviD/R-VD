using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraSetup : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    /// <summary>
    /// 카메라 following 영역 조절 변수
    /// </summary>
    [SerializeField] private float deadZoneWidth = 0.1f;
    [SerializeField] private float deadZoneHeight = 0.1f;

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

        CinemachineTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer == null)
        {
            transposer = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
        }
        transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        transposer.m_FollowOffset = new Vector3(0, 6, -10);

        CinemachineGroupComposer groupComposer = virtualCamera.GetCinemachineComponent<CinemachineGroupComposer>();
        if (groupComposer == null)
        {
            groupComposer = virtualCamera.AddCinemachineComponent<CinemachineGroupComposer>();
        }
        groupComposer.m_TrackedObjectOffset = new Vector3(0, 2, 0);

        groupComposer.m_DeadZoneWidth = deadZoneWidth;
        groupComposer.m_DeadZoneHeight = deadZoneHeight;
    }
}