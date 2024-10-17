using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;
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
        
        Vector3 v1 = new Vector3(CurveStartPoint.x, 0f, CurveStartPoint.z);
        Vector3 v2 = CurveStartPoint;
        Vector3 v3 = CurveEndPoint;
        Vector3 v4 = new Vector3(CurveEndPoint.x, 0f, CurveEndPoint.z);

        Vector3 v1v2 = (v2 - v1) * _t;
        Vector3 v2v3 = (v3 - v2) * _t;
        Vector3 v3v4 = (v4 - v3) * _t;

        Vector3 v1v2tov2v3 = (v2v3 - v1v2) * _t;
        Vector3 v2v3tov3v4 = (v3v4 - v2v3) * _t;

        return (v2v3tov3v4 - v1v2tov2v3) * _t;
    }
}

public class CameraPathInsertSystem : MonoBehaviour
{
    [SerializeField] private CameraPathInputSystem inputSystem;
    [SerializeField] private CameraPointCollider prefabCameraPoint;

    [SerializeField] private Text txtState;
    
    public GimmickDefines.CameraPathInsertMode InsertMode { get; private set; }

    public List<CameraPointCollider> CameraPointList { get; private set; }

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
        CameraPointList.Add(cameraPoint);
    }

    private void InsertPath(Vector3 _position)
    {
        
    }
}
