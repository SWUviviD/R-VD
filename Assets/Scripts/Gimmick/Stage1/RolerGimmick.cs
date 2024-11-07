using System.Collections;
using UnityEngine;

public class RolerGimmick : GimmickBase<RolerGimmickData>
{
    private float rotationSpeed;
    private Vector3 initialPosition;
    private Rigidbody rb;

    protected override void Init()
    {
        // 추가 작업 없음
    }

    public override void SetGimmick()
    {
        LogManager.Log("rotationDuration before: " + gimmickData.rotationDuration);
        rotationSpeed = 360f / gimmickData.rotationDuration;
        LogManager.Log("rotationSpeed after: " + rotationSpeed);
    }

    private void Start()
    {
        SetGimmick();

        // 초기 위치를 오브젝트의 현재 위치로 설정
        initialPosition = transform.position;

        // Rigidbody 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();

        // 로그로 회전 속도 및 초기 위치 확인
        LogManager.Log("rotationSpeed: " + rotationSpeed);
        LogManager.Log("initialPosition: " + initialPosition);
    }

    private void FixedUpdate()
    {
        // 시계방향/반시계방향 설정
        float direction = gimmickData.rotateClockwise ? 1f : -1f;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, direction * rotationSpeed * Time.fixedDeltaTime, 0));

    }

    private void OnCollisionStay(Collision collision)
    {
        // 충돌 중일 때, 오브젝트를 원래 위치로 고정
        transform.position = initialPosition;
    }
}
