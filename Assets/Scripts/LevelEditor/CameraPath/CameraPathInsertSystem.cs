using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
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
    
    /// <summary>
    /// 포인트의 곡선을 조절하는 포인트의 위치.
    /// 이 위치는 베지어 곡선을 시작하는 위치이다.
    /// </summary>
    public Vector3 CurveStartPoint { get; set; }
    
    /// <summary>
    /// 포인트의 곡선을 조절하는 포인트의 위치.
    /// 이 위치는 베지어 곡선이 끝나는 위치이다.
    /// </summary>
    public Vector3 CurveEndPoint { get; set; }

    public Vector3 GetStartPoint()
    {
        return Position + CurveStartPoint;
    }

    public Vector3 GetEndPoint()
    {
        return Position + CurveEndPoint;
    }

    public Vector3 GetBezier(float _t)
    {
        _t = Mathf.Clamp(_t, 0f, 1f);

        Vector3 StoE = (CurveEndPoint - CurveStartPoint).normalized;
        Vector3 EtoS = (CurveStartPoint - CurveEndPoint).normalized;
        float dist = (CurveStartPoint - CurveEndPoint).magnitude;
        float halfDist = dist * 0.5f;

        Vector3 v1 = Position + EtoS * halfDist;
        Vector3 v2 = CurveStartPoint;
        Vector3 v3 = CurveEndPoint;
        Vector3 v4 = Position + StoE * halfDist;

        Vector3 v1v2 = v1 + (v2 - v1) * _t;
        Vector3 v2v3 = v2 + (v3 - v2) * _t;
        Vector3 v3v4 = v3 + (v4 - v3) * _t;

        Vector3 v1v2tov2v3 = v1v2 + (v2v3 - v1v2) * _t;
        Vector3 v2v3tov3v4 = v2v3 + (v3v4 - v2v3) * _t;

        return v1v2tov2v3 + (v2v3tov3v4 - v1v2tov2v3) * _t;
    }
}

public class CameraPathInsertSystem : MonoBehaviour
{
    [SerializeField] private CameraPathInputSystem inputSystem;
    [SerializeField] private CameraPointCollider prefabCameraPoint;

    [SerializeField] private LineRenderer pathRenderer;
    
    [SerializeField] private Text txtState;
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

        var camera = Camera.main;
        
        var right = camera.transform.right;
        var left = -right;
        var up = camera.transform.up;
        
        Vector3 startBezierPoint = _position + (left + up) * 0.8f;
        Vector3 endBezierPoint = _position + (right + up) * 0.8f;
        
        cameraPoint.Set();
        cameraPoint.SetStartBezierPoint(startBezierPoint);
        cameraPoint.SetEndBezierPoint(endBezierPoint);
        CameraPointList.Add(cameraPoint);
        
        RefreshLineDrawer();
    }

    private void InsertPath(Vector3 _position)
    {
        
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
            pathRenderer.SetPosition(i * CameraPathCurveVertexCount, CameraPointList[i].CameraPathPoint.GetStartPoint());
            
            // 버텍스를 찍을 곡선의 비율
            float ratio = 0f;

            for (int j = 0; j < CameraPathCurveVertexCount; ++j)
            {
                // 곡선 위치 구하기
                var position = CameraPointList[i].CameraPathPoint.GetBezier(ratio);
                pathRenderer.SetPosition(i * CameraPathCurveVertexCount + j, position);
                ratio += incrementRatio;
            }
        }
    }
}
