using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHp : MonoBehaviour
{
    [SerializeField] private PlayerStatus status;

    public Vector3 RespawnPoint { get; set; }
    private int currentHp;
    public bool IsAlive { get => currentHp > 0; }

    public UnityEvent<int> OnDamaged {  get; set; } = new UnityEvent<int>();
    public UnityEvent OnDeath { get; set; } = new UnityEvent();

    private void OnEnable()
    {
        FullHeal();
        RespawnPoint = transform.position;
    }

    private void Die()
    {
        // 사망 확인
        // 추가 구현 필요

        // 플레이어 체력 회복 (임시 추가)
        //FullHeal();

        OnDeath?.Invoke();

        // 플레이어 위치를 리스폰 지점으로 이동
        transform.position = RespawnPoint;
    }


    /// <summary>
    /// 일정량(amount) 체력 감소
    /// </summary>
    public void Damage(int amount)
    {
        currentHp -= amount;
        LogManager.Log(currentHp.ToString());

        // 사망 처리
        if (currentHp <= 0f)
        {
            Die();
        }
        else
        {
            OnDamaged?.Invoke(currentHp);
        }
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    public void FullHeal()
    {
        currentHp = status.HP;
        OnDamaged?.Invoke(currentHp);
        LogManager.Log(currentHp.ToString());
    }

    public void Heal(int amount)
    {
        LogManager.Log(currentHp.ToString());
        currentHp += amount;
        if (currentHp > status.HP)
        {
            currentHp = status.HP;
        }
    }
}
