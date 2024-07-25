using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;
using UnityEngine.UI;

public class UISampleWindow : UIBase
{
    [SerializeField] private Text txtName;
    [SerializeField] private Button btnShowFull;

    private void Start()
    {
        btnShowFull.onClick.AddListener(OnClickFull);
    }

    public override void OnLoad()
    {
        txtName.text = "SampleWindow";
    }

    private void OnClickFull()
    {
        UIManager.Instance.Show(UIDefines.UISampleFull, (_) =>
        {
            (_ as UISampleFull).Set(true);
        });
    }
}
