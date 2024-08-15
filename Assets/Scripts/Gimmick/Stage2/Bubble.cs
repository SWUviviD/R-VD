using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] GameObject bubbleObject;
    [SerializeField] Collider bubbleCollider;
    [SerializeField] float popOffsetTime;
    [SerializeField] float resetOffsetTime;

    private WaitForSeconds resetSeconds;

    private void Start()
    {
        resetSeconds = new WaitForSeconds(resetOffsetTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerJump jump = null;
        if (collision.gameObject.TryGetComponent<PlayerJump>(out jump) == true)
        {
            jump.Jump();
            StartCoroutine(BubblePop());
        }

    }

    private IEnumerator BubblePop()
    {
        // 여기에 터지는 애니메이션 세팅
        bubbleObject.SetActive(false);
        bubbleCollider.enabled = false;

        yield return resetSeconds;

        // 여기에 다시 제생되는 애니메이션 세팅
        bubbleObject.SetActive(true);
        bubbleCollider.enabled = true;
    }
}
