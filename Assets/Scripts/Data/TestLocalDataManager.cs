using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using LocalData;

public class TestLocalDataManager : MonoBehaviour
{
    void Start()
    {
        string key = LocalDataManager.Instance.SetDataFromFile("RawData/Json/", "sample.json");

        var datas = LocalDataManager.Instance.GetData<sample>(key);
        foreach (var data in datas)
        {
            LogManager.Log(data.LINE_NM);
        }
    }
}
