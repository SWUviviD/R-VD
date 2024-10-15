using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;
using UnityEngine.UI;

public class PlacementMode : MonoBehaviour
{
    [SerializeField] private Dropdown dropdownMode;
    [SerializeField] private List<GameObject> gimmickModeObjects;
    [SerializeField] private List<GameObject> cameraPathModeObjects;

    private GimmickDefines.PlacementModeType currentMode;
    
    public Action<GimmickDefines.PlacementModeType> OnModeChanged { get; set; }
    
    private void Start()
    {
        dropdownMode.onValueChanged.AddListener(OnValueChanged);
        OnValueChanged(dropdownMode.value);
    }

    private void OnValueChanged(int _value)
    {
        currentMode = (GimmickDefines.PlacementModeType)_value;
        OnModeChanged?.Invoke(currentMode);
        gimmickModeObjects.ForEach(_ => _.SetActive(currentMode == GimmickDefines.PlacementModeType.Gimmick));
        cameraPathModeObjects.ForEach(_ => _.SetActive(currentMode == GimmickDefines.PlacementModeType.CameraPath));
    }
}
