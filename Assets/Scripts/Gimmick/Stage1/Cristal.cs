using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cristal : GimmickTrigger
{
    [Header("Cristal")]
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private GameObject cristalGO;
    [SerializeField] private GameObject starPathGO;
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private int scaleUpTime = 60;

    protected virtual void Start()
    {
        cristalGO.SetActive(true);
        starPathGO.SetActive(false);
        StartCoroutine(IRotate(rotateSpeed));
    }

    protected override void Interact()
    {
        isActive = false;
        capsuleCollider.enabled = false;
        cristalGO.SetActive(false);
        starPathGO.SetActive(true);
        StartCoroutine(IScaleUp(scaleUpTime));
    }

    /// <summary> 크리스탈 회전 샘플 </summary>
    private IEnumerator IRotate(float speed)
    {
        while (isActive)
        {
            cristalGO.transform.Rotate(Vector3.up * speed);
            yield return null;
        }
    }

    /// <summary> 빙판 생성 연출 샘플 </summary>
    private IEnumerator IScaleUp(int time)
    {
        for (int i = 0; i <= time; ++i)
        {
            starPathGO.transform.localScale = Vector3.one * i / time;
            yield return null;
        }
    }
}
