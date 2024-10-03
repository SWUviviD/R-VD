using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraSetup : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

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

        // CinemachineTransposer 설정
        CinemachineTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer == null)
        {
            transposer = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
        }
        transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        transposer.m_FollowOffset = new Vector3(0, 6, -10);

        // CinemachineGroupComposer 설정
        CinemachineGroupComposer groupComposer = virtualCamera.GetCinemachineComponent<CinemachineGroupComposer>();
        if (groupComposer == null)
        {
            groupComposer = virtualCamera.AddCinemachineComponent<CinemachineGroupComposer>();
        }
        groupComposer.m_TrackedObjectOffset = new Vector3(0, 2, 0);
    }
}
