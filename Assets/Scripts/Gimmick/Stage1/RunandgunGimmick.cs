using System.Collections;
using UnityEngine;

public class RunandgunGimmick : GimmickBase<RunandgunGimmickData>
{
    [SerializeField] private PlayerStatus playerStatus;
    private Coroutine damageCoroutine;

    /// <summary>
    /// 초기화
    /// </summary>
    protected override void Init()
    {
        // 추가 초기화 작업 없음
    }

    public override void SetGimmick()
    {
        // 필요 시 추가 구현
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
                Debug.Log("Heal");
                playerStatus.FullHeal();
            }
            else if (damageCoroutine == null)
            {
                Debug.Log("Damaged");
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
