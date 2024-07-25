using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClose : MonoBehaviour
{
    private void Start()
    {
        var button = GetComponent<Button>();
        var parent = GetComponentInParent<UIBase>();
        button.onClick.AddListener(parent.Close);
    }
}
