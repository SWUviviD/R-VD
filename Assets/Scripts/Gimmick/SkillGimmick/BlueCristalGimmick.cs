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
    [SerializeField] private Rigidbody sphereRigid;

    private WaitForSeconds waitForSphereToRoll;

    private const string GimmickStartPoint = "GimmickStartPoint";
    private const string GimmickEndPoint = "GimmickEndPoint";

    private bool isCristalBroke = false;
    private bool isReturnToStartPoint = false;

    private Vector3 endPoint = Vector3.zero;
    private Vector3 startPoint = Vector3.zero;

    protected override void Init()
    {
        cristal.Init();

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
        foreach(var r in Rail)
        {
            r.transform.localPosition = startPoint;
            r.transform.LookAt(endPoint);
            r.transform.localScale =
                new Vector3(r.transform.localScale.x, r.transform.localScale.y,
                (endPoint - startPoint).magnitude * 0.5f + GimmickData.SphereSize);
            r.transform.localPosition += new Vector3(gimmickData.SphereSize / 2 * (isLeft ? -1 : 1), 0f, -GimmickData.SphereSize * 0.5f);
            isLeft = true;
        }
    }

    private void Update()
    {
        if (isCristalBroke == false)
        {
            cristal.cristalUpdate?.Invoke();
        }
    }

    public void OnCristalBreak()
    {
        isCristalBroke = true;
        cristal.OnCristalBreak();
        ActivateGimmick();
    }

    public void ActivateGimmick()
    {
        gimmick.gameObject.SetActive(true);
    }

    private IEnumerator CoStopSphere()
    {
        yield return waitForSphereToRoll;
        StartCoroutine(CoStartRolling());
    }

    private IEnumerator CoStartRolling()
    {
        float elapsedTime = 0f;
        float endTime = gimmickData.SphereMoveTime;

        sphereRigid.angularVelocity = transform.right * gimmickData.SphereRotateSpeed;

        while(true)
        {
            elapsedTime += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, elapsedTime / endTime);
            sphereRigid.MovePosition(newPosition);

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

        sphereRigid.velocity = Vector3.zero;
        sphereRigid.angularVelocity = Vector3.zero;

        StopAllCoroutines();

        StartCoroutine(CoStopSphere());
    }
}
