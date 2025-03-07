using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GimmickStatusBool : GimmickStatusTypeBase<bool>
{
    [SerializeField] private Text txtName;
    [SerializeField] private Toggle toggleValue;

    private void Start()
    {
        toggleValue.onValueChanged.AddListener(OnInputValueChanged);
    }
    
    public override void Set(string _name, bool _value, object _targetObject, Action<object, object> _setProperty)
    {
        txtName.text = _name;
        targetObject = _targetObject;
        setProperty = _setProperty;
        toggleValue.isOn = _value;
    }

    private void OnInputValueChanged(bool _value)
    {
        setProperty.Invoke(targetObject, _value);
    }
}
