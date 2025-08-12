using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingWing : MonoBehaviour
{
    [SerializeField] private float _speed;

    public void Update()
    {
        transform.Rotate(Vector3.right * _speed * Time.deltaTime, Space.Self);
    }
}
