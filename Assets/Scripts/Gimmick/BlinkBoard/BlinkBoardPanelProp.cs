using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

/// <summary>
/// BlinkBoard 기믹의 반짝이는 판
/// </summary>
public class BlinkBoardPanelProp : MonoBehaviour
{
    /// <summary> 실제로 꺼졌다가 켜졌다 해야하는 판 오브젝트 </summary>
    [SerializeField] private GameObject objPanel;

    /// <summary> 활성화 상태가 유지되는 시간 </summary>
    private float enableDurationTime;
    /// <summary> 비활성화 상태가 유지되는 시간 </summary>
    private float disableDurationTime;
    
    /// <summary> 활성화 되어있는지 여부 </summary>
    private bool isEnable => objPanel.activeSelf;

    /// <summary> 그동안 흐른 시간 </summary>
    private float elapsedTime;

    /// <summary> 반짝임이 시작되었는지 여부 </summary>
    private bool isStartBlink;
    
    /// <summary>
    /// 초기화한다.
    /// </summary>
    public void Init(float _enableDurationTime, float _disableDurationTime)
    {
        enableDurationTime = _enableDurationTime;
        disableDurationTime = _disableDurationTime;
        elapsedTime = 0.0f;
        objPanel.SetActive(true);
        isStartBlink = false;
    }

    /// <summary>
    /// 깜빡임을 시작한다.
    /// </summary>
    public void StartBlink()
    {
        isStartBlink = true;
    }

    private void Update()
    {
        if (isStartBlink == false) return;

        elapsedTime += Time.deltaTime;

        if (isEnable && elapsedTime >= enableDurationTime)
        {
            objPanel.SetActive(!isEnable);
            elapsedTime -= enableDurationTime;
        } 
        else if (isEnable == false && elapsedTime >= disableDurationTime)
        {
            objPanel.SetActive(!isEnable);
            elapsedTime -= disableDurationTime;
        }
    }
}
