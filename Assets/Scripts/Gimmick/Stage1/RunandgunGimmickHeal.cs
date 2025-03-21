using System.Collections;
using UnityEngine;

public class RunandgunGimmickHeal : GimmickBase<RunandgunGimmickData>, IFloorInteractive
{
    private PlayerHp playerHp;
    private Coroutine damageCoroutine;

    [SerializeField] private GameObject editorObj;
    [SerializeField] private AudioSource audioSource;

    /// <summary>
    /// 초기화
    /// </summary>
    protected override void Init()
    {
    }

    public override void SetGimmick()
    {
        // 필요 시 추가 구현
        editorObj.SetActive(false);
    }

    /// <summary>
    /// 데미지를 일정 시간 간격으로 적용
    /// </summary>
    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            playerHp?.Damage(GimmickData.DamageAmount);
            yield return new WaitForSeconds(GimmickData.DamageTickInterval);
        }
    }

    public void InteractStart(GameObject player)
    {
        playerHp = player.GetComponent<PlayerHp>();

        audioSource.Play();
        if (gameObject.tag == "Heal")
        {
            playerHp?.Heal((int)GimmickData.HealAmount);
        }
        else if (damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DamageOverTime());
        }
    }

    public void InteractEnd(GameObject player)
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
