using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThornStick : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 3f;

    [SerializeField] private int damage = 1;
    [SerializeField] private float rotateSpeed = 400f;
    [SerializeField] private float movingSpeed = 2f;
    [SerializeField] private Rigidbody rb;
    private static float velocityDamping = 0.98f;

    private ThornStickMap map;
    private WaitForSeconds lifeTime;
    private bool isDroppingStop = false;

    private Vector3 rollDir = Vector3.zero;

    [SerializeField] private LayerMask floorLayer;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rollingClip;
    

    public void Init(ThornStickMap map)
    {
        this.map = map;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = rollingClip;
        audioSource.loop = true;

        lifeTime = new WaitForSeconds(map.StickLifeTime);
    }

    private void OnEnable()
    {
        rb.constraints = RigidbodyConstraints.None;
    }


    public void Drop()
    {
        isDroppingStop = false;
        StartCoroutine(CoDrop());
    }

    private IEnumerator CoDrop()
    {
        yield return lifeTime;

        map.EnqueueStick(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDroppingStop)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBody"))
            {
                Transform parent = collision.transform.parent;
                PlayerHp hp = parent.GetComponent<PlayerHp>();
                hp.Damage(damage);
                parent.GetComponent<Rigidbody>().AddForce(
                    (parent.transform.up + parent.transform.forward * -1f).normalized * knockbackForce,
                    ForceMode.Impulse);
            }

            return;
        }

        if ((floorLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            if (isDroppingStop == true)
                return;

            isDroppingStop = true;

#if UNITY_EDITOR
            Debug.Log("Thorn Sound Play");
#endif
            audioSource.Play();

            rollDir = collision.transform.forward;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isDroppingStop == false)
            return;

        if ((floorLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            isDroppingStop = false;
        }
    }

    private void FixedUpdate()
    {
        if (isDroppingStop)
        {
            rb.velocity = rollDir * -1f * movingSpeed;
            rb.angularVelocity = transform.right * -1f * rotateSpeed;
        }
        else
        {
            rb.velocity *= velocityDamping;
            rb.angularVelocity *= velocityDamping;
        }
    }

    private void OnDisable()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        audioSource.Stop();
    }
}
