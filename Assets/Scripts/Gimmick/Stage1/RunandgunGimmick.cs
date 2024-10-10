using UnityEngine;

public class RunandgunGimmick : GimmickBase<RunandgunGimmickData>
{
    [Header("Gimmick Configuration")]

    [Tooltip("플레이어 레이어")]
    [SerializeField] private LayerMask playerLayerMask;

    private bool isPlayerInside = false;
    private float timer = 0f;

    private PlayerStatus playerStatus;

    protected override void Init()
    {
        
    }

    public override void SetGimmick()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
        {
            isPlayerInside = true;
            playerStatus = other.GetComponent<PlayerStatus>();

            if (playerStatus == null)
            {
                LogManager.LogWarning("RunandgunGimmick: 플레이어 오브젝트에 PlayerStatus 컴포넌트가 없습니다.");
                return;
            }

            if (gimmickData.IsHealZone)
            {
                if (gimmickData.HealAmount <= 0f)
                {
                    playerStatus.FullHeal();
                }
                else
                {
                    playerStatus.Heal(gimmickData.HealAmount);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
        {
            isPlayerInside = false;
            timer = 0f;
            playerStatus = null;
        }
    }

    private void Update()
    {
        if (isPlayerInside && !gimmickData.IsHealZone && playerStatus != null && playerStatus.IsAlive())
        {
            timer += Time.deltaTime;
            if (timer >= gimmickData.DamageInterval)
            {
                timer -= gimmickData.DamageInterval;

                float damageAmount = playerStatus.MaxHealth * gimmickData.DamagePercentage;
                playerStatus.TakeDamage(damageAmount);
            }
        }
    }
}
