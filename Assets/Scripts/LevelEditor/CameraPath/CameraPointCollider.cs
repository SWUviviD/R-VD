using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 경로 포인트.
/// </summary>
public class CameraPointCollider : MonoBehaviour
{
    [SerializeField] private Transform trStartBezierPoint;
    [SerializeField] private Transform trEndBezierPoint;

    [SerializeField] private NameHandleTarget handleTarget;
    [SerializeField] private NameHandleTarget startHandleTarget;
    [SerializeField] private NameHandleTarget endHandleTarget;
    
    public Transform TrStartBezierPoint => trStartBezierPoint;
    public Transform TrEndBezierPoint => trEndBezierPoint;
    
    public CameraPathPoint CameraPathPoint { get; private set; }

    private NameHandleData pointHandle;
    private NameHandleData startPathHandle;
    private NameHandleData endPathHandle;

    private Action<Transform> onClickHandle;

    /// <summary>
    /// 캐싱해둔 카메라
    /// </summary>
    private Camera camera;
    
    private void Awake()
    {
        CameraPathPoint = new CameraPathPoint();

        CameraPathPoint.Position = transform.position;
        CameraPathPoint.CurveStartPoint = trStartBezierPoint.localPosition;
        CameraPathPoint.CurveEndPoint = trEndBezierPoint.localPosition;
        
        trStartBezierPoint.localPosition = Vector3.zero;
        trEndBezierPoint.localPosition = Vector3.zero;

        camera = Camera.main;
    }

    public void Set(Action<Transform> _onClickHandle)
    {
        onClickHandle = _onClickHandle;
        
        startPathHandle = new NameHandleData("StartHandle", Color.cyan, OnClickStartHandle);
        endPathHandle = new NameHandleData("EndHandle", Color.cyan, OnClickEndHandle);
        pointHandle = new NameHandleData("Point", new Color(0f, 1f, 0.8f, 1f), OnClickPointHandle);
        
        startHandleTarget.Set(startPathHandle);
        endHandleTarget.Set(endPathHandle);
        handleTarget.Set(pointHandle);
    }

    /// <summary>
    /// 베지어 곡선의 시작 지점이 수정된 경우
    /// </summary>
    public void SetStartBezierPoint(Vector3 _startPoint)
    {
        trStartBezierPoint.position = _startPoint;
    }

    /// <summary>
    /// 베지어 곡선의 끝 지점이 수정된 경우
    /// </summary>
    public void SetEndBezierPoint(Vector3 _endPoint)
    {
        trEndBezierPoint.position = _endPoint;
    }

    private void OnClickStartHandle()
    {
        onClickHandle(trStartBezierPoint);
    }

    private void OnClickEndHandle()
    {
        onClickHandle(trEndBezierPoint);
    }

    private void OnClickPointHandle()
    {
        onClickHandle(transform);
    }

    public Vector3 GetBezier(float _ratio)
    {
        CameraPathPoint.CurveStartPoint = trStartBezierPoint.position;
        CameraPathPoint.CurveEndPoint = trEndBezierPoint.position;
        return CameraPathPoint.GetBezier(_ratio);
    }
}
