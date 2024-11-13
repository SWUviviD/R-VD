using UnityEngine;

public class ChasingStarProp : MonoBehaviour
{
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
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
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

                gameObject.SetActive(false);
                isFalling = false;
            }
        }
        else if (other.CompareTag("Panel"))
        {
            gameObject.SetActive(false);
            isFalling = false;
        }
    }


    public void ResetStar()
    {
        isFalling = false;
        transform.position = new Vector3(0, 10, 0);
        gameObject.SetActive(false);
    }
}
