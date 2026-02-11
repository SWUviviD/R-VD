using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Cart : MonoBehaviour, IFloorInteractive
{
    [SerializeField] private Rigidbody cartTrans;
    [SerializeField] private Transform siroPos;
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

    private PlayerMove playerMove;
    private Rigidbody ridingPlayer;
    private LevitateAroundPlayer pet;

    private void Awake()
    {
        audioSource.clip = movingSound;
        audioSource.loop = true;

        isMovingForward = startMovingForward;
        nextIndex = GetNextIndex(startIndex);

        cartTrans.position = Rails[startIndex].CenterPos.position;
        isMoving = false;

        dir = (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized;

        pet = FindObjectOfType<LevitateAroundPlayer>();
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

    private Vector3 delta = Vector3.zero;
    private void FixedUpdate()
    {
        if (isMoving == false) return;

        Vector3 before = cartTrans.position;

        if (Vector3.Dot(dir, (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized) < 0)
        {
            cartTrans.MovePosition(Rails[nextIndex].CenterPos.position);
            nextIndex = GetNextIndex(nextIndex);
            dir = (Rails[nextIndex].CenterPos.position - cartTrans.position).normalized;
        }

        Vector3 nextPos = cartTrans.position + (dir * Time.fixedDeltaTime * movingSpeed);
        cartTrans.MovePosition(nextPos);

        delta = nextPos - before;

        if (ridingPlayer != null)
        {
            ridingPlayer.MovePosition(ridingPlayer.position + delta);
        }
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

    public void InteractStart(GameObject player)
    {
        if(player.TryGetComponent<PlayerMove>(out var playerMove))
        {
            ridingPlayer = playerMove.GetComponent<Rigidbody>();
            this.playerMove = playerMove.GetComponent<PlayerMove>();
            if (pet != null) pet.SitDown(siroPos);
        }
    }

    public void InteractEnd(GameObject player)
    {
        if (player.TryGetComponent<PlayerMove>(out var playerMove))
        {
            this.playerMove = null;
            ridingPlayer = null;
            if(pet != null) pet.SetTargetPlayer(playerMove.transform);
        }
    }
}
