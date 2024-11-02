using System.Collections;
using UnityEngine;

public class GalaxyGimmick : GimmickBase<GalaxyGimmickData>
{
    [SerializeField] private float rotationSpeed;  // 회전 속도
    [SerializeField] private float minDisappearTime; // 사라지는 최소 시간
    [SerializeField] private float maxDisappearTime; // 사라지는 최대 시간

    private Rigidbody rb;
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
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


    /// <summary>
    /// 오브젝트 회전
    /// </summary>
    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.back * rotationSpeed; // Z축을 기준으로 회전
    }

    /// <summary>
    /// 페이드아웃-페이드인
    /// </summary>
    /// <returns></returns>
    private IEnumerator AppearDisappearRoutine()
    {
        while (true)
        {
            // 기믹 활성화
            gameObject.SetActive(true);
            yield return new WaitForSeconds(gimmickData.VisibleDuration); // 가시화 유지 시간

            yield return StartCoroutine(FadeOutRoutine()); // 서서히 사라짐

            // 기믹 비활성화
            gameObject.SetActive(false);

            // 비활성화 상태 유지
            yield return new WaitForSeconds(2f);

            // 서서히 나타남
            yield return StartCoroutine(FadeInRoutine());
        }
    }

    private IEnumerator FadeOutRoutine()
    {
        Material material = GetComponent<Renderer>().material;
        Color color = material.color;

        // 현재 알파 값을 유지하면서 서서히 감소
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1, 0, t / 0.3f); // 알파 값 1에서 0으로 변경
            material.color = color;
            yield return null; // 다음 프레임까지 대기
        }

        color.a = 0; // 완전히 투명하게 설정
        material.color = color; // 색상 적용
    }

    private IEnumerator FadeInRoutine()
    {
        Material material = GetComponent<Renderer>().material;
        Color color = material.color;

        // 현재 알파 값을 유지하면서 서서히 증가
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0, 1, t / 0.3f); // 알파 값 0에서 1로 변경
            material.color = color;
            yield return null; // 다음 프레임까지 대기
        }

        color.a = 1; // 완전히 불투명하게 설정
        material.color = color; // 색상 적용
    }


    /// <summary>
    /// 충돌 판정 (+ 플레이어 넉백)
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
