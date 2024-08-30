using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using StaticData;
using System.Diagnostics;

public class TestLocalDataManager : MonoBehaviour
{
    void Start()
    {
        object obj = AddressableAssetsManager.Instance.SyncLoadObject("Assets/Prefabs/Data/Json/sample.json", "sample");
        TextAsset list = obj as TextAsset;

        var data = JsonUtility.FromJson<CSVToJson.SerializableList<StaticData.sample>>(list.text);
        foreach(var item in data.list)
        {
            LocalDataManager.Instance.AddData("sample", item);
        }

        var insertedData = LocalDataManager.Instance.GetData<sample>("sample");
        foreach(var item in insertedData)
        {
            UnityEngine.Debug.Log(item.LINE_NM);
        }
    }
}
