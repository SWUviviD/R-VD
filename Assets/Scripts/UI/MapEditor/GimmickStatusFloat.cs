using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GimmickStatusFloat : GimmickStatusTypeBase<float>
{
    [SerializeField] private Text txtName;
    [SerializeField] private InputField inputValue;

    private void Start()
    {
        inputValue.onValueChanged.AddListener(OnInputValueChanged);
    }

    public override void Set(string _name, float _value, object _targetObject, Action<object, object> _setProperty)
    {
        txtName.text = _name;
        targetObject = _targetObject;
        setProperty = _setProperty;
        inputValue.text = _value.ToString();
    }

    private void OnInputValueChanged(string _value)
    {
        float value;
        if (_value.IsNullOrEmpty()) value = 0;
        else value = float.Parse(_value);
        setProperty.Invoke(targetObject, value);
    }
}
