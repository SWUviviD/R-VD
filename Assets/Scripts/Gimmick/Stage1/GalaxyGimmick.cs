using System.Collections;
using UnityEngine;

public class GalaxyGimmick : GimmickBase<GalaxyGimmickData>
{
    [SerializeField] private float rotationSpeed;  // 회전 속도
    [SerializeField] private float minDisappearTime; // 사라지는 최소 시간
    [SerializeField] private float maxDisappearTime; // 사라지는 최대 시간

    private Rigidbody rb;
    private Renderer galRenderer;
    private Collider galCollider;
    private Coroutine disappearCoroutine;

    protected override void Init()
    {
        // 추가 작업 없음
    }

    public override void SetGimmick()
    {
        rotationSpeed = 30f; 
        minDisappearTime = 2f; 
        maxDisappearTime = 5f; 
    }

    protected override string GetAddress()
    {
        return "Assets/Data/Prefabs/Gimmick/Galaxy.prefab";
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        galRenderer = GetComponent<Renderer>();
        galCollider = GetComponent<Collider>();
    }


    /// <summary>
    /// 가시화-비가시화 상태의 코루틴 지정
    /// </summary>
    private void OnEnable()
    {
        disappearCoroutine = StartCoroutine(AppearDisappearRoutine());
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
        rb.angularVelocity = Vector3.back * rotationSpeed; // z축을 기준으로 회전
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
            galRenderer.enabled = true;
            galCollider.enabled = true;
            yield return new WaitForSeconds(gimmickData.VisibleDuration);

            // 오브젝트 비가시화, 충돌 x
            galRenderer.enabled = false;
            galCollider.enabled = false;
            yield return new WaitForSeconds(Random.Range(2, 6));
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
