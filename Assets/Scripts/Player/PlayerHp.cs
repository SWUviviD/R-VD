using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHp : MonoBehaviour
{
    [SerializeField] private PlayerStatus status;
    [SerializeField] private PlayerMove move;

    [SerializeField] private GameObject damageEffectPrefab;
    [SerializeField] private GameObject healEffectPrefab;
    [SerializeField] private GameObject respawnEffectPrefab;

    public Vector3 RespawnPoint { get; set; }
    private int currentHp;
    public bool IsAlive { get => currentHp > 0; }

    public UnityEvent<int> OnDamaged { get; set; } = new UnityEvent<int>();
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
    }

    public void Respawn()
    {
        move.SetPosition(RespawnPoint);
        CameraController.Instance.Reset();
        CameraController.Instance.Play();
        // 이펙트 출력
        if (damageEffectPrefab != null)
        {
            GameObject respawnEffect = Instantiate(respawnEffectPrefab, transform.position, Quaternion.identity);
            Destroy(respawnEffect, 5f);
        }
    }

    /// <summary>
    /// 일정량(amount) 체력 감소
    /// </summary>
    public void Damage(int amount)
    {
        currentHp -= amount;
        LogManager.Log(currentHp.ToString());

        // 이펙트 출력
        if (damageEffectPrefab != null)
        {
            GameObject damageEffect = Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
            Destroy(damageEffect, 5f);
        }

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

    public void Fall(int amount)
    {
        if (IsAlive == false)
            return;

        Damage(amount);
        if(currentHp > 0f)
        {
            Respawn();
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

        // 이펙트 출력
        if (healEffectPrefab != null)
        {
            GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            healEffect.transform.SetParent(transform);
            Destroy(healEffect, 5f);
        }
    }

    public void Heal(int amount)
    {
        LogManager.Log(currentHp.ToString());
        currentHp += amount;
        if (currentHp > status.HP)
        {
            currentHp = status.HP;
        }

        // 이펙트 출력
        if (healEffectPrefab != null)
        {
            GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            healEffect.transform.SetParent(transform);
            Destroy(healEffect, 5f);
        }
    }
}
