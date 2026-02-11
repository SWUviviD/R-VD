using UnityEngine;

public class LevitateAroundPlayer : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Offsets")]
    [SerializeField] private float teleportDistance = 10f;
    [SerializeField] private Vector3 baseCenterPos = new Vector3(1.5f, 1.5f, -0.5f);
    [SerializeField] private Vector3 rotateOffset = new Vector3(0f, -90f, 0f);

    [Header("Floating Effect")]
    [SerializeField] private float floatingTime = 2f;
    [SerializeField] private float floatingAmplitude = 0.2f;

    [Header("Rotation")]
    [SerializeField] private float rotLerpSpeed = 10f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 currentPositionWithoutHover;

    private Transform ridingTR;
    private bool isSitDown = false;
    private bool isSeated = false;

    private void LateUpdate()
    {
        if (isSitDown)
        {
            if (ridingTR == null || player == null) return;

            if (!isSeated)
            {
                float sitSpeedMultiplier = 2f;
                float time = 1f - Mathf.Exp(-(10f * sitSpeedMultiplier) * Time.deltaTime);

                currentPositionWithoutHover = Vector3.Lerp(
                    currentPositionWithoutHover,
                    ridingTR.position,
                    time
                );

                transform.position = currentPositionWithoutHover;

                if (Vector3.SqrMagnitude(transform.position - ridingTR.position) < 0.25f)
                {
                    transform.SetParent(ridingTR);
                    transform.localPosition = Vector3.zero;
                    isSeated = true;
                }
            }

            Vector3 lookTarget = player.position;
            Vector3 dir = (lookTarget - transform.position).normalized;

            if (dir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir, Vector3.up),
                    1f - Mathf.Exp(-rotLerpSpeed * Time.deltaTime)
                );
            }

            return;
        }


        if (player == null) return;

        Vector3 targetPos = player.position + player.rotation * baseCenterPos;

        // 지수 추적 (프레임 독립적)
        float followSharpness = 8f; // 높을수록 빨리 붙음
        float t = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);

        currentPositionWithoutHover = Vector3.Lerp(
            currentPositionWithoutHover,
            targetPos,
            t
        );

        Quaternion desiredRot = player.rotation * Quaternion.Euler(rotateOffset);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            1f - Mathf.Exp(-rotLerpSpeed * Time.deltaTime)
        );

        float hover = Mathf.Sin(Time.time * (Mathf.PI * 2 / floatingTime)) * floatingAmplitude;
        transform.position = currentPositionWithoutHover + Vector3.up * hover;
    }

    public void SitDown(Transform tr)
    {
        isSitDown = true;
        isSeated = false;
        ridingTR = tr;
    }

    public void SetTargetPlayer(Transform target)
    {
        isSitDown = false;
        isSeated = false;
        player = target;
    }
}