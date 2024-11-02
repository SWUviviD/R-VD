using System.Collections;
using UnityEngine;

public class GalaxyGimmick : GimmickBase<GalaxyGimmickData>
{
    [SerializeField] private float rotationSpeed;  // 회전 속도
    [SerializeField] private float minDisappearTime; // 사라지는 최소 시간
    [SerializeField] private float maxDisappearTime; // 사라지는 최대 시간
    [SerializeField] private float fadeDuration = 0.3f; // 페이드 인/아웃 시간

    private Rigidbody rb;
    private Coroutine disappearCoroutine;
    private Renderer gimmickRenderer; // 페이드 효과용 렌더러

    protected override void Init()
    {
        // 추가 작업 없음
    }

    public override void SetGimmick()
    {
        rotationSpeed = 100f;
        minDisappearTime = 2f;
        maxDisappearTime = 5f;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gimmickRenderer = GetComponent<Renderer>();
    }

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

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.back * rotationSpeed; // Z축을 기준으로 회전
    }

    /// <summary>
    /// 오브젝트 비활성화-활성화 코루틴
    /// </summary>
    private IEnumerator AppearDisappearRoutine()
    {
        while (true)
        {
            yield return FadeIn(); // 페이드 인 효과
            yield return new WaitForSeconds(gimmickData.VisibleDuration); // 활성화
            yield return FadeOut(); // 페이드 아웃 효과

            yield return new WaitForSeconds(Random.Range((int)minDisappearTime, (int)maxDisappearTime + 1)); // 비활성화 유지
        }
    }

    /// <summary>
    /// 페이드 인
    /// </summary>
    private IEnumerator FadeIn()
    {
        Color color = gimmickRenderer.material.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); // 알파 값을 서서히 증가
            gimmickRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gimmickRenderer.material.color = new Color(color.r, color.g, color.b, 1f); // 완전히 가시화
    }

    /// <summary>
    /// 페이드 아웃
    /// </summary>
    private IEnumerator FadeOut()
    {
        Color color = gimmickRenderer.material.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration); // 알파 값을 서서히 감소
            gimmickRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gimmickRenderer.material.color = new Color(color.r, color.g, color.b, 0f); // 완전히 비가시화
        gameObject.SetActive(false); // 비가시화 상태로 전환
    }

    /// <summary>
    /// 충돌 판정 (+ 넉백)
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어와 충돌했을 때
        if (collision.gameObject.TryGetComponent(out PlayerStatus playerStatus))
        {
            playerStatus.TakeDamage(gimmickData.Damage); // 데미지

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
