using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 경로 포인트.
/// </summary>
public class CameraPointCollider : MonoBehaviour
{
    [field: SerializeField]
    public Transform TrPointV1 { get; private set; }
    [field: SerializeField]
    public Transform TrPointV2 { get; private set; }
    [field: SerializeField]
    public Transform TrPointV3 { get; private set; }
    [field: SerializeField]
    public Transform TrPointV4 { get; private set; }
    
    [SerializeField] private NameHandleTarget handleTarget;
    [SerializeField] private NameHandleTarget pointV1HandleTarget;
    [SerializeField] private NameHandleTarget pointV2HandleTarget;
    [SerializeField] private NameHandleTarget pointV3HandleTarget;
    [SerializeField] private NameHandleTarget pointV4HandleTarget;
    
    public CameraPathPoint CameraPathPoint { get; private set; }

    private NameHandleData pointHandle;
    private NameHandleData pointV1Handle;
    private NameHandleData pointV2Handle;
    private NameHandleData pointV3Handle;
    private NameHandleData pointV4Handle;

    private Action<Transform> onClickHandle;

    private static Color[] handleColors = new Color[2] { Color.cyan, new Color(0, 0.5f, 0.4f, 1f) };
    private static Color[] textColors = new Color[2] { Color.black, Color.white };

    /// <summary>
    /// 캐싱해둔 카메라
    /// </summary>
    private Camera camera;
    
    private void Awake()
    {
        CameraPathPoint = new CameraPathPoint();

        CameraPathPoint.Position = transform.position;
        CameraPathPoint.PointV1 = TrPointV1.position;
        CameraPathPoint.PointV2 = TrPointV2.position;
        CameraPathPoint.PointV3 = TrPointV3.position;
        CameraPathPoint.PointV4 = TrPointV4.position;

        camera = Camera.main;
    }

    public void Set(int _index, Action<Transform> _onClickHandle)
    {
        var right = Camera.main.transform.right;
        var left = -right;
        var up = Camera.main.transform.up;

        float handleDist = 0.8f;

        TrPointV1.position = transform.position + left * handleDist;
        TrPointV2.position = transform.position + (left + up) * handleDist;
        TrPointV3.position = transform.position + (right + up) * handleDist;
        TrPointV4.position = transform.position + right * handleDist;
        
        onClickHandle = _onClickHandle;

        pointV1Handle = new NameHandleData("V1", handleColors[_index % 2], textColors[_index % 2], () => onClickHandle(TrPointV1));
        pointV2Handle = new NameHandleData("V2", handleColors[_index % 2], textColors[_index % 2], () => onClickHandle(TrPointV2));
        pointV3Handle = new NameHandleData("V3", handleColors[_index % 2], textColors[_index % 2], () => onClickHandle(TrPointV3));
        pointV4Handle = new NameHandleData("V4", handleColors[_index % 2], textColors[_index % 2], () => onClickHandle(TrPointV4));
        pointHandle = new NameHandleData("Point", handleColors[_index % 2], textColors[_index % 2], () => onClickHandle(transform));
        
        pointV1HandleTarget.Set(pointV1Handle);
        pointV2HandleTarget.Set(pointV2Handle);
        pointV3HandleTarget.Set(pointV3Handle);
        pointV4HandleTarget.Set(pointV4Handle);
        handleTarget.Set(pointHandle);
    }

    public Vector3 GetBezier(float _ratio)
    {
        CameraPathPoint.PointV1 = TrPointV1.position;
        CameraPathPoint.PointV2 = TrPointV2.position;
        CameraPathPoint.PointV3 = TrPointV3.position;
        CameraPathPoint.PointV4 = TrPointV4.position;
        return CameraPathPoint.GetBezier(_ratio);
    }
}
