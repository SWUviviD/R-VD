using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class PlayerHp : MonoBehaviour
{
    [SerializeField] private PlayerStatus status;
    [SerializeField] private PlayerMove move;

    [SerializeField] private GameObject damageEffectPrefab;
    [SerializeField] private GameObject healEffectPrefab;
    [SerializeField] private GameObject respawnEffectPrefab;
    [SerializeField] private AudioSource audioSource;

    public Vector3 RespawnPoint { get; set; }
    public Vector3 RespawnRotation { get; set; }
    public int CurrentHp { get; private set; }
    public bool IsAlive { get => CurrentHp > 0; }

    public UnityEvent<int> OnDamaged { get; set; } = new UnityEvent<int>();
    public UnityEvent<int> OnHealed { get; set; } = new UnityEvent<int>();
    public UnityEvent<int> OnSetHP { get; set; } = new UnityEvent<int>();
    public UnityEvent OnDeath { get; set; } = new UnityEvent();

    private void OnEnable()
    {
        FullHeal();
        RespawnPoint = transform.position;
        RespawnRotation = transform.rotation.eulerAngles;
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
        audioSource.Play();
        move.StopMoving();
        move.SetPosition(RespawnPoint);
        move.SetRotation(RespawnRotation);
        // 이펙트 출력
        if (respawnEffectPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, -1.7f, 0);
            GameObject respawnEffect = Instantiate(respawnEffectPrefab, spawnPosition, Quaternion.identity);
            Destroy(respawnEffect, 5f);
        }
    }

    /// <summary>
    /// 일정량(amount) 체력 감소
    /// </summary>
    public void Damage(int amount)
    {
        if (IsAlive == false)
            return;

        CurrentHp -= amount;
        LogManager.Log(CurrentHp.ToString());

        // 이펙트 출력
        if (damageEffectPrefab != null)
        {
            GameObject damageEffect = Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
            Destroy(damageEffect, 5f);
        }

        // 사망 처리
        if (CurrentHp <= 0f)
        {
            Die();
        }
        else
        {
            OnDamaged?.Invoke(CurrentHp);
        }
    }

    public void Fall(int amount)
    {
        if (IsAlive == false)
            return;

        Damage(amount);
        if(CurrentHp > 0f)
        {
            Respawn();
        }
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    public void FullHeal()
    {
        CurrentHp = status.HP;
        OnHealed?.Invoke(CurrentHp);
        LogManager.Log(CurrentHp.ToString());

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
        LogManager.Log(CurrentHp.ToString());
        CurrentHp += amount;
        if (CurrentHp > status.HP)
        {
            CurrentHp = status.HP;
        }

        // 이펙트 출력
        if (healEffectPrefab != null)
        {
            GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            healEffect.transform.SetParent(transform);
            Destroy(healEffect, 5f);
        }
    }

    public void SetHealth(int amount)
    {
        CurrentHp = amount;
        OnSetHP?.Invoke(CurrentHp);
    }

    private void OnDisable()
    {
        GameManager.Instance.SaveData();
    }
}
