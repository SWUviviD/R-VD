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

    private NameHandleData startPathHandle;
    private NameHandleData endPathHandle;

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

    public void Set()
    {
        startPathHandle = new NameHandleData("StartHandle", Color.cyan);
        endPathHandle = new NameHandleData("EndHandle", Color.cyan);
        
        handleTarget.Set(new NameHandleData("Point", new Color(0f, 1f, 0.8f, 1f)));
        startHandleTarget.Set(startPathHandle);
        endHandleTarget.Set(endPathHandle);
    }

    /// <summary>
    /// 베지어 곡선의 시작 지점이 수정된 경우
    /// </summary>
    public void SetStartBezierPoint(Vector3 _startPoint)
    {
        trStartBezierPoint.position = _startPoint;
        // var startLocalPoint = trStartBezierPoint.localPosition;
        // Vector3 endPoint = transform.position + new Vector3(-startLocalPoint.x, startLocalPoint.y, -startLocalPoint.z);
        // trEndBezierPoint.position = endPoint;
        CameraPathPoint.CurveStartPoint = _startPoint;
        //CameraPathPoint.CurveEndPoint = endPoint;
    }

    /// <summary>
    /// 베지어 곡선의 끝 지점이 수정된 경우
    /// </summary>
    /// <param name="_endPoint"></param>
    public void SetEndBezierPoint(Vector3 _endPoint)
    {
        trEndBezierPoint.position = _endPoint;
        // var startLocalPoint = trEndBezierPoint.localPosition;
        // Vector3 startPoint = transform.position + new Vector3(-startLocalPoint.x, startLocalPoint.y, -startLocalPoint.z);
        // trStartBezierPoint.position = startPoint;
        CameraPathPoint.CurveEndPoint = _endPoint;
        //CameraPathPoint.CurveStartPoint = startPoint;
    }
}