using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bubble : GimmickBase<BubbleData>, IFloorInteractive
{
    [SerializeField] GameObject bubbleObject;
    [SerializeField] Collider bubbleCollider;
    [SerializeField] private Transform deco;
    [SerializeField] private float decoRotateSpeed = 180f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject crackEffect;

    private WaitForSeconds resetSeconds;

    protected override void Init()
    {
        SetGimmick();
    }

    [ContextMenu("SetMenu")]
    public override void SetGimmick()
    {
        //bubbleObject.transform.localScale = Vector3.one * gimmickData.BubbleSize;
        //deco.transform.localScale = Vector3.one * gimmickData.BubbleSize;

        resetSeconds = new WaitForSeconds(gimmickData.ResetOffsetTime);

        StopAllCoroutines();
        bubbleObject.SetActive(true);
        bubbleCollider.enabled = true;

        bubbleObject.transform.localScale = Vector3.one * gimmickData.BubbleSize;
        deco.transform.localScale = Vector3.one * gimmickData.BubbleSize;
    }

    private void Update()
    {
        if (deco == null)
            return;

        deco.transform.Rotate(Vector3.up * decoRotateSpeed * Time.deltaTime);
    }

    private IEnumerator BubblePop()
    {
        // 여기에 터지는 애니메이션 세팅
        audioSource.Play();
        GameObject watermove = Instantiate(crackEffect, transform.position, Quaternion.identity);
        bubbleObject.SetActive(false);
        bubbleCollider.enabled = false;

        yield return resetSeconds;

        // 여기에 다시 제생되는 애니메이션 세팅
        bubbleObject.SetActive(true);
        bubbleCollider.enabled = true;
    }

    public void InteractStart(GameObject player)
    {
        PlayerJump jump = player?.GetComponent<PlayerJump>();
        if (jump == null)
            return;

        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0f;

        if(dir.sqrMagnitude < 0.00001f)
        {
            dir = player.transform.forward;
            dir.y = 0f;
        }

        jump.Jump(dir, gimmickData.JumpForce, gimmickData.JumpForce);
        StartCoroutine(BubblePop());
    }

    public void InteractEnd(GameObject player)
    {
    }
}
