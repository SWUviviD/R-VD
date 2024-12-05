using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using LevelEditor;
using LocalData;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// 카메라 경로를 의미하는 포인트의 정보
/// 하나의 경로는 베지어 곡선을 위한 포인트 1~4를 갖는다.
/// 이전 경로의 4번 포인트는 다음 경로의 1번 포인트와 이어진다. 
/// </summary>
public class CameraPathPoint
{
    /// <summary> 포인트의 위치 </summary>
    public Vector3 Position { get; set; }
    
    // 베지어 곡선에 필요한 값. 각 순서별로 경로의 시작위치에 가깝다.
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

    public LDCameraPointData ToLocalData()
    {
        LDCameraPointData result = new LDCameraPointData();

        result.Position = Position;
        result.PointV1 = PointV1;
        result.PointV2 = PointV2;
        result.PointV3 = PointV3;
        result.PointV4 = PointV4;
        result.RatioV1V2 = RatioV1V2;
        result.RatioV2V3 = RatioV2V3;
        result.RatioV3V4 = RatioV3V4;
        return result;
    }
}

public class CameraPathInsertSystem : MonoSingleton<CameraPathInsertSystem>
{
    [SerializeField] private CameraPathInputSystem inputSystem;
    [SerializeField] private CameraPointCollider prefabCameraPoint;
    [SerializeField] private EditingTransformPosition editingTransform; 
    
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ESC키를 누르면 현재 핸들의 위치 수정하는 오브젝트를 끈다.
            OnClickPathHandle(null);
        }
    }

    public List<LDCameraPointData> GetCameraPath()
    {
        return CameraPointList.ConvertAll(_ => _.CameraPathPoint.ToLocalData());
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
}
