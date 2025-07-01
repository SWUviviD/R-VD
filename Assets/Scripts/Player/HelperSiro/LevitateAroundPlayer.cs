using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LevitateAroundPlayer : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 1f;

    [Header("Offsets")]
    [SerializeField] private float teleportDistance = 10f;
    [SerializeField] private float closeEnough = 1f;
    private float sqrTeleportDist;
    [SerializeField] private Vector3 baseCenterPos = new Vector3(1.5f, 1.5f, -0.5f);
    private Vector3 smoothedCenterPos;
    [SerializeField] private Vector3 rotateOffset = new Vector3(0f, -90f, 0f);

    [Header("Floating Effect")]
    [SerializeField] private float floatingTime = 1f;
    private float halfFloatingTime = 0.5f;
    [SerializeField] private Vector3 floatOffset = new Vector3(0f, 0.5f, 0f);
    private Vector3 currentFloatOffset = Vector3.zero;
    private Vector3 bottomOffset;
    private Vector3 topOffset;

    private bool isUp = false;
    private float floatElepasedTime = 0f;

    private Vector3 targetPos;

    private void Start()
    {
        sqrTeleportDist = teleportDistance * teleportDistance;

        transform.position = player.position + baseCenterPos;
        smoothedCenterPos = transform.position;
        LookAtWherePlayerFace();

        halfFloatingTime = floatingTime * 0.5f;

        bottomOffset = -floatOffset * 0.5f;
        topOffset = -bottomOffset;
    }

    private void Update()
    {
        CalTargetPos();

        if(isPlayerFarAway() == true)
        {
            TeleportToPlayer();
            return;
        }

        SetFloatOffset();

        if (isPlayerCloseEnough() == true)
        {
            LookAtWherePlayerFace();
        }
        else
        {
            MoveTowardPlayer();
        }

        SetPosition();
        
    }

    private bool isPlayerFarAway()
    {
        return (smoothedCenterPos - targetPos).sqrMagnitude > sqrTeleportDist;
    }

    private bool isPlayerCloseEnough()
    {
        return (smoothedCenterPos - targetPos).sqrMagnitude < closeEnough;
    }

    private void CalTargetPos()
    {
        targetPos = player.TransformPoint(baseCenterPos);
    }

    private void TeleportToPlayer()
    {
        transform.position = targetPos + currentFloatOffset;
        smoothedCenterPos = targetPos;

        // todo: 텔레포트 이펙트 및 사운드
    }

    private void SetFloatOffset()
    {
        floatElepasedTime += Time.deltaTime;
        if (floatElepasedTime > halfFloatingTime)
        {
            currentFloatOffset = isUp ? topOffset : bottomOffset;
            isUp = !isUp;
            floatElepasedTime = 0f;
        }

        currentFloatOffset = isUp ?
            Vector3.Lerp(bottomOffset, topOffset, floatElepasedTime / halfFloatingTime) :
            Vector3.Lerp(topOffset, bottomOffset, floatElepasedTime / halfFloatingTime);
    }

    private void MoveTowardPlayer()
    {
        smoothedCenterPos = Vector3.MoveTowards(smoothedCenterPos, targetPos, moveSpeed * Time.deltaTime);

        transform.LookAt(targetPos);
        transform.rotation = transform.rotation * Quaternion.Euler(rotateOffset);
    }

    private void SetPosition()
    {
        transform.position = smoothedCenterPos + currentFloatOffset;
    }

    private void LookAtWherePlayerFace()
    {
        transform.rotation = player.rotation * Quaternion.Euler(rotateOffset);
    }
}
