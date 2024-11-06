using System.Collections;
using UnityEngine;

public class RolerGimmick : GimmickBase<RolerGimmickData>
{
    private float rotationSpeed;
    private Rigidbody rb;


    protected override void Init()
    {
        // 추가 작업 없음
    }


    public override void SetGimmick()
    {
        rotationSpeed = 360f / gimmickData.rotationDuration;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(RotateObject());
    }


    private IEnumerator RotateObject()
    {
        float direction = gimmickData.rotateClockwise ? 1f : -1f;

        while (true)
        {
            rb.angularVelocity = new Vector3(0, 0, direction * rotationSpeed * Mathf.Deg2Rad);

            yield return null;
        }
    }
}
