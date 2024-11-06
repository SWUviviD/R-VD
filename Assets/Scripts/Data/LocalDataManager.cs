using LocalData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDataManager : Singleton<LocalDataManager>
{
    private Dictionary<string, List<DataBase>> datas;
    private const string ASSET_PATH = "Assets/Data/";

    public LocalDataManager()
    {
        datas = new Dictionary<string, List<DataBase>> ();
    }

    public string SetDataFromFile(string _folderPath, string _fileName)
    {
        string key = string.Empty;
        try
        {
            object obj = AddressableAssetsManager.Instance.SyncLoadObject(ASSET_PATH + _folderPath + "/" + _fileName, _fileName);
            TextAsset list = obj as TextAsset;

            var data = JsonUtility.FromJson<CSVToJson.SerializableList<sample>>(list.text);
            foreach (var item in data.list)
            {
                AddData(_fileName, item);
            }
        }
        catch (Exception ex)
        {
            LogManager.LogError("SetDataFromFile error: " + ex.Message);
        }

        return _fileName;
    }

    public void AddData(string key, DataBase data)
    {
        if(datas.ContainsKey(key) == false)
        {
            datas[key] = new List<DataBase> ();
        }
        datas[key].Add(data);
    }

    public T GetData<T>(string _key, int _id) where T : DataBase
    {
        List<T> list = GetData<T>(_key);
        return list?.Find(_ => _.ID == _id);
    }
    public T GetData<T>(string _key, string _id) where T : DataBase
    {
        List<T> list = GetData<T>(_key);
        return list?.Find(_ => _.ID_str == _id);
    }

    public List<T> GetData<T>(string _key) where T : DataBase
    {
        List<DataBase> list;
        if (datas.TryGetValue(_key, out list) == true)
        {
            List<T> returnList = new List<T>();
            foreach (DataBase data in list)
            {
                returnList.Add(data as T);
            }

            return returnList;
        }

        return null;
    }
}
