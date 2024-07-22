using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] float popOffsetTime;
    [SerializeField] float resetOffsetTime;

    private WaitForSeconds resetSeconds;

    private void Start()
    {
        resetSeconds = new WaitForSeconds(resetOffsetTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(BubblePop());
    }

    private IEnumerator BubblePop()
    {
        // 여기에 터지는 애니메이션 세팅

        yield return resetSeconds;

        // 여기에 다시 제생되는 애니메이션 세팅
    }
}
