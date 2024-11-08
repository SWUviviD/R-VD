using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCristalGimmickPlate : MonoBehaviour
{
    private GreenCristalGimmick gimmickMother;

    [SerializeField] private Transform plate;
    [SerializeField] private Collider col;
    [SerializeField] private Renderer[] renders;
    private List<Material> materials = new List<Material>();

    private Vector3 originalScale;
    private WaitForSeconds waitForHide;
    private WaitForSeconds waitForShow;

    public void Init()
    {
        foreach(var r in renders)
        {
            foreach(var m in r.materials)
            {
                materials.Add(m);
            }
        }
    }

    public void SetGimmick(GreenCristalGimmick _mother, Vector3 _position, Vector3 _scale)
    {
        col.enabled = true;

        gimmickMother = _mother;
        transform.position = _position;
        originalScale = _scale;

        waitForHide = new WaitForSeconds(gimmickMother.GimmickData.GimmickShowTime);
        waitForShow = new WaitForSeconds(gimmickMother.GimmickData.GimmickHideTime);

        plate.gameObject.SetActive(false);

        StopAllCoroutines();
    }

    public void AppearPlate()
    {
        StartCoroutine(CoAppearPlate());
    }

    private IEnumerator CoAppearPlate()
    {
        float elapsedTime = 0f;
        float appearTime = gimmickMother.GimmickData.GimmickAppearTime;

        plate.gameObject.SetActive(true);

        while (true)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= appearTime)
            {
                transform.localScale = originalScale;
                break;
            }

            float newScaleRatio = Mathf.Lerp(0f, 1f, elapsedTime / appearTime);
            transform.localScale = originalScale * newScaleRatio;
            yield return null;
        }

        col.enabled = true;

        StartCoroutine(CoShowPlate());
    }

    private IEnumerator CoShowPlate()
    {
        yield return waitForHide;
        DissapearPlate();
    }

    public void DissapearPlate()
    {
        StartCoroutine(CoDissapearPlate());
    }

    private IEnumerator CoDissapearPlate()
    {
        float elapsedTime = 0f;
        float hideTime = gimmickMother.GimmickData.GimmickDissappearTime;

        col.enabled = false;

        while (true)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= hideTime)
            {
                transform.localScale = originalScale * 0f;
                break;
            }

            float newScaleRatio = Mathf.Lerp(1f, 0f, elapsedTime / hideTime);
            transform.localScale = originalScale * newScaleRatio;
            yield return null;
        }

        plate.gameObject.SetActive(false);

        StartCoroutine(CoHidePlate());
    }

    private IEnumerator CoHidePlate()
    {
        yield return waitForHide;
        AppearPlate();
    }

    private void SetMaterialAlpha(float _alpha)
    {
        foreach (var m in materials)
        {
            m.color = new Color(m.color.r, m.color.g, m.color.b, _alpha);
        }
    }
}
