using System.Collections;
using UnityEngine;

public class RunandgunGimmickHeal : GimmickBase<RunandgunGimmickData>
{
    private PlayerStatus playerStatus;
    private Coroutine damageCoroutine;

    /// <summary>
    /// 초기화
    /// </summary>
    protected override void Init()
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
    }

    public override void SetGimmick()
    {
        // 필요 시 추가 구현
    }

    protected override string GetAddress()
    {
        return "Assets/Data/Prefabs/Gimmick/RunandGun/HealZone.prefab";
    }


    /// <summary>
    /// 트리거 처리
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.tag == "Heal")
            {
                playerStatus.FullHeal();
            }
            else if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamageOverTime());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.tag != "Heal")
            {
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                    damageCoroutine = null;
                }
            }
        }
    }

    /// <summary>
    /// 데미지를 일정 시간 간격으로 적용
    /// </summary>
    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            playerStatus.Damage(GimmickData.DamageAmount);
            yield return new WaitForSeconds(GimmickData.DamageTickInterval);
        }
    }
}
