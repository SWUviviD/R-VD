using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform trNeck;
    [Range(0.0f, 60.0f)]
    [SerializeField] private float rotateLimit;

    private void LateUpdate()
    {
        if (GameManager.Instance.Player == null) return;

        Vector3 toPlayer = (GameManager.Instance.Player.transform.position - transform.position).normalized;
        trNeck.forward = Vector3.RotateTowards(transform.forward, toPlayer, rotateLimit * Mathf.Deg2Rad, 0.0f);
    }
}
