using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GimmickStatusVector3 : GimmickStatusTypeBase<Vector3>
{
    [SerializeField] private Text txtName;
    [SerializeField] private InputField inputValueX;
    [SerializeField] private InputField inputValueY;
    [SerializeField] private InputField inputValueZ;

    private Vector3 value;
    
    private void Start()
    {
        inputValueX.onValueChanged.AddListener(OnInputValueXChanged);
        inputValueY.onValueChanged.AddListener(OnInputValueYChanged);
        inputValueZ.onValueChanged.AddListener(OnInputValueZChanged);
        value = Vector3.zero;
    }

    public override void Set(string _name, Vector3 _value, object _targetObject, Action<object, object> _setProperty)
    {
        txtName.text = _name;
        value = _value;
        inputValueX.text = _value.x.ToString();
        inputValueY.text = _value.y.ToString();
        inputValueZ.text = _value.z.ToString();
        targetObject = _targetObject;
        setProperty = _setProperty;
    }

    private void OnInputValueXChanged(string _value)
    {
        float x;
        if (_value.IsNullOrEmpty()) x = 0f;
        else x = float.Parse(_value);
        value.x = x;
        setProperty.Invoke(targetObject, value);
        SetValue(value);
    }
    
    private void OnInputValueYChanged(string _value)
    {
        float y;
        if (_value.IsNullOrEmpty()) y = 0f;
        else y = float.Parse(_value);
        value.y = y;
        setProperty.Invoke(targetObject, value);
        SetValue(value);
    }
    
    private void OnInputValueZChanged(string _value)
    {
        float z;
        if (_value.IsNullOrEmpty()) z = 0f;
        else z = float.Parse(_value);
        value.z = z;
        setProperty.Invoke(targetObject, value);
        SetValue(value);
    }
}
