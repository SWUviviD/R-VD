using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [field: SerializeField]
    public float MoveSpeed { get; private set; }
    [field: SerializeField]
    public float AdditionalMoveSpeed { get; set; }
    
    [field: SerializeField]
    public float AttackRange { get; private set; }
    [field: SerializeField]
    public float ProjectileSpeed { get; private set; }
    [field: SerializeField]
    public float JumpHeight { get; private set; }
    [field: SerializeField]
    public float JumpSpeed { get; private set; }
    [field: SerializeField]
    public float FallingSpeed { get; private set; }

    [field: SerializeField]
    public bool IsDashing { get; set; }

    [field: SerializeField]
    public float DashSpeed { get; private set; }

    [field: SerializeField]
    public float DashTime { get; private set; }

    [field: SerializeField]
    public Vector3 RespawnPoint { get; private set; }


    [field: SerializeField]
    public float HP { get; set; }


    private void Awake()
    {
        HP = 100f;
        RespawnPoint = transform.position;
    }


    public float GetMoveSpeed()
    {
        return MoveSpeed + AdditionalMoveSpeed;
    }

    public void SetMoveDelay(bool set)
    {
        AdditionalMoveSpeed = set ? -MoveSpeed * 0.75f : 0f;
    }
    
    public void SetMoveStop(bool set)
    {
        AdditionalMoveSpeed = set ? -MoveSpeed : 0f;
    }

    public void SetRespawnPoint(Vector3 pos)
    {
        RespawnPoint = pos;
    }

    public bool IsAlive()
    {
        return HP > 0f;
    }

    private void Die()
    {
        // 사망 확인
        // 추가 구현 필요

        // 플레이어 체력 회복 (임시 추가)
        FullHeal();

        // 플레이어 위치를 리스폰 지점으로 이동
        transform.position = RespawnPoint;
    }


    /// <summary>
    /// 일정량(amount) 체력 감소
    /// </summary>
    public void Damage(float amount)
    {
        HP -= amount;
        Debug.Log(HP);

        // 사망 처리
        if (HP <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    public void FullHeal()
    {
        HP = 100f;
        Debug.Log(HP);
    }

    public void Heal(float amount)
    {
        Debug.Log(HP);
        HP += amount;
        if (HP > 100)
        {
            HP = 100;
        }
    }
}
