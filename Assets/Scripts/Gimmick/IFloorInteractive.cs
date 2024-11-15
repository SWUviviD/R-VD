using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFloorInteractive
{
    public void InteractStart(GameObject player);
    public void InteractEnd(GameObject player);
}
