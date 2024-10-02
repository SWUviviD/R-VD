using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Defines;
using Cinemachine;

public class CameraSkill : MonoBehaviour
{
    [SerializeField] public Transform player; // 플레이어의 Transform
    [SerializeField] public CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float minZoom = 0f; // 최소 줌
    [SerializeField] private float maxZoom = 10f; // 최대 줌
    [SerializeField] private float zoomSpeed = 1f; // 줌 아웃 속도
    [SerializeField] private float returnSpeed = 2f; // 원래 상태로 돌아오는 속도
    private float targetZoom; // 목표 줌
    private float zoomTime; // 현재 스킬 키 누른 시간
    private bool isKeyPressing; // 키 누르고 있는지 상태

    private void Start()
    {
        targetZoom = minZoom; // 초기 줌 설정
    }

    private void Update()
    {
        // 줌 상태 업데이트
        if (isKeyPressing)
        {
            // 스킬 키가 눌리면 줌 아웃
            zoomTime += Time.deltaTime;
            if (zoomTime > 1f) zoomTime = 1f; // 최대 시간 제한
            targetZoom = Mathf.Lerp(minZoom, maxZoom, zoomTime);
        }
        else
        {
            // 스킬 키가 떼지면 원래 상태로 돌아옴
            if (targetZoom > minZoom)
            {
                targetZoom = Mathf.MoveTowards(targetZoom, minZoom, returnSpeed * Time.deltaTime);
            }
        }

        virtualCamera.transform.localPosition = new Vector3(virtualCamera.transform.localPosition.x, virtualCamera.transform.localPosition.y, -targetZoom);
    }

    public void OnSkillStart(InputAction.CallbackContext _playerStatus)
    {
        if (_playerStatus.started)
        {
            isKeyPressing = true;
            zoomTime = 0f; // 초기화
        }
    }

    public void OnSkillStop(InputAction.CallbackContext _playerStatus)
    {
        if (_playerStatus.canceled)
        {
            isKeyPressing = false; // 스킬 키가 떼어짐
        }
    }
}
