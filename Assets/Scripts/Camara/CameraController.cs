/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Timers;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// 경로를 따라 캐릭터를 따라가는 카메라 컨트롤러.
/// 플레이어는 특정 범위를 갖는다. 그 범위안에 카메라 경로가 있다면, 카메라가 플레이어를 따라간다.
/// 경로는 직선이기 때문에 원이 닿으면 교점이 두 개가 생긴다. 두 교점중에 더 멀리(플레이어의 뒤) 있는 교점이 카메라의 위치가 된다.
/// 경로에 원이 닿았는지 검사는 2D 형태로 이루어진다. 이는 위에서 아래를 내려다 봤을때 기준으로 이루어져 있기 때문에, X축과 Z축을 기준으로 검사한다. 
/// </summary>
public class CameraController : MonoSingleton<CameraController>
{
    [SerializeField] private Camera mainCamera;
    /// <summary>
    /// 플레이어의 트랜스폼
    /// </summary>
    private Transform trPlayerPoint;
    /// <summary>
    /// 플레이어의 위치에서 카메라 경로를 감지하는 범위
    /// </summary>
    [Header("플레이어의 위치에서 카메라 경로를 감지하는 범위")]
    [SerializeField] private float cameraDetectRange;

    /// <summary>
    /// 플레이어와 카메라간의 거리
    /// </summary>
    [Header("플레이어와 카메라간의 거리")]
    [SerializeField] private float cameraDistance;

    /// <summary>
    /// 카메라 초당 이동속도
    /// </summary>
    [Header("카메라 초당 이동속도")]
    [SerializeField] private float cameraMoveSpeedPerSec;
    
    /// <summary>
    /// 카메라 높이 오프셋
    /// </summary>
    [Header("카메라 높이 오프셋")]
    [SerializeField] private float cameraHeightOffset;
    
    /// <summary>
    /// 카메라 경로 리스트
    /// </summary>
    public List<CameraPathPoint> CameraPointList { get; private set; }

    public Camera MainCamera => mainCamera;

    private bool isInit = false;

    /// <summary> 카메라가 이동하고자 하는 위치 </summary>
    private Vector3 desiredCameraPosition;

    /// <summary>
    /// 실제 카메라의 포지션
    /// </summary>
    private Vector3 cameraPosition;

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
        if (CameraPointList.IsNullOrEmpty()) return;

