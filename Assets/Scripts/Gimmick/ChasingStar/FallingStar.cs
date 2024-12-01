using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class FallingStar : MonoBehaviour
{
    [SerializeField] private Transform star;
    [SerializeField] private Renderer starRenderer;
    private Rigidbody starRigid;

    [SerializeField] private StarShadow shadow;

    [Header("State")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 120f;
    [SerializeField] private float startPositionRadius = 50f;
    [SerializeField] private float startPositionDegree = 130f;
    [SerializeField] private float rotForce = 90f;
    [SerializeField] private float fallingTime = 1f;
    [SerializeField] private float posOffset = 1f;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 startPos = Vector3.zero;
    private Vector3 newPos = Vector3.zero;

    [Header("FadeOut")]
    [SerializeField] private float fadeTime;
    private Color originalColor;
    private Color endColor;

    public void Init()
    {
        starRigid = star.GetComponent<Rigidbody>();

        originalColor = starRenderer.material.color;
        originalColor.a = 1f;
        endColor = originalColor;
        endColor.a = 0f;

        shadow.Init(fallingTime);
        star.gameObject.SetActive(false);
        shadow.gameObject.SetActive(false);
    }

    public void SetGimmick()
    {
        StopAllCoroutines();
        star.gameObject.SetActive(false);
        shadow.gameObject.SetActive(false);
    }

    [ContextMenu("SetPosition")]
    public void StartFalling(Vector3 _targetPosition, Bounds bound)
    {
        //Init();

        //Vector3 _targetPosition = Vector3.zero;

        targetPos = _targetPosition + transform.forward * posOffset;

        startPos.x = 0f;
        startPos.y = Mathf.Sin(startPositionDegree * Mathf.Deg2Rad) * startPositionRadius;
        startPos.z = Mathf.Cos(startPositionDegree * Mathf.Deg2Rad) * startPositionRadius;
        startPos += targetPos;

        starRigid.transform.position = startPos;
        starRigid.AddTorque(Vector3.right * -rotForce, ForceMode.Impulse);
        starRigid.maxAngularVelocity = 500f;

        starRenderer.material.color = originalColor;

        star.gameObject.SetActive(true);
        shadow.gameObject.SetActive(true);

        shadow.SetTargetPosition(targetPos);
        StartCoroutine(CoStarFall());
        shadow.StartFilling();
    }

    private IEnumerator CoStarFall()
    {
        float elapsedTime = 0f;

        while(true)
        {
            newPos = Vector3.Lerp(startPos, targetPos, Mathf.Lerp(0, 1f, (elapsedTime / fallingTime) * (elapsedTime / fallingTime)));
            starRigid.transform.position = newPos;

            if(elapsedTime >= fallingTime)
            {
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        starRigid.transform.position = targetPos;

        starRigid.angularVelocity = Vector3.zero;
        starRigid.velocity = Vector3.zero;

        StartCoroutine(CoFadeOut());
    }

    private IEnumerator CoFadeOut()
    {
        shadow.gameObject.SetActive(false);
        float elapsedTime = 0f;
        while(true)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            starRenderer.material.color = newColor;

            if (elapsedTime >= fadeTime)
                break;

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        starRenderer.material.color = endColor;
        star.gameObject.SetActive(false);
    }

    public void OnPlayerSenced(Collider other)
    {
        // 플레이어와 충돌했을 때
        Transform otherParent = other.transform.parent;
        if (otherParent.TryGetComponent(out PlayerStatus playerStatus))
        {
            otherParent.GetComponent<PlayerHp>().Damage(damage); // 데미지

            Rigidbody playerRb = otherParent.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // 넉백 방향 계산
                Vector3 knockbackDir = (otherParent.GetComponent<PlayerMove>().GetPosition() - transform.position).normalized;

                knockbackDir.y = 0;
                knockbackDir.Normalize();

                // 대쉬 여부에 따른 넉백 거리
                if (playerStatus.IsDashing) // 대쉬 상태면
                {
                    playerRb.AddForce(knockbackDir * (knockbackForce * 1.5f), ForceMode.Impulse); // 넉백 거리 증가
                    playerStatus.IsDashing = false;
                }
                else // 대쉬 상태가 아니면
                {
                    playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse); // 일반 넉백 거리
                }
            }
        }

    }
}
