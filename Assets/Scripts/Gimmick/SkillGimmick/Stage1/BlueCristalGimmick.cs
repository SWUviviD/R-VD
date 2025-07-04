using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCristalGimmick : GimmickBase<BlueCristalGimmickData>
{
    [SerializeField] private Cristal cristal;
    [SerializeField] private CristalSense sense;
    [SerializeField] private BlueCristalSphereSense sphereSense;

    [SerializeField] private Transform gimmick;
    [SerializeField] private Transform[] Rail;
    [SerializeField] private GameObject railPrefab;
    private List<List<Transform>> railList = new List<List<Transform>>();
    [SerializeField] private Rigidbody sphereRigid;
    [SerializeField] private AudioSource audioSource;

    private WaitForSeconds waitForSphereToRoll;

    private const string GimmickStartPoint = "GimmickStartPoint";
    private const string GimmickEndPoint = "GimmickEndPoint";

    private bool isCristalBroke = false;
    private bool isReturnToStartPoint = false;

    private Vector3 endPoint = Vector3.zero;
    private Vector3 startPoint = Vector3.zero;

    private Vector3 rotatingAxis;
    private Quaternion endRotPoint;
    private Quaternion startRotPoint;

    protected override void Init()
    {
        cristal.Init();

        railList.Add(new List<Transform>());
        railList.Add(new List<Transform>());

        sense.OnCristalBreak.RemoveListener(OnCristalBreak);
        sense.OnCristalBreak.AddListener(OnCristalBreak);

        sphereSense.OnPlayerOn.AddListener(() => StartCoroutine(CoStartRolling()));

        SetGimmick();
    }

    private void Start()
    {
        // 세팅 문제로 이동
        bool isLeft = false;
        for (int i = 0; i < 2; ++i)
        {
            SetRail(ref Rail[i], isLeft);
            isLeft = true;
        }
        railPrefab.SetActive(false);
    }

    [ContextMenu("SetGimmick")]
    public override void SetGimmick()
    {
        cristal.SetGimmick();

        isCristalBroke = false;
        isReturnToStartPoint = false;

        gimmick.gameObject.SetActive(false);

        
        waitForSphereToRoll = new WaitForSeconds(gimmickData.SphereStopTime);

        Vector3 startPoint = gimmickData.DictPoint[GimmickStartPoint].position;
        Vector3 endPoint = gimmickData.DictPoint[GimmickEndPoint].position;
        this.endPoint = endPoint;
        this.startPoint = startPoint;
        Vector3 offset = endPoint - startPoint;
        Quaternion rotation = Quaternion.LookRotation(startPoint, endPoint);

        gimmick.LookAt(endPoint);

        sphereRigid.transform.localScale = 
            Vector3.one * gimmickData.SphereSize;
        sphereRigid.transform.position = startPoint;
        //sphereRigid.transform.LookAt(endPoint);
        sphereRigid.rotation = rotation;

        rotatingAxis = Vector3.Cross(Vector3.up, (endPoint - startPoint).normalized);
    }

    Vector3 railPosition = Vector3.zero;
    private void SetRail(ref Transform _rail, bool _isLeft)
    {
        _rail.position = startPoint;
        _rail.LookAt(endPoint);
        _rail.localPosition += new Vector3(gimmickData.SphereSize / 2 * (_isLeft ? -1 : 1), 0f, -GimmickData.SphereSize);

        int railLength = Mathf.CeilToInt((endPoint - startPoint).magnitude * 0.5f + GimmickData.SphereSize);
        int radiListIndex = _isLeft ? 0 : 1;
        for (int i = 0; i < railLength; ++i)
        {
            railPosition = new Vector3(0f, 0f, 0.9f + 1.95f * i - 0.5f);
            if ( i < railList[radiListIndex].Count)
            {
                railList[radiListIndex][i].localPosition = railPosition;
            }
            else
            {
                Transform newRail = Instantiate(railPrefab, _rail).transform;
                newRail.localPosition = railPosition;
                railList[radiListIndex].Add(newRail);
            }
        }
    }

    private void Update()
    {
        if (isCristalBroke == false)
        {
            cristal.cristalUpdate?.Invoke();
        }
    }

    [ContextMenu("CristalBreak")]
    public void OnCristalBreak()
    {
        isCristalBroke = true;
        audioSource.Play();
        cristal.OnCristalBreak();
        ActivateGimmick();
    }

    public void ActivateGimmick()
    {
        gimmick.gameObject.SetActive(true);
        sphereRigid.gameObject.SetActive(true);
    }

    private IEnumerator CoStopSphere()
    {
        yield return waitForSphereToRoll;
        StartCoroutine(CoStartRolling());
    }

    private IEnumerator CoStartRolling()
    {
        float elapsedTime = 0f;
        float elapsedRotTime = 0f;
        float endTime = gimmickData.SphereMoveTime;
        Quaternion currentRot = sphereRigid.rotation;

        while (true)
        {
            elapsedRotTime += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, elapsedTime / endTime);

            Quaternion deltaRot = Quaternion.AngleAxis(gimmickData.SphereRotateSpeed * Time.deltaTime, rotatingAxis);
            currentRot = deltaRot * currentRot;

            sphereRigid.MovePosition(newPosition);
            sphereRigid.MoveRotation(currentRot);

            if (elapsedTime >= endTime)
            {
                break;
            }

            yield return null;
        }

        isReturnToStartPoint = !isReturnToStartPoint;
        sphereRigid.MovePosition(endPoint);

        Vector3 temp = endPoint;
        endPoint = startPoint;
        startPoint = temp;

        rotatingAxis = rotatingAxis * -1f;

        sphereRigid.velocity = Vector3.zero;
        sphereRigid.angularVelocity = Vector3.zero;

        StopAllCoroutines();

        StartCoroutine(CoStopSphere());
    }

    //private IEnumerator CoStartRolling()
    //{
    //    float duration = gimmickData.SphereMoveTime;
    //    float elapsed = 0f;

    //    Vector3 dir = (endPoint - startPoint).normalized;
    //    float totalDistance = Vector3.Distance(startPoint, endPoint);
    //    float radius = sphereRigid.transform.localScale.x * 0.5f;

    //    Vector3 prevPos = startPoint;
    //    Quaternion currentRot = sphereRigid.rotation;
    //    Vector3 currentPos = startPoint;

    //    float maxRotateSpeed = gimmickData.SphereRotateSpeed; // deg/sec

    //    while (elapsed < duration)
    //    {
    //        float deltaTime = Time.deltaTime;
    //        elapsed += deltaTime;

    //        float t = Mathf.Clamp01(elapsed / duration);

    //        // 이동 위치 계산
    //        Vector3 nextPos = Vector3.Lerp(startPoint, endPoint, t);
    //        Vector3 moveDelta = nextPos - prevPos;

    //        float distanceMoved = moveDelta.magnitude;

    //        if (distanceMoved > 0.0001f)
    //        {
    //            Vector3 rotationAxis = Vector3.Cross(Vector3.up, moveDelta.normalized); // ✅ 방향 반전함
    //            float idealAngleDeg = (distanceMoved / radius) * Mathf.Rad2Deg;

    //            // ✅ 회전 속도 제한 적용
    //            float maxAngleDeg = maxRotateSpeed * deltaTime;
    //            float clampedAngleDeg = Mathf.Min(idealAngleDeg, maxAngleDeg);

    //            Quaternion deltaRot = Quaternion.AngleAxis(clampedAngleDeg, rotationAxis);
    //            currentRot = deltaRot * currentRot;
    //        }

    //        sphereRigid.MovePosition(nextPos);
    //        sphereRigid.MoveRotation(currentRot);

    //        prevPos = nextPos;

    //        yield return null;
    //    }

    //    // 보정
    //    sphereRigid.MovePosition(endPoint);
    //    sphereRigid.velocity = Vector3.zero;
    //    sphereRigid.angularVelocity = Vector3.zero;

    //    // 반전
    //    isReturnToStartPoint = !isReturnToStartPoint;

    //    Vector3 temp = startPoint;
    //    startPoint = endPoint;
    //    endPoint = temp;

    //    StartCoroutine(CoStopSphere());
    //}
}
