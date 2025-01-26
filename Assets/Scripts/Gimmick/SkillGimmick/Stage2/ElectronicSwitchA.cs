using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicSwitchA : ElectronicSwitch, IFusionable
{
    public bool Activate(Transform player)
    {
        OnShocked(null);
        return true;
    }
}