        cameraPosition = GetCameraPosition(trPlayerPoint.position);
        isInit = true;
    }
    
    private void LateUpdate()
    {
        if (isInit == false) return;

        // 카메라의 위치를 계속 갱신한다.
        desiredCameraPosition = GetCameraPosition(trPlayerPoint.position);
        float moveSpeed = cameraMoveSpeedPerSec * Time.deltaTime;
        Vector3 toDesired = desiredCameraPosition - cameraPosition;
        if (toDesired.magnitude > moveSpeed)
        {
            cameraPosition += toDesired.normalized * moveSpeed;
        }
        else
        {
            cameraPosition = desiredCameraPosition;
        }

        mainCamera.transform.position = cameraPosition + -mainCamera.transform.forward * cameraDistance + Vector3.down * cameraHeightOffset;
        mainCamera.transform.forward = (trPlayerPoint.position - mainCamera.transform.position).normalized;
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
    }
    
    /// <summary>
    /// 플레이어의 위치와 거리에 따른 카메라의 위치를 계산하는 함수. 
    /// </summary>
    public Vector3 GetCameraPosition(Vector3 _playerPosition)
    {
        Vector2 playerPoint = GetCameraPoint(_playerPosition);

        Vector2 closestPoint;
        
        // 경로의 시작인덱스부터 경로가 플레이어의 범위 안에 들어왔는지 검사한다. 
        for (int i = 0, count = CameraPointList.Count; i < count; ++i)
        {
            var point = CameraPointList[i];

            Vector2 startPoint = GetCameraPoint(point.PointV1);
            Vector2 endPoint = GetCameraPoint(point.PointV4);
            
            if (IsCircleLineCollided(startPoint, endPoint, playerPoint, cameraDetectRange, out closestPoint))
            {
                float ratio = (startPoint - closestPoint).magnitude / (startPoint - endPoint).magnitude;
                if (ratio < 1.0f - float.Epsilon)
                {
                    Vector3 toBack = (point.PointV1 - point.PointV4).normalized * cameraDistance;
                    return point.GetBezier(ratio) + toBack;
                }
            }

            if (i < count - 1)
            {
                var nextPoint = CameraPointList[i + 1];
                Vector2 nextStartPoint = GetCameraPoint(nextPoint.PointV1);
                if (IsCircleLineCollided(endPoint, nextStartPoint, playerPoint, cameraDetectRange, out closestPoint))
                {
                    if (closestPoint == nextStartPoint) continue;
                    float ratio = (endPoint - closestPoint).magnitude / (endPoint - nextStartPoint).magnitude;
                    if (ratio >= 1.0f - float.Epsilon) continue;
                    Vector3 toBack = (point.PointV4 - nextPoint.PointV1).normalized * cameraDistance;
                    return point.PointV4 + (nextPoint.PointV1 - point.PointV4) * ratio + toBack;
                }
            }
        }

        return cameraPosition;
    }
    
    /// <summary>
    /// 원이 직선에 닿았는지 여부를 판단하고, 직선에서 가장 가까운 지점을 반환합니다.
    /// </summary>
    public static bool IsCircleLineCollided(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 circlePosition, float radius, out Vector2 closestPoint)
    {
        Vector2 line = lineEndPosition - lineStartPosition;
        float lineLengthSquared = line.sqrMagnitude;

        // 직선의 길이가 0인 경우, 시작 지점만을 고려
        if (lineLengthSquared == 0f)
        {
            closestPoint = lineStartPosition;
            return Vector2.Distance(circlePosition, lineStartPosition) <= radius;
        }

        // 점 위치를 직선에 투영한 t 값 계산
        float t = Vector2.Dot(circlePosition - lineStartPosition, line) / lineLengthSquared;

        // t를 [0,1] 범위로 클램프하여 직선 구간 내의 점으로 제한
        t = Mathf.Clamp01(t);

        // 가장 가까운 지점 계산
        closestPoint = lineStartPosition + t * line;

        // 점과 가장 가까운 지점 간의 거리 계산
        float distance = Vector2.Distance(circlePosition, closestPoint);

        // 거리가 반지름 이하인지 확인
        return distance <= radius;
    }
    
    /// <summary>
    /// 카메라 포인트의 위치를 Vector2로 구한다. Y축을 제외한 X, Z축이다.
    /// </summary>
    private Vector2 GetCameraPoint(Vector3 _point)
    {
        return new Vector2(_point.x, _point.z);
    }
}
*/

//using Cinemachine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CameraController : MonoSingleton<CameraController>
//{
//    [Header("Cinemachine Camera")]
//    [SerializeField] private Transform cinemachineBrainTR;    // Cinemachine brain
//    [SerializeField] private Transform playerFollowCameraTR;  // Player follow camera
//    [SerializeField] private Transform dialogueCameraTR;      // Dialogue camera

//    [Header("Camera Settings")]
//    [SerializeField] private Transform target;          // Target (Player)
//    [SerializeField] private float minDistance = 3;     // Camera & Target distatnce (min)
//    [SerializeField] private float maxDistance = 30;    // Camera & Target distatnce (max)
//    [SerializeField] private float wheelSpeed = 500;    // Mouse wheel speed
//    [SerializeField] private float xMoveSpeed = 500;    // Camera Y speed
//    [SerializeField] private float yMoveSpeed = 250;    // Camera X speed

//    private float yMinLimit = 5;    // Camera X rotate limit (min)
//    private float yMaxLimit = 80;   // Camera X rotate limit (max)
//    private float x, y;             // Mouse movement value
//    private float distance;         // Camera & Target distance

//    public bool IsActive {  get; private set; }

