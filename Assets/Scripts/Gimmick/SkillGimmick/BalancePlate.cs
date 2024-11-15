using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BalancePlate : GimmickBase<BalancePlateData>, IFloorInteractive
{
    [SerializeField] private GameObject plate;
    private Rigidbody plateRigid;

    private float level1Dist;
    private float level2Dist;
    private float level3Dist;

    private Transform player;
    private PlayerMove playerMove;
    private bool isPlayerOn;

    private const string CenterStr = "Center";
    private Transform centerTransform => gimmickData.DictPoint[CenterStr];


    protected override void Init()
    {
        plateRigid = plate.GetComponent<Rigidbody>();
    }

    [ContextMenu("SetMenu")]
    public override void SetGimmick()
    {
        plate.transform.rotation = Quaternion.Euler(Vector3.zero);

        plate.transform.localScale = Vector3.one * gimmickData.Radius * 2;
        level1Dist = gimmickData.Radius / 3;
        level2Dist = level1Dist * 2;
        level3Dist = level1Dist * 3;
    }

    private float elapsedTime = 0f;
    private Vector3 newRotation = Vector3.zero;
    private Vector3 currentRotation = Vector3.zero;
    private Vector3 rotateDirection = Vector3.zero;
    private void Update()
    {
        if(isPlayerOn)
        {
            elapsedTime = 0;

            float rotateLevel = 0f;
            float dist = (centerTransform.position - player.transform.position).magnitude;
            if (dist < level1Dist) { rotateLevel = gimmickData.Level1_Roate; }
            else if (dist < level2Dist) { rotateLevel = gimmickData.Level2_Roate; }
            else if (dist < level3Dist) { rotateLevel = gimmickData.Level3_Roate; }

            rotateDirection = Vector3.Cross(centerTransform.up, playerMove.CurrentFeetPosition - centerTransform.position).normalized;
            plate.transform.Rotate(rotateDirection * dist * Time.deltaTime);
        }
        else
        {
            if (plate.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                return;
            }

            plateRigid.angularVelocity = Vector3.zero;

            elapsedTime += Time.deltaTime;
            currentRotation = plate.transform.rotation.eulerAngles;
            currentRotation.Set(
                currentRotation.x > 180f ? 360f - currentRotation.x : currentRotation.x,
                currentRotation.y > 180f ? 360f - currentRotation.y : currentRotation.y,
                currentRotation.z > 180f ? 360f - currentRotation.z : currentRotation.z);
            newRotation = Vector3.Lerp(currentRotation, Vector3.zero, elapsedTime / gimmickData.ReturnToNormalTime);
            plate.transform.rotation = Quaternion.Euler(newRotation);
        }
    }

    public void InteractStart(GameObject _player)
    {
        player = _player.transform;
        playerMove = player.GetComponent<PlayerMove>();
        isPlayerOn = true;
    }

    public void InteractEnd(GameObject _player)
    {
        isPlayerOn = false;
    }
}
