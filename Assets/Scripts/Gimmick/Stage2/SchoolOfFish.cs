using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolOfFish : MonoBehaviour
{
    [SerializeField] private Transform platform1;
    [SerializeField] private Transform platform2;
    [SerializeField] private float offset;

    [SerializeField] private SphereCollider fish;

    private void Start()
    {
        fish.transform.localPosition = (platform1.localPosition + platform2.localPosition) / 2.0f;
        fish.radius = (fish.transform.localPosition - platform1.localPosition).magnitude - offset;
    }
}
