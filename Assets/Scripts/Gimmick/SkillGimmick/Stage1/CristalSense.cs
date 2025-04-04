using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CristalSense : MonoBehaviour
{
    public UnityEvent OnCristalBreak { get; private set; } = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
            OnCristalBreak?.Invoke();
    }
}
