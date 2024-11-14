using UnityEngine;

public class ChasingStarProp : GimmickDataBase
{
    [SerializeField] public Transform panel;
    public ChasingGimmickData Data;

    private Rigidbody rb;
    private float fallSpeed;
    private float damage;
    private bool isFalling;
    private float KnockbackForce = 120f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void StartFalling(float speed, float playerDamage)
    {
        fallSpeed = speed;
        damage = playerDamage;
        isFalling = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isFalling)
        {
            transform.position += Vector3.down * Data.StarFallSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
        }

        ResetStar();
    }


    public void ResetStar()
    {
        isFalling = false;
        transform.position = new Vector3(transform.position.x, panel.transform.position.y + 20, transform.position.z);
        gameObject.SetActive(false);
    }
}
