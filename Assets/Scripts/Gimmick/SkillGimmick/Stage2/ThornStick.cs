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

    private Transform floor;

    public void Init(ThornStickMap map)
    {
        this.map = map;
        lifeTime = new WaitForSeconds(map.StickLifeTime);
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
            if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBody")
                || collision.gameObject.layer == LayerMask.NameToLayer("PlayerBody"))
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

        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            isDroppingStop = true;

            floor = collision.transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isDroppingStop == false)
            return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            isDroppingStop = false;
        }
    }

    private void FixedUpdate()
    {
        if (isDroppingStop)
        {
            rb.velocity = floor.forward * -1f * movingSpeed;
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
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
