using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GimmickStatusInt : GimmickStatusTypeBase<int>
{
    [SerializeField] private Text txtName;
    [SerializeField] private InputField inputValue;

    private void Start()
    {
        inputValue.onValueChanged.AddListener(OnInputValueChanged);
    }
    
    public override void Set(string _name, int _value, object _targetObject, Action<object, object> _setProperty)
    {
        txtName.text = _name;
        inputValue.text = _value.ToString();
        targetObject = _targetObject;
        setProperty = _setProperty;
    }

    private void OnInputValueChanged(string _value)
    {
        int value;
        if (_value.IsNullOrEmpty()) value = 0;
        else value = int.Parse(_value);
        setProperty.Invoke(targetObject, value);
        SetValue(value);
    }
}
