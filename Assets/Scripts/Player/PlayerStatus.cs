using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [field: SerializeField]
    public float MoveSpeed { get; private set; }
    [field: SerializeField]
    public float AdditionalMoveSpeed { get; private set; }
    
    [field: SerializeField]
    public float AttackRange { get; private set; }
    [field: SerializeField]
    public float ProjectileSpeed { get; private set; }
    
    public float GetMoveSpeed()
    {
        return MoveSpeed + AdditionalMoveSpeed;
    }

    public void SetMoveDelay(bool set)
    {
        AdditionalMoveSpeed = set ? -MoveSpeed * 0.75f : 0f;
    }
    
    public void SetMoveStop(bool set)
    {
        AdditionalMoveSpeed = set ? -MoveSpeed : 0f;
    }
}