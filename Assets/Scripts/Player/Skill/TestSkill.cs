using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill : MonoBehaviour
{
    [SerializeField] PlayerStatus status;

    [SerializeField] TestProjectile projectile;

    public enum SkillMode
    {
        Slow, // 느린 이속 화살
        Stop, // 정지 화살
    }

    private SkillMode currMode;

    private bool isUseSkill;

    private void Start()
    {
        isUseSkill = false;
        currMode = SkillMode.Slow;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && isUseSkill == false)
        {
            currMode = SkillMode.Slow;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && isUseSkill == false)
        {
            currMode = SkillMode.Stop;   
        }

        if (Input.GetKeyDown(KeyCode.J) && isUseSkill == false)
        {
            PrepareShot();
            isUseSkill = true;
        }

        if (Input.GetKeyUp(KeyCode.J) && isUseSkill)
        {
            Shot();
            isUseSkill = false;
        }
    }

    private void PrepareShot()
    {
        switch (currMode)
        {
            case SkillMode.Slow:
                status.SetMoveDelay(true);
                break;
            case SkillMode.Stop:
                status.SetMoveStop(true);
                break;
        }
    }

    private void Shot()
    {
        projectile.transform.position = transform.position;
        projectile.transform.position += Vector3.up;
        projectile.Fire(status.AttackRange, status.ProjectileSpeed, transform.forward);
        switch (currMode)
        {
            case SkillMode.Slow:
                status.SetMoveDelay(false);
                break;
            case SkillMode.Stop:
                status.SetMoveStop(false);
                break;
        }
    }
}
