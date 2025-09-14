using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffector : MonoBehaviour
{
    private Camera camScript;
    private float originalFOV;

    private float currentScale;

    private void Awake()
    {
        camScript = transform.GetComponent<Camera>();
        if(camScript == null)
        {
            Debug.LogError("Cam is Not Valide");
        }

        originalFOV = camScript.fieldOfView;
        currentScale = 1f;
    }

    public void StopAllEffect()
    {
        StopAllCoroutines();
    }

    public void SetFOV(float fov)
    {
        camScript.fieldOfView = originalFOV * fov;
    }

    public void Move(Vector3 _originalPos, Vector3 _endPos, float _changeTime = 0f, Action _callback = null)
    {
        StartCoroutine(CoMove(_originalPos, _endPos, _changeTime, _callback));
    }

    private IEnumerator CoMove(Vector3 _originalPos, Vector3 _endPos, float _changeTime = 0f, Action _callback = null)
    {
        transform.localPosition = _originalPos;
        Vector3 pos = _originalPos;

        float elapsedTime = 0f;
        while (elapsedTime < _changeTime)
        {
            elapsedTime += Time.deltaTime;

            pos = Vector3.Lerp(_originalPos, _endPos, elapsedTime / _changeTime);
            transform.localPosition = pos;
            yield return null;
        }

        transform.localPosition = _endPos;
        _callback?.Invoke();
    }

    public void Rotate(Vector3 _originalRot, Vector3 _endRot, float _changeTime = 0f, Action _callback = null)
    {
        StartCoroutine(CoRotate(Quaternion.Euler(_originalRot), 
            Quaternion.Euler(_endRot), _changeTime, _callback));
    }

    private IEnumerator CoRotate(Quaternion _originalRot, Quaternion _endRot, float _changeTime = 0f, Action _callback = null)
    {
        transform.localRotation = _originalRot;
        Quaternion rot = _originalRot;

        float elapsedTime = 0f;
        while (elapsedTime < _changeTime)
        {
            elapsedTime += Time.deltaTime;

            rot = Quaternion.Lerp(_originalRot, _endRot, elapsedTime / _changeTime);
            transform.localRotation = rot;
            yield return null;
        }

        transform.localRotation = _endRot;
        _callback?.Invoke();
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
            elapsedTime += Time.deltaTime;

            currentScale = Mathf.Lerp(_originalScale, _newScale, elapsedTime / _changeTime);
            camScript.fieldOfView = originalFOV * currentScale;
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
        Vector3 baseLocal = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;

            float offsetX = UnityEngine.Random.Range(-1f, 1f) * _magnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * _magnitude;

            transform.localPosition = new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        transform.localPosition = baseLocal;
    }
}
