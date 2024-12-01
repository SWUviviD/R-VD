using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSence : MonoBehaviour
{
    [SerializeField] private FallingStar starScript;

    private void OnTriggerEnter(Collider other)
    {
        starScript.OnPlayerSenced(other);
    }
}
