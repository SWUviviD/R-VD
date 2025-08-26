using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffector : MonoBehaviour
{
    [SerializeField] private Transform cam;
    private Camera camScript;
    private float originalFOV;

    private float currentScale;

    private void Awake()
    {
        camScript = cam.GetComponent<Camera>();
        if(camScript == null)
        {
            Debug.LogError("Cam is Not Valide");
        }

        originalFOV = camScript.fieldOfView;
        currentScale = 1f;
    }

    public void Zoom(float _originalScale, float _newScale, float _changeTime = 0f, Action _callback = null)
    {
        StartCoroutine(CoZoom(_originalScale, _newScale, _changeTime, _callback));
    }

    private IEnumerator CoZoom(float _originalScale, float _newScale, float _changeTime, Action _callback)
    {
        camScript.fieldOfView = originalFOV * _originalScale;
        currentScale = _originalScale;

        float elapsedTime = 0f;
        while(elapsedTime < _changeTime)
        {
            currentScale = Mathf.Lerp(_originalScale, _newScale, elapsedTime / _changeTime);
            yield return null;
        }

        camScript.fieldOfView = originalFOV * _newScale;
        _callback?.Invoke();
    }

    public void Shake(float _duration, float _magnitude)
    {
        StartCoroutine(CoShake(_duration, _magnitude));
    }

    private IEnumerator CoShake(float _duration, float _magnitude)
    {
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * _magnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * _magnitude;

            cam.localPosition = new Vector3(offsetX, offsetY, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = Vector3.zero;
    }
}
