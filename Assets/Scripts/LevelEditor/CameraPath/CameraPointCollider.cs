using System;
using System.Collections;
using System.Collections.Generic;
using LocalData;
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
    
    public int Index { get; private set; }

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
    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Set(int _index, Action<Transform> _onClickHandle)
    {
        Index = _index;
        
        var right = Camera.main.transform.right;
        var left = -right;

        float handleDist = 0.8f;

        TrPointV1.position = transform.position + handleDist * 2 * left;
        TrPointV2.position = transform.position + handleDist * left;
        TrPointV3.position = transform.position + handleDist * right;
        TrPointV4.position = transform.position + handleDist * 2 * right;
        
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
        
        CameraPathPoint = new CameraPathPoint();

        CameraPathPoint.Position = transform.position;
        CameraPathPoint.PointV1 = TrPointV1.position;
        CameraPathPoint.PointV2 = TrPointV2.position;
        CameraPathPoint.PointV3 = TrPointV3.position;
        CameraPathPoint.PointV4 = TrPointV4.position;
    }

    public void Set(int _index, Action<Transform> _onClickHandle, LDCameraPointData _cameraPointData)
    {
        Set(_index, _onClickHandle);

        TrPointV1.position = _cameraPointData.PointV1;
        TrPointV2.position = _cameraPointData.PointV2;
        TrPointV3.position = _cameraPointData.PointV3;
        TrPointV4.position = _cameraPointData.PointV4;

        Refresh();
    }

    public void Refresh()
    {
        CameraPathPoint.PointV1 = TrPointV1.position;
        CameraPathPoint.PointV2 = TrPointV2.position;
        CameraPathPoint.PointV3 = TrPointV3.position;
        CameraPathPoint.PointV4 = TrPointV4.position;

        var distV1V2 = (GetVector2Point(TrPointV1) - GetVector2Point(TrPointV2)).magnitude;
        var distV2V3 = (GetVector2Point(TrPointV2) - GetVector2Point(TrPointV3)).magnitude;
        var distV3V4 = (GetVector2Point(TrPointV3) - GetVector2Point(TrPointV4)).magnitude;

        var totalDist = distV1V2 + distV2V3 + distV3V4;

        CameraPathPoint.RatioV1V2 = distV1V2 / totalDist;
        CameraPathPoint.RatioV2V3 = distV2V3 / totalDist;
        CameraPathPoint.RatioV3V4 = distV3V4 / totalDist;
    }

    /// <summary>
    /// 각 포인트의 트랜스폼으로 Vector2를 계산하여 반환한다.
    /// 카메라의 이동을 구현할 때 Y축을 제외한 X와 Z만을 사용하기 때문이다.
    /// </summary>
    private Vector2 GetVector2Point(Transform _transform)
    {
        return new Vector2(_transform.position.x, _transform.position.z);
    }

    public Vector3 GetBezier(float _ratio)
    {
        return CameraPathPoint.GetBezier(_ratio);
    }
}
