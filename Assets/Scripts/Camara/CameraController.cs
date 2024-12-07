using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 경로를 따라 캐릭터를 따라가는 카메라 컨트롤러.
/// 플레이어는 특정 범위를 갖는다. 그 범위안에 카메라 경로가 있다면, 카메라가 플레이어를 따라간다.
/// 경로는 직선이기 때문에 원이 닿으면 교점이 두 개가 생긴다. 두 교점중에 더 멀리(플레이어의 뒤) 있는 교점이 카메라의 위치가 된다.
/// 경로에 원이 닿았는지 검사는 2D 형태로 이루어진다. 이는 위에서 아래를 내려다 봤을때 기준으로 이루어져 있기 때문에, X축과 Z축을 기준으로 검사한다. 
/// </summary>
public class CameraController : MonoSingleton<CameraController>
{
    [SerializeField] private Camera mainCamera;
    /// <summary>
    /// 플레이어의 트랜스폼
    /// </summary>
    private Transform trPlayerPoint;
    /// <summary>
    /// 플레이어의 위치에서 카메라 경로를 감지하는 범위
    /// </summary>
    [Header("플레이어의 위치에서 카메라 경로를 감지하는 범위")]
    [SerializeField] private float cameraDetectRange;

    /// <summary>
    /// 플레이어와 카메라간의 거리
    /// </summary>
    [Header("플레이어와 카메라간의 거리")]
    [SerializeField] private float cameraDistance;

    /// <summary>
    /// 카메라 초당 이동속도
    /// </summary>
    [Header("카메라 초당 이동속도")]
    [SerializeField] private float cameraMoveSpeedPerSec;
    
    /// <summary>
    /// 카메라 경로 리스트
    /// </summary>
    public List<CameraPathPoint> CameraPointList { get; private set; }

    public Camera MainCamera => mainCamera;

    private bool isInit = false;

    /// <summary> 카메라가 이동하고자 하는 위치 </summary>
    private Vector3 desiredCameraPosition;

    public void SetPlayer(Transform _trPlayer)
    {
        trPlayerPoint = _trPlayer;
    }

    /// <summary>
    /// 경로를 입력받는다. 카메라가 실질적으로 동작한다.
    /// </summary>
    public void Set(List<CameraPathPoint> _path)
    {
        CameraPointList = _path;
    }

    /// <summary>
    /// 카메라의 기능을 멈춘다.
    /// </summary>
    public void Reset()
    {
        isInit = false;
    }

    /// <summary> 카메라 기능을 동작시킨다 </summary>
    public void Play()
    {
        if (CameraPointList.IsNullOrEmpty()) return;
        
        mainCamera.transform.position = GetCameraPosition(trPlayerPoint.position);
        isInit = true;
    }
    
    private void LateUpdate()
    {
        if (isInit == false) return;

        // 카메라의 위치를 계속 갱신한다.
        desiredCameraPosition = GetCameraPosition(trPlayerPoint.position);
        float moveSpeed = cameraMoveSpeedPerSec * Time.deltaTime;
        Vector3 toDesired = desiredCameraPosition - mainCamera.transform.position;
        if (toDesired.magnitude > moveSpeed)
        {
            mainCamera.transform.position += toDesired.normalized * moveSpeed;
        }
        else
        {
            mainCamera.transform.position = desiredCameraPosition;
        }
        mainCamera.transform.forward = (trPlayerPoint.position - mainCamera.transform.position).normalized;
    }

    private void OnDrawGizmos()
    {
        if (isInit == false) return;
        
        CameraPointList[0].DrawGizmo();
        for (int i = 1, count = CameraPointList.Count; i < count; ++i)
        {
            Gizmos.DrawLine(CameraPointList[i - 1].PointV4, CameraPointList[i].PointV1);
            CameraPointList[i].DrawGizmo();
        }
        
        Gizmos.DrawSphere(trPlayerPoint.position, cameraDetectRange);
    }
    
    /// <summary>
    /// 플레이어의 위치와 거리에 따른 카메라의 위치를 계산하는 함수. 
    /// </summary>
    public Vector3 GetCameraPosition(Vector3 _playerPosition)
    {
        Vector2 playerPoint = GetCameraPoint(_playerPosition);

        Vector2 closestPoint;
        
        // 경로의 시작인덱스부터 경로가 플레이어의 범위 안에 들어왔는지 검사한다. 
        for (int i = 0, count = CameraPointList.Count; i < count; ++i)
        {
            var point = CameraPointList[i];

            Vector2 startPoint = GetCameraPoint(point.PointV1);
            Vector2 endPoint = GetCameraPoint(point.PointV4);
            
            if (IsCircleLineCollided(startPoint, endPoint, playerPoint, cameraDetectRange, out closestPoint))
            {
                float ratio = (startPoint - closestPoint).magnitude / (startPoint - endPoint).magnitude;
                if (ratio < 1.0f - float.Epsilon)
                {
                    Vector3 toBack = (point.PointV1 - point.PointV4).normalized * cameraDistance;
                    return point.GetBezier(ratio) + toBack;
                }
            }

            if (i < count - 1)
            {
                var nextPoint = CameraPointList[i + 1];
                Vector2 nextStartPoint = GetCameraPoint(nextPoint.PointV1);
                if (IsCircleLineCollided(endPoint, nextStartPoint, playerPoint, cameraDetectRange, out closestPoint))
                {
                    if (closestPoint == nextStartPoint) continue;
                    float ratio = (endPoint - closestPoint).magnitude / (endPoint - nextStartPoint).magnitude;
                    if (ratio >= 1.0f - float.Epsilon) continue;
                    Vector3 toBack = (point.PointV4 - nextPoint.PointV1).normalized * cameraDistance;
                    return point.PointV4 + (nextPoint.PointV1 - point.PointV4) * ratio + toBack;
                }
            }
        }

        return mainCamera.transform.position;
    }
    
    /// <summary>
    /// 원이 직선에 닿았는지 여부를 판단하고, 직선에서 가장 가까운 지점을 반환합니다.
    /// </summary>
    public static bool IsCircleLineCollided(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 circlePosition, float radius, out Vector2 closestPoint)
    {
        Vector2 line = lineEndPosition - lineStartPosition;
        float lineLengthSquared = line.sqrMagnitude;

        // 직선의 길이가 0인 경우, 시작 지점만을 고려
        if (lineLengthSquared == 0f)
        {
            closestPoint = lineStartPosition;
            return Vector2.Distance(circlePosition, lineStartPosition) <= radius;
        }

        // 점 위치를 직선에 투영한 t 값 계산
        float t = Vector2.Dot(circlePosition - lineStartPosition, line) / lineLengthSquared;

        // t를 [0,1] 범위로 클램프하여 직선 구간 내의 점으로 제한
        t = Mathf.Clamp01(t);

        // 가장 가까운 지점 계산
        closestPoint = lineStartPosition + t * line;

        // 점과 가장 가까운 지점 간의 거리 계산
        float distance = Vector2.Distance(circlePosition, closestPoint);

        // 거리가 반지름 이하인지 확인
        return distance <= radius;
    }
    
    /// <summary>
    /// 카메라 포인트의 위치를 Vector2로 구한다. Y축을 제외한 X, Z축이다.
    /// </summary>
    private Vector2 GetCameraPoint(Vector3 _point)
    {
        return new Vector2(_point.x, _point.z);
    }
}
