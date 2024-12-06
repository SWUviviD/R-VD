using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 경로를 따라 캐릭터를 따라가는 카메라 컨트롤러.
/// 플레이어는 특정 범위를 갖는다. 그 범위안에 카메라 경로가 있다면, 카메라가 플레이어를 따라간다.
/// 경로는 직선이기 때문에 원이 닿으면 교점이 두 개가 생긴다. 두 교점중에 더 멀리(플레이어의 뒤) 있는 교점이 카메라의 위치가 된다.
/// 경로에 원이 닿았는지 검사는 2D 형태로 이루어진다. 이는 위에서 아래를 내려다 봤을때 기준으로 이루어져 있기 때문에, X축과 Z축을 기준으로 검사한다. 
/// </summary>
public class CameraController : MonoSingleton<CameraController>
{
    /// <summary>
    /// 플레이어의 트랜스폼
    /// </summary>
    private Transform trPlayerPoint;
    /// <summary>
    /// 플레이어에서부터 카메라까지의 거리
    /// </summary>
    [SerializeField] private float cameraDist;
    
    /// <summary>
    /// 카메라 경로 리스트
    /// </summary>
    public List<CameraPathPoint> CameraPointList { get; private set; }

    private bool isInit = false;

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
        isInit = true;
    }
    
    private void LateUpdate()
    {
        if (isInit == false) return;

        // 카메라의 위치를 계속 갱신한다.
        Camera.main.transform.position = GetCameraPosition(trPlayerPoint.position, cameraDist);
        Camera.main.transform.forward = (trPlayerPoint.position - Camera.main.transform.position).normalized;
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
        
        Gizmos.DrawSphere(trPlayerPoint.position, cameraDist);
    }
    
    /// <summary>
    /// 플레이어의 위치와 거리에 따른 카메라의 위치를 계산하는 함수. 
    /// </summary>
    public Vector3 GetCameraPosition(Vector3 _playerPosition, float _dist)
    {
        int prevIndex = -1;
        int currIndex = -1;

        Vector2 playerPoint = GetCameraPoint(_playerPosition);
        
        // 경로의 시작인덱스부터 경로가 플레이어의 범위 안에 들어왔는지 검사한다. 
        for (int i = 0; i < CameraPointList.Count; ++i)
        {
            var point = CameraPointList[i];
            currIndex = i;

            // 포인트가 플레이어의 범위안에 들어있는지 여부
            if (IsInCircle(GetCameraStartPoint(point), playerPoint, _dist) == false &&
                IsInCircle(GetCameraEndPoint(point), playerPoint, _dist))
            {
                // 카메라의 위치가 현재 카메라 포인트의 시작점과 끝점 사이에 있는 경우
                if (IsInCircle(GetCameraPoint(point.PointV1), playerPoint, _dist) == false &&
                    IsInCircle(GetCameraPoint(point.PointV2), playerPoint, _dist))
                {
                    return GetCameraPath(point.PointV1, point.PointV2, _playerPosition, _dist);
                }
                if (IsInCircle(GetCameraPoint(point.PointV2), playerPoint, _dist) == false &&
                    IsInCircle(GetCameraPoint(point.PointV3), playerPoint, _dist))
                {
                    return GetCameraPath(point.PointV2, point.PointV3, _playerPosition, _dist);
                }
                if (IsInCircle(GetCameraPoint(point.PointV3), playerPoint, _dist) == false &&
                    IsInCircle(GetCameraPoint(point.PointV4), playerPoint, _dist))
                {
                    return GetCameraPath(point.PointV3, point.PointV4, _playerPosition, _dist);
                }
            }

            if (prevIndex >= 0 && IsInCircle(GetCameraStartPoint(point), playerPoint, _dist))
            {
                // 카메라가 이전 마지막 지점과 현재 첫 번째 지점 사이에 있는경우
                var prevPoint = CameraPointList[prevIndex];
                return GetCameraPath(prevPoint.PointV4, point.PointV1, _playerPosition, _dist);
            }

            prevIndex = currIndex;
        }

        return CameraPointList[^1].PointV4;
    }
    
    /// <summary>
    /// 카메라 포인트의 위치를 Vector2로 구한다. Y축을 제외한 X, Z축이다.
    /// </summary>
    private Vector2 GetCameraPoint(Vector3 _point)
    {
        return new Vector2(_point.x, _point.z);
    }
    
    private Vector3 GetCameraPath(Vector3 _start, Vector3 _end, Vector3 _circle, float _radius)
    {
        // 카메라가 이전 마지막 지점과 현재 첫 번째 지점 사이에 있는경우
        var point1 = GetCameraPoint(_start);
        var point2 = GetCameraPoint(_end);
        var circle = GetCameraPoint(_circle);
        float ratio = GetLineCircleIntersection(point1, point2, circle, _radius);
        return _start + (_end - _start) * ratio;
    }
    
    /// <summary>
    /// 플레이어의 범위가 두 점 사이에 있다면 비율을 반환한다. 없다면 1을 반환한다. 
    /// </summary>
    private float GetLineCircleIntersection(Vector2 _point1, Vector2 _point2, Vector2 _circleCenter, float _circleRadius)
    {
        List<Vector2> intersections = new List<Vector2>();

        // 직선의 방향 벡터
        Vector2 d = _point2 - _point1;

        // 이차방정식의 계수 계산
        float dx = d.x;
        float dy = d.y;

        float A = dx * dx + dy * dy;
        float B = 2 * (dx * (_point1.x - _circleCenter.x) + dy * (_point1.y - _circleCenter.y));
        float C = (_point1.x - _circleCenter.x) * (_point1.x - _circleCenter.x) + (_point1.y - _circleCenter.y) * (_point1.y - _circleCenter.y) - _circleRadius * _circleRadius;

        float discriminant = B * B - 4 * A * C;

        if (discriminant == 0)
        {
            // 접점 하나
            return -B / (2 * A);
        }
        
        if(discriminant > 0)
        {
            // 교점 두 개
            float sqrtDiscriminant = Mathf.Sqrt(discriminant);

            float t1 = (-B + sqrtDiscriminant) / (2 * A);
            float t2 = (-B - sqrtDiscriminant) / (2 * A);

            return Mathf.Min(t1, t2);
        }

        return 1f;
    }
    
    /// <summary>
    /// 경로의 시작 포인트를 반환한다.
    /// </summary>
    private Vector2 GetCameraStartPoint(CameraPathPoint _pathPoint)
    {
        return GetCameraPoint(_pathPoint.PointV1);
    }

    /// <summary>
    /// 경로의 끝 포인트를 반환한다.
    /// </summary>
    private Vector2 GetCameraEndPoint(CameraPathPoint _pathPoint)
    {
        return GetCameraPoint(_pathPoint.PointV4);
    }
    
    /// <summary>
    /// 어떤 포인트가 범위 안에 들어와있는지 여부 
    /// </summary>
    private bool IsInCircle(Vector2 _position, Vector2 _circleCenter, float _circleRadius)
    {
        return (_position - _circleCenter).magnitude < _circleRadius;
    }
}
