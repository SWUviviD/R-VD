using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private PlayerMove move;
    private PlayerDash dash;
    private PlayerHp hp;
    private StarHunt skill;

    private readonly int OnDeathID = Animator.StringToHash("OnDeath");
    private readonly int OnShotStartID = Animator.StringToHash("OnShotStart");
    private readonly int IsDashingID = Animator.StringToHash("IsDashing");
    private readonly int IsShotingID = Animator.StringToHash("IsShoting");
    private readonly int IsJumpingID = Animator.StringToHash("IsJumping");
    private readonly int IsWalkingID = Animator.StringToHash("IsWalking");
    private readonly int IsFallingID = Animator.StringToHash("IsFalling");

    private void Awake()
    {
        move = GetComponent<PlayerMove>();
        move.OnMove.RemoveListener(OnMove);
        move.OnMove.AddListener(OnMove);

        dash = GetComponent<PlayerDash>();
        dash.OnDashEvent?.RemoveListener(OnDash);
        dash.OnDashEvent?.AddListener(OnDash);


        hp = GetComponent<PlayerHp>();
        hp.OnDeath?.RemoveListener(OnDeath);
        hp.OnDeath?.AddListener(OnDeath);


        skill = GetComponent<StarHunt>();
        skill.OnStarHuntKeyDown.RemoveListener(OnShotStart);
        skill.OnStarHuntKeyDown.AddListener(OnShotStart);
        skill.OnStarHuntKeyUp.RemoveListener(OnShotEnd);
        skill.OnStarHuntKeyUp.AddListener(OnShotEnd);
    }


    private bool isMoving = true;
    private void OnMove(bool _isMoving)
    {
        if(isMoving != _isMoving)
        {
            animator.SetBool(IsWalkingID, _isMoving);
            isMoving = _isMoving;
        }
    }

    private bool isDashing;
    private void OnDash(bool _isDash)
    {
        if(isDashing != _isDash)
        {
            animator.SetBool(IsDashingID, _isDash);
            isDashing = _isDash;
        }
    }

    private void OnShotStart()
    {
        animator.SetTrigger(OnShotStartID);
        animator.SetBool(IsShotingID, true);
    }

    private void OnShotEnd()
    {
        animator.SetBool(IsShotingID, false);
    }

    private void OnDeath()
    {
        animator.SetTrigger(OnDeathID);
    }
}
