using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : ScriptableObject
{
    public float Speed { get => speed; set { speed = value; } }
    [SerializeField]
    private float speed;

    public float Jump { get => speed; set { speed = value; } }
    [SerializeField]
    private float jump;

}
