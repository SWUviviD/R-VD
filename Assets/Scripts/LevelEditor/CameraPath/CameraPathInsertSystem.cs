using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using LevelEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// 카메라 경로를 의미하는 포인트의 정보
/// </summary>
public class CameraPathPoint
{
    /// <summary> 포인트의 위치 </summary>
    public Vector3 Position { get; set; }
    
    // 베지어 곡선에 필요한 값들
    public Vector3 PointV1 { get; set; }
    public Vector3 PointV2 { get; set; }
    public Vector3 PointV3 { get; set; }
    public Vector3 PointV4 { get; set; }
    
    // 각 지점 사이의 비율. 카메라의 위치를 빠르게 계산하기 위해 미리 계산해놓는다.
    public float RatioV1V2 { get; set; }
    public float RatioV2V3 { get; set; }
    public float RatioV3V4 { get; set; }

    public Vector3 GetBezier(float _t)
    {
        _t = Mathf.Clamp(_t, 0f, 1f);

        Vector3 v1v2 = PointV1 + (PointV2 - PointV1) * _t;
        Vector3 v2v3 = PointV2 + (PointV3 - PointV2) * _t;
        Vector3 v3v4 = PointV3 + (PointV4 - PointV3) * _t;

        Vector3 v1v2tov2v3 = v1v2 + (v2v3 - v1v2) * _t;
        Vector3 v2v3tov3v4 = v2v3 + (v3v4 - v2v3) * _t;

        return v1v2tov2v3 + (v2v3tov3v4 - v1v2tov2v3) * _t;
    }
}

public class CameraPathInsertSystem : MonoBehaviour
{
    [SerializeField] private CameraPathInputSystem inputSystem;
    [SerializeField] private CameraPointCollider prefabCameraPoint;
    [SerializeField] private EditingTransformPosition editingTransform; 
    
    [SerializeField] private LineRenderer pathRenderer;
    
    [SerializeField] private Text txtState;

    [SerializeField] private Transform trPlayerPoint;
    [SerializeField] private float cameraDist;
    
    public GimmickDefines.CameraPathInsertMode InsertMode { get; private set; }

    /// <summary>
    /// 카메라 경로 리스트. 맵 저장 시 저장되어야 함.
    /// </summary>
    public List<CameraPointCollider> CameraPointList { get; private set; }

    /// <summary>
    /// 카메라 경로 곡선을 표현하기 위한 꼭짓점 개수
    /// </summary>
    public const int CameraPathCurveVertexCount = 8;

    private void Awake()
    {
        inputSystem.OnClickScreen = OnClickScreen;
        CameraPointList = new List<CameraPointCollider>();
        SetInsertMode(GimmickDefines.CameraPathInsertMode.None);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ESC키를 누르면 현재 핸들의 위치 수정하는 오브젝트를 끈다.
            OnClickPathHandle(null);
        }

        if (trPlayerPoint.gameObject.activeSelf == false) return;