//    private void Awake()
//    {
//        // Init distance
//        //target = GameManager.Instance.Player.transform;
//        IsActive = true;
//        distance = Vector3.Distance(transform.position, target.position);

//        // Init angles
//        Vector3 angles = transform.eulerAngles;
//        x = angles.y;
//        y = angles.x;
//    }

//    private void Update()
//    {
//        if (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver || GameManager.Instance.IsStageClear) return;
//        if (IsActive == false) return;

//        //Cursor.visible = false; // Invisible mouse cursor
//        //Cursor.lockState = CursorLockMode.Locked; // Locked mouse cursor

//        // Mouse X & Mouse Y
//        x += Input.GetAxis("Mouse X") * xMoveSpeed * Time.deltaTime;
//        y -= Input.GetAxis("Mouse Y") * yMoveSpeed * Time.deltaTime;

//        // Clamp Angle Y
//        y = ClampAngle(y, yMinLimit, yMaxLimit);

//        // Camera Rotation
//        transform.rotation = Quaternion.Euler(y, x, 0);

//        // Mouse wheel = distance
//        distance -= Input.GetAxis("Mouse ScrollWheel") * wheelSpeed * Time.deltaTime;

//        // Clamp distance
//        distance = Mathf.Clamp(distance, minDistance, maxDistance);
//    }

//    private void LateUpdate()
//    {
//        if (target == null) return;
//        if (IsActive == false) return;

//        // Camera Position (+ distance)
//        transform.position = transform.rotation * new Vector3(0, 0, -distance) + target.position;
//    }

//    private float ClampAngle(float angle, float min, float max)
//    {
//        if (angle < -360) angle += 360;
//        if (angle > 360) angle -= 360;

//        return Mathf.Clamp(angle, min, max);
//    }

//    public void SetMainCameraActive(bool active)
//    {
//        if (active)
//        {
//            StartCoroutine(IActive(1f));
//        }
//        else
//        {
//            IsActive = active;
//            playerFollowCameraTR.SetPositionAndRotation(transform.position, transform.rotation);
//            transform.SetPositionAndRotation(cinemachineBrainTR.position, cinemachineBrainTR.rotation);
//        }
//    }

//    private IEnumerator IActive(float time)
//    {
//        yield return new WaitForSeconds(time);
//        IsActive = true;
//        yield return null;
//    }

//    public void Respawn(Vector3 _position, Vector3 _rotation)
//    {
//        transform.rotation = Quaternion.Euler(new Vector3(30f, _rotation.y, 0f));
//        transform.position = transform.rotation * new Vector3(0, 0, -distance) + _position;

//        x = transform.eulerAngles.y;
//        y = transform.eulerAngles.x;
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraPosition
{
}

public class CameraController : MonoSingleton<CameraController>
{
    public enum CameraMode
    {
        Fixed,
        Cinematography,
        Orbit,
    }

    [field: SerializeField] public Camera MainCamera { get; private set; }

    [Header("Scripts")]
    [SerializeField] private OrbitCamera _orbitCam;
    [SerializeField] private CameraEffector _effectCam;

    private CameraMode _mode = CameraMode.Orbit;

    public void OnLoadCameraSetting(Transform cam)
    {
        MainCamera = cam.GetComponent<Camera>();

        _orbitCam = cam.GetComponent<OrbitCamera>();
        _effectCam = cam.GetComponent<CameraEffector>();
    }

    public void SetCameraMode(CameraMode mode)
    {
        _mode = mode;

        switch (mode)
        {
            case CameraMode.Fixed: break;
            case CameraMode.Orbit: break;
        }
    }

    public void SetCameraPositionAndRotation(Vector3 rotation, Vector3 position)
    {
        switch (_mode)
        {
            case CameraMode.Fixed:
            case CameraMode.Cinematography:
                {
                    transform.position = position;
                    transform.rotation = Quaternion.Euler(rotation);
                    break;
                }

            case CameraMode.Orbit:
                {
                    _orbitCam.SetCameraRotation(rotation.x);
                    break;
                }

            default: break;
        }
    }
}