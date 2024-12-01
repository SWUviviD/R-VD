using UnityEngine;

public class ChasingStarProp : GimmickDataBase
{
    [SerializeField] public Transform panel;
    public ChasingGimmick sc;
    public ChasingGimmickData Data;

    private Rigidbody rb;
    private float fallSpeed;
    private float damage;
    private bool isFalling;
    private float KnockbackForce = 120f;
    private float move;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        move = panel.localScale.x / 6.0f;
        SetPosition();
    }


    /// <summary>
    /// 위치 초기화
    /// </summary>
    public void SetPosition()
    {
    }

    public void StartFalling(float speed, float playerDamage)
    {
        fallSpeed = speed;
        damage = playerDamage;
        isFalling = true;
        gameObject.SetActive(true);
    }


    /// <summary>
    /// 떨어지는 속도 조절
    /// </summary>
    private void Update()
    {
        if (isFalling)
        {
            //transform.position += Vector3.down * Time.deltaTime * 10 * Data.StarFallSpeed;
        }
    }


    /// <summary>
    /// 넉백 처리
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //// 플레이어와 충돌했을 때
        //if (collision.gameObject.TryGetComponent(out PlayerStatus playerStatus))
        //{
        //    collision.gameObject.GetComponent<PlayerHp>().Damage(Data.Damage); // 데미지

        //    Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        //    if (playerRb != null)
        //    {
        //        // 넉백 방향 계산
        //        Vector3 knockbackDir = (collision.transform.position - transform.position).normalized;

        //        knockbackDir.y = 0;
        //        knockbackDir.Normalize();

        //        // 대쉬 여부에 따른 넉백 거리
        //        if (playerStatus.IsDashing) // 대쉬 상태면
        //        {
        //            playerRb.AddForce(knockbackDir * (Data.KnockbackForce * 1.5f), ForceMode.Impulse); // 넉백 거리 증가
        //            playerStatus.IsDashing = false;
        //        }
        //        else // 대쉬 상태가 아니면
        //        {
        //            playerRb.AddForce(knockbackDir * Data.KnockbackForce, ForceMode.Impulse); // 일반 넉백 거리
        //        }
        //    }
        //}
        ResetStar();
    }




    public void ResetStar()
    {
        isFalling = false;
        transform.position = new Vector3(panel.transform.position.x, panel.transform.position.y + 30, panel.transform.position.z);
        gameObject.SetActive(false);
    }
}
