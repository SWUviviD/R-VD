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
        if (sc.starList.Length == 1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        else if (sc.starList.Length == 2)
        {
            if (sc.currentStarIndex == 0)
            {
                transform.position = new Vector3(transform.position.x + move, transform.position.y, transform.position.z + move);
            }

            else if (sc.currentStarIndex == 1)
            {
                transform.position = new Vector3(transform.position.x - 2 * move, transform.position.y, transform.position.z - 2 * move);
            }
        }
        else if (sc.starList.Length == 3)
        {
            if (sc.currentStarIndex == 0)
            {
                transform.position = new Vector3(transform.position.x + move, transform.position.y, transform.position.z + move);
            }

            else if (sc.currentStarIndex == 1)
            {
                transform.position = new Vector3(transform.position.x - 2 * move, transform.position.y, transform.position.z - 2 * move);
            }

            else if (sc.currentStarIndex == 2)
            { 
                transform.position = new Vector3(transform.position.x - 2 * move, transform.position.y, transform.position.z + 2 * move);
            }
        }
        else if (sc.starList.Length == 4)
        {
            if (sc.currentStarIndex == 0)
            {
                transform.position = new Vector3(transform.position.x + move, transform.position.y, transform.position.z + move);
            }

            else if (sc.currentStarIndex == 1)
            {
                transform.position = new Vector3(transform.position.x - move, transform.position.y, transform.position.z + move);
            }

            else if (sc.currentStarIndex == 2)
            {
                transform.position = new Vector3(transform.position.x + move, transform.position.y, transform.position.z - move);
            }

            else if (sc.currentStarIndex == 3)
            {
                transform.position = new Vector3(transform.position.x - move, transform.position.y, transform.position.z - move);
            }
        }
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
            transform.position += Vector3.down * Time.deltaTime * 10 * Data.StarFallSpeed;
        }
    }


    /// <summary>
    /// 넉백 처리
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerStatus>();
            if (player != null)
            {
                player.TakeDamage(damage);

                Rigidbody playerRb = other.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 knockbackDir = (other.transform.position - transform.position).normalized;

                    float appliedKnockback = player.IsDashing ? KnockbackForce * 1.5f : KnockbackForce;
                    playerRb.AddForce(knockbackDir * appliedKnockback, ForceMode.Impulse);
                }
            }

            Debug.Log("player");
        }

        Destroy(gameObject);

        ResetStar();
    }


    public void ResetStar()
    {
        isFalling = false;
        transform.position = new Vector3(transform.position.x, panel.transform.position.y + 30, transform.position.z);
        gameObject.SetActive(false);
    }
}
