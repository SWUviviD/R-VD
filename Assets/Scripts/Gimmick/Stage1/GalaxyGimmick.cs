using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyGimmick : GimmickBase<GalaxyGimmickData>
{
    [SerializeField] GameObject GalaxyObject;


    private Rigidbody rb;
    private Renderer[] galRenderer;
    private Collider galCollider;
    private Coroutine disappearCoroutine;

    protected override void Init()
    {
        rb = GetComponent<Rigidbody>();
        galCollider = GalaxyObject.GetComponent<Collider>();
        galRenderer = GalaxyObject.GetComponentsInChildren<Renderer>();
    }

    public override void SetGimmick()
    {
        GalaxyObject.transform.localScale = Vector3.one * gimmickData.Size;

        disappearCoroutine = StartCoroutine(AppearDisappearRoutine());
    }

    protected override string GetAddress()
    {
        return "Data/Prefabs/Gimmick/Galaxy";
    }

    private void OnDisable()
    {
        if (disappearCoroutine != null)
        {
            StopCoroutine(disappearCoroutine);
        }
    }


    /// <summary>
    /// 오브젝트 회전
    /// </summary>
    private void FixedUpdate()
    {
        transform.Rotate(0f, 0f, -gimmickData.RotationSpeed, Space.Self);
    }


    /// <summary>
    /// 가시화-비가시화
    /// </summary>
    /// <returns></returns>
    private IEnumerator AppearDisappearRoutine()
    {
        while (true)
        {
            // 오브젝트 가시화, 충돌 처리
            galRenderer.ForEach(_ => _.enabled = true);
            galCollider.enabled = true;
            yield return new WaitForSeconds(gimmickData.VisibleDuration);

            // 오브젝트 비가시화, 충돌 x
            // 하위 렌더러 전체 비가시화 처리 필요함
            galRenderer.ForEach(_ => _.enabled = false);
            galCollider.enabled = false;
            yield return new WaitForSeconds(gimmickData.MaxDisappearTime);
        }
    }


    /// <summary>
    /// 충돌 판정 (+ 플레이어 넉백)
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어와 충돌했을 때
        if (collision.gameObject.TryGetComponent(out PlayerHp playerHp))
        {
            playerHp.Damage(gimmickData.Damage); // 데미지
            PlayerStatus playerStatus = collision.gameObject.GetComponent<PlayerStatus>();

            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // 넉백 방향 계산
                Vector3 knockbackDir = (collision.transform.position - transform.position).normalized;

                // 대쉬 여부에 따른 넉백 거리
                if (playerStatus.IsDashing) // 대쉬 상태면
                {
                    playerRb.AddForce(knockbackDir * (gimmickData.KnockbackForce * 1.5f), ForceMode.Impulse); // 넉백 거리 증가
                }
                else // 대쉬 상태가 아니면
                {
                    playerRb.AddForce(knockbackDir * gimmickData.KnockbackForce, ForceMode.Impulse); // 일반 넉백 거리
                }
            }
        }
    }
}
