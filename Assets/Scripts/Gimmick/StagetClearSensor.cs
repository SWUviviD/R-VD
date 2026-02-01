using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagetClearSensor : MonoBehaviour
{
    [SerializeField] private StageClearPoint parent;

    private void Start()
    {
        if(parent == null) gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerHp>(out var hp) == true)
        {
            parent.OnGameClear();
        }
    }
}
