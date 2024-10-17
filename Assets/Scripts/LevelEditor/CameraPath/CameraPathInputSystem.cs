using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPathInputSystem : MonoBehaviour
{
    public Action<Vector3> OnClickScreen { get; set; }

    private const float cameraDistance = 10.0f;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = cameraDistance;
            var position = Camera.main.ScreenToWorldPoint(mousePosition);
            
            OnClickScreen?.Invoke(position);
        }
    }
}
