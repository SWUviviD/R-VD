using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HowToUIKeyPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI functionText;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private string function;
    [SerializeField] private string key;

    private void Awake()
    {
        functionText.text = function;
        keyText.text = key;
    }
}
