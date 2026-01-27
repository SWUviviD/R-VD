using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitateAroundPlayer : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Offsets")]
    [SerializeField] private float teleportDistance = 10f;
    [SerializeField] private float closeEnough = 1f; // 선형값
    private float sqrTeleportDist;
    private float sqrCloseEnough;                    // 제곱값으로 보관
    [SerializeField] private Vector3 baseCenterPos = new Vector3(1.5f, 1.5f, -0.5f);
    private Vector3 smoothedCenterPos;
    [SerializeField] private Vector3 rotateOffset = new Vector3(0f, -90f, 0f);

    [Header("Floating Effect")]
    [SerializeField] private float floatingTime = 1f;
    private float halfFloatingTime = 0.5f;
    [SerializeField] private Vector3 floatOffset = new Vector3(0f, 0.5f, 0f);
    private Vector3 currentFloatOffset = Vector3.zero;
    private Vector3 bottomOffset, topOffset;
    private bool isUp = false;
    private float floatElapsedTime = 0f;

    [Header("Rotation")]
    [SerializeField] private float rotLerpSpeed = 10f; // 회전 부드럽게

    private Vector3 targetPos;

    private void Start()
    {
        sqrTeleportDist = teleportDistance * teleportDistance;
        sqrCloseEnough = closeEnough * closeEnough;

        transform.position = player.position + baseCenterPos;
        smoothedCenterPos = transform.position;
        LookAtWherePlayerFaceImmediate();

        halfFloatingTime = floatingTime * 0.5f;
        bottomOffset = -floatOffset * 0.5f;
        topOffset = floatOffset * 0.5f;
    }

    // 플로팅(상하 운동)은 프레임 어디서 해도 무방
    private void Update()
    {
        UpdateFloatOffset();
    }

    // 추적/회전/위치 갱신은 LateUpdate에서!
    private void LateUpdate()
    {
        CalcTargetPos();

        if (IsPlayerFarAway())
        {
            TeleportToPlayer();
            return;
        }

        if (!IsPlayerCloseEnough())
        {
            smoothedCenterPos = Vector3.MoveTowards(smoothedCenterPos, targetPos, moveSpeed * Time.deltaTime);
        }
        else
        {
            // 근접 시 미세 진동 방지: 그대로 붙여두기
            smoothedCenterPos = targetPos;
        }

        // 회전: 플레이어의 바라보는 각도에 오프셋을 준 "목표 회전"으로 Slerp
        Quaternion desiredRot = player.rotation * Quaternion.Euler(rotateOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-rotLerpSpeed * Time.deltaTime));

        // 마지막에 위치 반영
        transform.position = smoothedCenterPos + currentFloatOffset;
    }

    private bool IsPlayerFarAway()
    {
        return (smoothedCenterPos - targetPos).sqrMagnitude > sqrTeleportDist;
    }

    private bool IsPlayerCloseEnough()
    {
        return (smoothedCenterPos - targetPos).sqrMagnitude < sqrCloseEnough;
    }

    private void CalcTargetPos()
    {
        // player.TransformPoint(baseCenterPos)와 동일 의미(명시적으로 작성)
        targetPos = player.position + player.rotation * baseCenterPos;
    }

    private void TeleportToPlayer()
    {
        smoothedCenterPos = targetPos;
        transform.position = targetPos + currentFloatOffset;
        // TODO: 텔레포트 이펙트 & 사운드
    }

    private void UpdateFloatOffset()
    {
        floatElapsedTime += Time.deltaTime;
        if (floatElapsedTime > halfFloatingTime)
        {
            isUp = !isUp;
            floatElapsedTime = 0f;
        }

        currentFloatOffset = isUp
            ? Vector3.Lerp(bottomOffset, topOffset, floatElapsedTime / halfFloatingTime)
            : Vector3.Lerp(topOffset, bottomOffset, floatElapsedTime / halfFloatingTime);
    }

    private void LookAtWherePlayerFaceImmediate()
    {
        transform.rotation = player.rotation * Quaternion.Euler(rotateOffset);
    }

    public void SetTargetPlayer(Transform target)
    {
        player = target;
    }
}
