using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class StarHunt : SkillBase
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform projectTrs;
    [SerializeField] private PlayerMove playerMove;

    [SerializeField] private float minShotRange = 20f;
    [SerializeField] private float maxShotRange = 105;
    [SerializeField] private float maxKeyPressedTime = 0.8f;

    private float keyPressedTime = 0.0f;
    private bool isKeyPressing = false;

    public UnityEvent OnStarHuntKeyDown { get; private set; } = new UnityEvent();
    public UnityEvent OnStarHuntKeyUp { get; private set; } = new UnityEvent();

    private void Start()
    {
        try
        {
            var arrow = AddressableAssetsManager.Instance.SyncLoadObject(
                "Data/Prefabs/Arrow", 
                Defines.PoolDefines.PoolType.StarHunts.ToString()) as GameObject;
            Poolable arrowPrefab = Instantiate(arrow).GetComponent<Poolable>();
            PoolManager.Instance.CreatePool(Defines.PoolDefines.PoolType.StarHunts, arrowPrefab, 3);
            Destroy(arrowPrefab.gameObject);
        }
        catch(System.Exception e)
        {
            LogManager.LogError($"{e.Message}");
        }
    }

    private void Update()
    {
        if (isKeyPressing == false)
            return;

        keyPressedTime += Time.deltaTime;
        if (keyPressedTime > maxKeyPressedTime)
            keyPressedTime = maxKeyPressedTime;
    }

    private void FixedUpdate()
    {
        if (isKeyPressing == false)
            return;

        var q = GetCameraFloatLook();
        if(q.HasValue)
        {
            rb.MoveRotation(q.Value);
        }
    }

    public override void OnSkill(InputAction.CallbackContext _playerStatus) { }

    public override void OnSkillStart(InputAction.CallbackContext _playerStatus)
    {
        isKeyPressing = true;
        keyPressedTime = 0f;
        OnStarHuntKeyDown?.Invoke();

        var q = GetCameraFloatLook();
        if(q.HasValue) rb.MoveRotation(q.Value);
    }

    public override void OnSkillStop(InputAction.CallbackContext _playerStatus)
    {
        OnStarHuntKeyUp?.Invoke();
        isKeyPressing = false;

        float t01 = Mathf.Clamp01(keyPressedTime/maxKeyPressedTime);
        float resultRange = Mathf.Lerp(minShotRange, maxShotRange, t01);

        var arrow = PoolManager.Instance.GetPoolObject(Defines.PoolDefines.PoolType.StarHunts);
        if (arrow.TryGetComponent<StarHuntArrow>(out var arrowScript))
        {
            arrowScript.Project(projectTrs, resultRange);
        }
    }

    private Quaternion? GetCameraFloatLook()
    {
        Camera cam = CameraController.Instance.MainCamera;
        if (cam == null)
            return null;

        Vector3 f = cam.transform.forward;
        f.y = 0f;
        if (f.sqrMagnitude < 1e-6f)
            return null;

        f.Normalize();
        return Quaternion.LookRotation(f, Vector3.up);
    }
}
