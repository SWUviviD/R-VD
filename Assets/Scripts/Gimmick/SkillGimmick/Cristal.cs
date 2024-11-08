using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cristal : MonoBehaviour 
{
    public enum CristalType
    {
        Basic,
        Blink,
        Move
    }

    [SerializeField] protected CristalData data;

    /// <summary> 크리스탈 위치 </summary>
    [SerializeField] private Transform cristalTrs;

    private const string CristalOriginalPoint = "OriginalPoint";
    private Vector3 CristalOriginalPosition => data.DictPoint[CristalOriginalPoint].position;

    /// <summary> 크리스탈 타입 마다의 업데이트 </summary>
    public delegate void CristalUpdate();
    public CristalUpdate cristalUpdate;

    // 깜빡이는 크리스탈
    /// <summary> 깜빡거리는 누적 시간 </summary>
    protected float blinkCristal_ElapsedTime = 0f; 
    /// <summary> 깜빡거리는지 </summary>
    protected bool blinkCristal_IsHide = false;

    // 움직이는 크리스탈
    protected const string CristalMoveStartPoint = "MoveStartPoint";
    protected const string CristalMoveEndPoint = "MoveEndPoint";

    protected float moveCristal_ElapsedTime = 0f;
    protected Vector3 moveCristal_startPoint;
    protected Vector3 moveCristal_endPoint;
    protected Rigidbody moveCristal_rigidBody;

    public virtual void Init()
    {
        moveCristal_rigidBody = cristalTrs.GetComponent<Rigidbody>();
    }

    public virtual void SetGimmick()
    {
        switch((CristalType)data.CristalType)
        {
            default:
            case CristalType.Basic:
                {
                    cristalTrs.position = CristalOriginalPosition;
                    cristalUpdate = BasicCristalUpdate;
                    break;
                }
            case CristalType.Blink:
                {
                    cristalTrs.position = CristalOriginalPosition;
                    cristalUpdate = BlinkCristalUpdate;
                    break;
                }
            case CristalType.Move:
                {
                    cristalUpdate = MoveCristalUpdate;
                    break;
                }
        }

        cristalTrs.gameObject.SetActive(true);

        blinkCristal_ElapsedTime = 0f;
        blinkCristal_IsHide = false;

        moveCristal_ElapsedTime = 0f;
        moveCristal_startPoint = data.DictPoint[CristalMoveStartPoint].position;
        moveCristal_endPoint = data.DictPoint[CristalMoveEndPoint].position;
    }

    protected virtual void BasicCristalUpdate() { }



    protected virtual void BlinkCristalUpdate()
    {
        blinkCristal_ElapsedTime += Time.deltaTime;
        if(blinkCristal_IsHide == false) // 보여지고 있을때
        {
            if(blinkCristal_ElapsedTime >= data.BlinkShowTime)
            {
                blinkCristal_ElapsedTime = 0f;
                HideCristal();
            }
        }
        else
        {
            if (blinkCristal_ElapsedTime >= data.BlinkHideTime)
            {
                blinkCristal_ElapsedTime = 0f;
                ShowCristal();
            }
        }
    }

    protected virtual void HideCristal()
    {
        cristalTrs.gameObject.SetActive(false);
        blinkCristal_IsHide = true;
    }
    protected virtual void ShowCristal()
    {
        cristalTrs.gameObject.SetActive(true);
        blinkCristal_IsHide = false;
    }



    protected virtual void MoveCristalUpdate()
    {
        moveCristal_ElapsedTime += Time.deltaTime;
        if(moveCristal_ElapsedTime >= data.MoveMoveTime)
        {
            moveCristal_ElapsedTime -= data.MoveMoveTime;

            Vector3 temp = moveCristal_endPoint;
            moveCristal_endPoint = moveCristal_startPoint;
            moveCristal_startPoint = temp;
        }

        Vector3 newPosition = Vector3.Lerp(moveCristal_startPoint, moveCristal_endPoint, 
            moveCristal_ElapsedTime / data.MoveMoveTime);

        moveCristal_rigidBody.MovePosition(newPosition);
    }

    public void OnCristalBreak()
    {
        cristalTrs.gameObject.SetActive(false);
    }
}
