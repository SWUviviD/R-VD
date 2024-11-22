using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlueCristalSphereSense : MonoBehaviour, IFloorInteractive
{
    public UnityEvent OnPlayerOn { get; private set; } = new UnityEvent();

    public void InteractEnd(GameObject player)
    {
        //player.transform.SetParent(null);
    }

    public void InteractStart(GameObject player)
    {
        OnPlayerOn?.Invoke();
        //player.transform.SetParent(transform, true);
    }
}
