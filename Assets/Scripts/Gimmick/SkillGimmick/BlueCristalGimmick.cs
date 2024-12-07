using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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

    private WaitForSeconds waitForSphereToRoll;

    private const string GimmickStartPoint = "GimmickStartPoint";
    private const string GimmickEndPoint = "GimmickEndPoint";

    private bool isCristalBroke = false;
    private bool isReturnToStartPoint = false;

    private Vector3 endPoint = Vector3.zero;
    private Vector3 startPoint = Vector3.zero;

    private Vector3 endRotPoint = new Vector3(360f, 0f, 0f);
    private Vector3 startRotPoint = Vector3.zero;

    protected override void Init()
    {
        cristal.Init();

        railList.Add(new List<Transform>());
        railList.Add(new List<Transform>());

        sense.OnCristalBreak.RemoveListener(OnCristalBreak);
        sense.OnCristalBreak.AddListener(OnCristalBreak);

        sphereSense.OnPlayerOn.AddListener(() => StartCoroutine(CoStartRolling()));
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
        sphereRigid.transform.LookAt(endPoint);

        bool isLeft = false;
        for(int i =0;i<2;++i)
        {
            SetRail(ref Rail[i], isLeft);
            isLeft = true;
        }
    }

    Vector3 railPosition = Vector3.zero;
    private void SetRail(ref Transform _rail, bool _isLeft)
    {
        _rail.localPosition = startPoint;
        _rail.LookAt(endPoint);
        _rail.localPosition += new Vector3(gimmickData.SphereSize / 2 * (_isLeft ? -1 : 1), 0f, -GimmickData.SphereSize * 0.5f);

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
        float endRotTime = 360 / gimmickData.SphereRotateSpeed;

        while(true)
        {
            elapsedRotTime += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, elapsedTime / endTime);
            Vector3 newRotation = Vector3.Slerp(startRotPoint, endRotPoint, elapsedRotTime / endRotTime);
            if(elapsedRotTime > endRotTime)
            {
                elapsedRotTime -= endRotTime;
            }

            sphereRigid.MovePosition(newPosition);
            sphereRigid.MoveRotation(Quaternion.Euler(newRotation));

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

        temp = endRotPoint;
        endRotPoint = startRotPoint;
        startRotPoint = temp;

        sphereRigid.velocity = Vector3.zero;
        sphereRigid.angularVelocity = Vector3.zero;

        StopAllCoroutines();

        StartCoroutine(CoStopSphere());
    }
}
