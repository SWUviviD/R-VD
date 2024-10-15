using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GimmickStatusData
{
    public string GimmickName { get; private set; }
    public GimmickDataBase GimmickDataBase { get; private set; }
    public UnityAction OnReset { get; set; }

    public GimmickStatusData(string _gimmickName, GimmickDataBase _gimmickDataBase, UnityAction _onReset)
    {
        GimmickName = _gimmickName;
        GimmickDataBase = _gimmickDataBase;
        OnReset = _onReset;
    }
}

public class GimmickStatus : MonoBehaviour
{
    /// <summary> 기믹 스테이터스의 제목 </summary>
    [SerializeField] private Text txtTitle;
    /// <summary> 리셋 버튼. 데이터에 맞게 기믹을 변경한다. </summary>
    [SerializeField] private Button btnReset;
    /// <summary> 리셋 버튼의 트랜스폼 </summary>
    [SerializeField] private Transform trResetButton;

    /// <summary> 기믹 스테이터스에서 bool 타입을 표현하기 위한 프리팹 </summary>
    [SerializeField] private GimmickStatusBool prefabBool;
    /// <summary> 기믹 스테이터스에서 int 타입을 표현하기 위한 프리팹 </summary>
    [SerializeField] private GimmickStatusInt prefabInt;
    /// <summary> 기믹 스테이터스에서 float 타입을 표현하기 위한 프리팹 </summary>
    [SerializeField] private GimmickStatusFloat prefabFloat;
    /// <summary> 기믹 스테이터스에서 Vector3 타입을 표현하기 위한 프리팹 </summary>
    [SerializeField] private GimmickStatusVector3 prefabVector3;

    private List<GimmickStatusBool> boolTypeList;
    private List<GimmickStatusInt> intTypeList;
    private List<GimmickStatusFloat> floatTypeList;
    private List<GimmickStatusVector3> vector3TypeList;

    private List<IGimmickStatusTypeBase> currentGimmickStatusList;
    
    private RectTransform rt;
    private GimmickDataBase gimmickData;
    
    private void Start()
    {
        boolTypeList = new List<GimmickStatusBool>();
        intTypeList = new List<GimmickStatusInt>();
        floatTypeList = new List<GimmickStatusFloat>();
        vector3TypeList = new List<GimmickStatusVector3>();

        currentGimmickStatusList = new List<IGimmickStatusTypeBase>();
        
        rt = transform as RectTransform;
    }

    /// <summary>
    /// 기믹 데이터 세팅
    /// </summary>
    public void SetGimmickData(GimmickStatusData _gimmickStatusData)
    {
        gameObject.SetActive(_gimmickStatusData != null);
        
        if (_gimmickStatusData == null) return;
        
        gimmickData = _gimmickStatusData.GimmickDataBase;
        txtTitle.text = _gimmickStatusData.GimmickName;
        btnReset.onClick.RemoveAllListeners();
        btnReset.onClick.AddListener(_gimmickStatusData.OnReset);
        
        Refresh();
    }

    /// <summary>
    /// 현재 타입 리스트를 다시 기존의 리스트로 넣는다.
    /// </summary>
    private void ResetCurrentTypeList()
    {
        foreach (var currentGimmickType in currentGimmickStatusList)
        {
            Type gimmickType = currentGimmickType.GetType();
            
            if (gimmickType == typeof(GimmickStatusBool))
            {
                boolTypeList.Add(currentGimmickType as GimmickStatusBool);
            }
            else if (gimmickType == typeof(GimmickStatusInt))
            {
                intTypeList.Add(currentGimmickType as GimmickStatusInt);
            }
            else if (gimmickType == typeof(GimmickStatusFloat))
            {
                floatTypeList.Add(currentGimmickType as GimmickStatusFloat);
            }
            else if (gimmickType == typeof(GimmickStatusVector3))
            {
                vector3TypeList.Add(currentGimmickType as GimmickStatusVector3);
            }
        }
        
        currentGimmickStatusList.Clear();
    }

    /// <summary>
    /// 기믹 스테이터스를 리프레시한다.
    /// 이 과정에서 기믹 데이터의 프로퍼티값을 가져와서 순서대로 리스트를 만든다.
    /// </summary>
    private void Refresh()
    {
        ResetCurrentTypeList();
        
        boolTypeList.ForEach(_ => _.gameObject.SetActive(false));
        intTypeList.ForEach(_ => _.gameObject.SetActive(false));
        floatTypeList.ForEach(_ => _.gameObject.SetActive(false));
        vector3TypeList.ForEach(_ => _.gameObject.SetActive(false));

        PropertyInfo[] propertyList = gimmickData.GetType().GetProperties();

        int index = 0;
        
        foreach (PropertyInfo property in propertyList)
        {
            if (Attribute.IsDefined(property, typeof(GimmickDataAttribute)) == false) continue;
            
            Type propertyType = property.PropertyType;

            if (propertyType == typeof(bool))
            {
                GimmickStatusBool statusBool;

                if (boolTypeList.IsNullOrEmpty())
                {
                    statusBool = Instantiate(prefabBool, transform);
                }
                else
                {
                    statusBool = boolTypeList[^1];
                    statusBool.gameObject.SetActive(true);
                    boolTypeList.RemoveAt(boolTypeList.Count - 1);
                }
                
                statusBool.Set(property.Name, (bool)property.GetValue(gimmickData), gimmickData, property.SetValue);
                statusBool.transform.SetSiblingIndex(++index);
                currentGimmickStatusList.Add(statusBool);
            }
            else if (propertyType == typeof(int))
            {
                GimmickStatusInt statusInt;
                
                if (intTypeList.IsNullOrEmpty())
                {
                    statusInt = Instantiate(prefabInt, transform);
                }
                else
                {
                    statusInt = intTypeList[^1];
                    statusInt.gameObject.SetActive(true);
                    intTypeList.RemoveAt(intTypeList.Count - 1);
                }

                statusInt.Set(property.Name, (int)property.GetValue(gimmickData), gimmickData, property.SetValue);
                statusInt.transform.SetSiblingIndex(++index);
                currentGimmickStatusList.Add(statusInt);
            }
            else if (propertyType == typeof(float))
            {
                GimmickStatusFloat statusFloat;

                if (floatTypeList.IsNullOrEmpty())
                {
                    statusFloat = Instantiate(prefabFloat, transform);
                }
                else
                {
                    statusFloat = floatTypeList[^1];
                    statusFloat.gameObject.SetActive(true);
                    floatTypeList.RemoveAt(floatTypeList.Count - 1);
                }
                
                statusFloat.Set(property.Name, (float)property.GetValue(gimmickData), gimmickData, property.SetValue);
                statusFloat.transform.SetSiblingIndex(++index);
                currentGimmickStatusList.Add(statusFloat);
            }
            else if (propertyType == typeof(Vector3))
            {
                GimmickStatusVector3 statusVector3;

                if (vector3TypeList.IsNullOrEmpty())
                {
                    statusVector3 = Instantiate(prefabVector3, transform);
                }
                else
                {
                    statusVector3 = vector3TypeList[^1];
                    statusVector3.gameObject.SetActive(true);
                    vector3TypeList.RemoveAt(vector3TypeList.Count - 1);
                }
                
                statusVector3.Set(property.Name, (Vector3)property.GetValue(gimmickData), gimmickData, property.SetValue);
                statusVector3.transform.SetSiblingIndex(++index);
                currentGimmickStatusList.Add(statusVector3);
            }
        }
        
        trResetButton.SetSiblingIndex(++index);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}
