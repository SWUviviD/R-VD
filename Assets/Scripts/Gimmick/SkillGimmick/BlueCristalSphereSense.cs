using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlueCristalSphereSense : MonoBehaviour
{
    public UnityEvent OnPlayerOn { get; private set; } = new UnityEvent();

    private void OnCollisionEnter(Collision collision)
    {
        OnPlayerOn?.Invoke();
    }
}
