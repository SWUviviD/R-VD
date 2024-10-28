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

    [SerializeField] private NameHandleTarget startHandleTarget;
    [SerializeField] private NameHandleTarget endHandleTarget;
    
    public Transform TrStartBezierPoint => trStartBezierPoint;
    public Transform TrEndBezierPoint => trEndBezierPoint;
    
    public CameraPathPoint CameraPathPoint { get; private set; }

    private NameHandleData startPathHandle;
    private NameHandleData endPathHandle;

    /// <summary>
    /// 캐싱해둔 카메라
    /// </summary>
    private Camera camera;
    
    private void Awake()
    {
        CameraPathPoint = new CameraPathPoint();
        trStartBezierPoint.position = Vector3.zero;
        trEndBezierPoint.position = Vector3.zero;

        camera = Camera.main;
    }

    public void Set()
    {
        startPathHandle = new NameHandleData("StartHandle", Color.cyan);
        endPathHandle = new NameHandleData("EndHandle", Color.cyan);
        
        startHandleTarget.Set(startPathHandle);
        endHandleTarget.Set(endPathHandle);
    }

    /// <summary>
    /// 베지어 곡선의 시작 지점이 수정된 경우
    /// </summary>
    public void SetStartBezierPoint(Vector3 _startPoint)
    {
        Vector3 endPoint = new Vector3(-_startPoint.x, _startPoint.y, -_startPoint.z);
        CameraPathPoint.CurveStartPoint = _startPoint;
        CameraPathPoint.CurveEndPoint = endPoint;
        trStartBezierPoint.position = _startPoint;
        trEndBezierPoint.position = endPoint;
    }

    /// <summary>
    /// 베지어 곡선의 끝 지점이 수정된 경우
    /// </summary>
    /// <param name="_endPoint"></param>
    public void SetEndBezierPoint(Vector3 _endPoint)
    {
        SetStartBezierPoint(new Vector3(-_endPoint.x, _endPoint.y, -_endPoint.z));
    }
}
