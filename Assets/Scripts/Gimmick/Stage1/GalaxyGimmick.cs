using System.Collections;
using UnityEngine;

public class GalaxyGimmick : GimmickBase<GalaxyGimmickData>
{
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float minDisappearTime = 2f;
    [SerializeField] private float maxDisappearTime = 5f;

    private Rigidbody rb;
    private Coroutine disappearCoroutine;

    /// <summary>
    /// 상속
    /// </summary>
    protected override void Init()
    {
        // 추가 작업 없음 
    }

    public override void SetGimmick()
    {
        // 추가 작업 없음 
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

    private void FixedUpdate()
    {
        rb.velocity = Vector3.left * moveSpeed;
        rb.angularVelocity = Vector3.back * rotationSpeed;
    }

    private IEnumerator AppearDisappearRoutine()
    {
        while (true)
        { 
            gameObject.SetActive(true);

            yield return new WaitForSeconds(Random.Range(2f, 5f));

            gameObject.SetActive(false);

            yield return new WaitForSeconds(Random.Range(minDisappearTime, maxDisappearTime));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerStatus playerStatus))
        {
            playerStatus.TakeDamage(damageAmount);

            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDir = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}
