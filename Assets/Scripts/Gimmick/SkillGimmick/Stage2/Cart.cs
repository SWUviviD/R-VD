using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Cart : MonoBehaviour
{
    [SerializeField] private Transform cartTrans;
    [SerializeField] private CartRail[] Rails;

    [SerializeField] private bool loop = false;
    [SerializeField] private int startIndex = 0;
    [SerializeField] private bool startMovingForward = true;
    [SerializeField] private float movingSpeed = 10f;
    private int nextIndex = 1;
    private bool isMovingForward = true;

    private bool isMoving = false;

    private void Awake()
    {
        isMovingForward = startMovingForward;
        nextIndex = GetNextIndex(startIndex);

        cartTrans.position = Rails[startIndex].CenterPos.position;
        isMoving = false;

        dir = (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized;
    }

    public void StartMoving()
    {
        isMoving = true;
        dir = (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    private Vector3 dir = Vector3.zero;
    private void FixedUpdate()
    {
        if (isMoving == false)
            return;

        if (Vector3.Dot(dir, (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized) < 0)
        {
            cartTrans.position = Rails[nextIndex].CenterPos.position;
            
            nextIndex = GetNextIndex(nextIndex);
            dir = (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized;
        }

        cartTrans.position +=
            dir *
            Time.fixedDeltaTime * movingSpeed;
    }

    private int GetNextIndex(int preIndex)
    {
        int newIndex = isMovingForward ? preIndex + 1 : preIndex - 1;
        if (loop)
        {
            newIndex = (preIndex + 1) % Rails.Length;
        }
        else if (newIndex < 0 || newIndex >= Rails.Length)
        {
            isMovingForward = !isMovingForward;
            newIndex = isMovingForward ? preIndex + 1 : preIndex - 1;
        }
        return newIndex;
    }
}
