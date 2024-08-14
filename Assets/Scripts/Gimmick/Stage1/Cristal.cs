using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cristal : MonoBehaviour
{
    [SerializeField] GameObject cristal;
    [SerializeField] GameObject icyRoad;

    private void Start()
    {
        cristal.SetActive(true);
        icyRoad.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 화살이 지나갔을 경우
        if(other.transform.parent.TryGetComponent<TestProjectile>(out var _) == true)
        {
            cristal.SetActive(false);
            icyRoad.SetActive(true);
        }
    }
}
