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

    /// <summary>
    /// Runandgun 기믹을 위한 체력 힐 세팅
    /// </summary>
    [Header("Health Settings")]
    [SerializeField]
    private float maxHealth = 100f;
    public float MaxHealth => maxHealth;

    [SerializeField]
    private float currentHealth;
    public float CurrentHealth => currentHealth;


    private void Awake()
    {
        currentHealth = maxHealth;
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
        return currentHealth > 0f;
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
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // 사망 처리
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 일정량(amount) 체력 회복
    /// </summary>
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    /// <summary>
    /// 100% 체력 회복
    /// </summary>
    public void FullHeal()
    {
        currentHealth = maxHealth;
    }

    public float GetHealthPercentage()
    {
        return (currentHealth / maxHealth) * 100f;
    }
}
