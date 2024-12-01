using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    /// <summary> 화살 발사를 위한 스파인. 활을 쏘는 순간에 돌려야한다. </summary>
    [SerializeField] private Transform trSpine;

    private PlayerMove move;
    private PlayerDash dash;
    private PlayerHp hp;
    private StarHunt skill;

    private bool isShooting;

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

        isShooting = false;
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
        isShooting = true;
    }

    private void OnShotEnd()
    {
        animator.SetBool(IsShotingID, false);
        isShooting = false;
        trSpine.localRotation = Quaternion.identity;
    }

    private void OnDeath()
    {
        animator.SetTrigger(OnDeathID);
    }

    private void LateUpdate()
    {
        if(isShooting) trSpine.localRotation = Quaternion.Euler(Vector3.up * 90f);
    }
}
