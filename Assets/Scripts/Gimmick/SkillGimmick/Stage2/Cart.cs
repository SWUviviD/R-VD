using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Cart : MonoBehaviour, IFloorInteractive
{
    [SerializeField] private Transform cartTrans;
    [SerializeField] private CartRail[] Rails;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip movingSound;

    [SerializeField] private bool loop = false;
    [SerializeField] private int startIndex = 0;
    [SerializeField] private bool startMovingForward = true;
    [SerializeField] private float movingSpeed = 10f;
    private int nextIndex = 1;
    private bool isMovingForward = true;

    private bool isMoving = false;

    private void Awake()
    {
        audioSource.clip = movingSound;
        audioSource.loop = true;

        isMovingForward = startMovingForward;
        nextIndex = GetNextIndex(startIndex);

        cartTrans.position = Rails[startIndex].CenterPos.position;
        isMoving = false;

        dir = (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized;
    }

    public void StartMoving()
    {
        audioSource.Play();

        isMoving = true;
        dir = (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized;
    }

    public void StopMoving()
    {
        isMoving = false;
        audioSource.Stop();
    }

    private Vector3 dir = Vector3.zero;
    private void FixedUpdate()
    {
        if (isMoving == false)
            return;

        Vector3 before = cartTrans.position;

        if (Vector3.Dot(dir, (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized) < 0)
        {
            cartTrans.position = Rails[nextIndex].CenterPos.position;
            
            nextIndex = GetNextIndex(nextIndex);
            dir = (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized;
        }

        cartTrans.position +=
            dir *
            Time.fixedDeltaTime * movingSpeed;

        Vector3 delta = cartTrans.position - before;
        if (ridingPlayer != null)
            ridingPlayer.position += delta;
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

    private Transform ridingPlayer;

    public void InteractStart(GameObject player)
    {
        if(player.TryGetComponent<PlayerMove>(out var playerMove))
        {
            ridingPlayer = playerMove.transform;
        }
    }

    public void InteractEnd(GameObject player)
    {
        if (player.TryGetComponent<PlayerMove>(out var playerMove))
        {
            ridingPlayer = null;
        }
    }
}
