using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarShadow : MonoBehaviour
{
    [SerializeField] private Transform bound;
    [SerializeField] private Transform fill;

    Vector3 newScale = Vector3.zero;
    Vector3 fullScale;

    private float fillTime = 0f;

    private bool isInit = false;

    public void Init(float _fillTime)
    {
        fullScale = bound.localScale;
        fillTime = _fillTime;
        isInit = true;
    }

    public void SetGimmick()
    {
        StopAllCoroutines();
    }

    public bool StartFilling()
    {
        if (isInit == false)
            return false;

        StartCoroutine(CoStartFilling());
        return true;
    }

    public void SetTargetPosition(Vector3 _position)
    {
        transform.position = _position;
    }

    private IEnumerator CoStartFilling()
    {
        float elapsedTime = 0f;
        while(true)
        {
            newScale = Vector3.Lerp(Vector3.zero, fullScale, elapsedTime / fillTime);
            fill.localScale = newScale;

            if (elapsedTime >= fillTime)
                break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