        Camera.main.transform.position = GetCameraPosition(trPlayerPoint.position, cameraDist);
    }

    public void SetInsertMode(GimmickDefines.CameraPathInsertMode _insertMode)
    {
        InsertMode = _insertMode;
        txtState.text = InsertMode.ToString();
    }

    private void OnClickScreen(Vector3 _position)
    {
        if (InsertMode == GimmickDefines.CameraPathInsertMode.None) return;

        switch (InsertMode)
        {
            case GimmickDefines.CameraPathInsertMode.Add:
                AddPath(_position);
                break;
            case GimmickDefines.CameraPathInsertMode.Insert:
                InsertPath(_position);
                break;
        }

        SetInsertMode(GimmickDefines.CameraPathInsertMode.None);
    }

    private void AddPath(Vector3 _position)
    {
        var cameraPoint = Instantiate(prefabCameraPoint, _position, Quaternion.identity, transform);
        
        cameraPoint.Set(CameraPointList.Count, OnClickPathHandle);
        CameraPointList.Add(cameraPoint);
        
        RefreshLineDrawer();
    }

    private void InsertPath(Vector3 _position)
    {
        
    }

    /// <summary>
    /// 카메라 경로를 표기하는 핸들이 클릭된경우, 트랜스폼을 설정할 수 있도록 설정한다.
    /// </summary>
    private void OnClickPathHandle(Transform _transform)
    {
        editingTransform.SetObjectTransform(_transform, RefreshLineDrawer);
    }

    /// <summary>
    /// 라인렌더러를 그리기 위한 데이터 갱신
    /// </summary>
    private void RefreshLineDrawer()
    {
        int cameraPositionCount = CameraPointList.Count * CameraPathCurveVertexCount;
        pathRenderer.positionCount = cameraPositionCount;

        // 버텍스 곡선 증가량
        float incrementRatio = 1f / (CameraPathCurveVertexCount - 1);
        
        for (int i = 0; i < CameraPointList.Count; ++i)
        {
            CameraPointList[i].Refresh();
            
            pathRenderer.SetPosition(i * CameraPathCurveVertexCount, CameraPointList[i].TrPointV1.position);
            
            // 버텍스를 찍을 곡선의 비율
            float ratio = 0f;

            for (int j = 0; j < CameraPathCurveVertexCount; ++j)
            {
                // 곡선 위치 구하기
                var position = CameraPointList[i].GetBezier(ratio);
                pathRenderer.SetPosition(i * CameraPathCurveVertexCount + j, position);
                ratio += incrementRatio;
            }
        }
    }

    /// <summary>
    /// 플레이어의 위치와 거리에 따른 카메라의 위치를 계산하는 함수. 
    /// </summary>
    public Vector3 GetCameraPosition(Vector3 _playerPosition, float _dist)
    {
        int prevIndex = -1;
        int currIndex = -1;

        Vector2 playerPoint = GetCameraPoint(_playerPosition);
        
        for (int i = 0; i < CameraPointList.Count; ++i)
        {
            var point = CameraPointList[i];
            currIndex = i;

            if (IsInCircle(GetCameraStartPoint(point.CameraPathPoint), playerPoint, _dist) == false &&
                IsInCircle(GetCameraEndPoint(point.CameraPathPoint), playerPoint, _dist))
            {
                // 카메라의 위치가 현재 카메라 포인트의 시작점과 끝점 사이에 있는 경우
                if (IsInCircle(GetCameraPoint(point.CameraPathPoint.PointV1), playerPoint, _dist) == false &&
                    IsInCircle(GetCameraPoint(point.CameraPathPoint.PointV2), playerPoint, _dist))
                {
                    return GetCameraPath(point.CameraPathPoint.PointV1, point.CameraPathPoint.PointV2, _playerPosition, _dist);
                }
                if (IsInCircle(GetCameraPoint(point.CameraPathPoint.PointV2), playerPoint, _dist) == false &&
                    IsInCircle(GetCameraPoint(point.CameraPathPoint.PointV3), playerPoint, _dist))
                {
                    return GetCameraPath(point.CameraPathPoint.PointV2, point.CameraPathPoint.PointV3, _playerPosition, _dist);
                }
                if (IsInCircle(GetCameraPoint(point.CameraPathPoint.PointV3), playerPoint, _dist) == false &&
                    IsInCircle(GetCameraPoint(point.CameraPathPoint.PointV4), playerPoint, _dist))
                {
                    return GetCameraPath(point.CameraPathPoint.PointV3, point.CameraPathPoint.PointV4, _playerPosition, _dist);
                }
            }

            if (prevIndex >= 0 && IsInCircle(GetCameraStartPoint(point.CameraPathPoint), playerPoint, _dist))
            {
                // 카메라가 이전 마지막 지점과 현재 첫 번째 지점 사이에 있는경우
                var prevPoint = CameraPointList[prevIndex];
                return GetCameraPath(prevPoint.CameraPathPoint.PointV4, point.CameraPathPoint.PointV1, _playerPosition, _dist);
            }

            prevIndex = currIndex;
        }

        return CameraPointList[^1].CameraPathPoint.PointV4;
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
    /// 카메라 포인트의 위치를 Vector2로 구한다. Y축을 제외한 X, Z축이다.
    /// </summary>
    private Vector2 GetCameraPoint(Vector3 _point)
    {
        return new Vector2(_point.x, _point.z);
    }
    
    private Vector2 GetCameraStartPoint(CameraPathPoint _pathPoint)
    {
        return GetCameraPoint(_pathPoint.PointV1);
    }

    private Vector2 GetCameraEndPoint(CameraPathPoint _pathPoint)
    {
        return GetCameraPoint(_pathPoint.PointV4);
    }

    private bool IsInCircle(Vector2 _position, Vector2 _circleCenter, float _circleRadius)
    {
        return (_position - _circleCenter).magnitude < _circleRadius;
    }

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
}
