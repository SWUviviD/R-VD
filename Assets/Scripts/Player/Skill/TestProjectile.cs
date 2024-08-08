using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    private bool isFire;
    private Vector3 firePoint;
    private Vector3 dir;
    private float moveSpeed;
    private float attackRange;

    private void Update()
    {
        if (isFire == false) return;

        transform.position += dir * (moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, firePoint) >= attackRange)
        {
            gameObject.SetActive(false);
            isFire = false;
        }
    }
    
    public void Fire(float attackRange, float moveSpeed, Vector3 dir)
    {
        gameObject.SetActive(true);
        this.dir = dir;
        this.moveSpeed = moveSpeed;
        this.attackRange = attackRange;
        isFire = true;
        firePoint = transform.position;
        transform.forward = dir;
    }
}
