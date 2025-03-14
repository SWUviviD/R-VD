 using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Defines;
using UnityEditor;
using UnityEngine.Assertions;
using System.ComponentModel;
using MemoryPack;
using LocalData;
using StaticData;

public class CSVToJson
#if UNITY_EDITOR
    : AssetPostprocessor
#endif
{
#if UNITY_EDITOR
    const string basePath = "Assets/Resources/Data/RawData/LocalData";

    void OnPreprocessAsset()
    {
        // 인포트 되는 대상이 csv 파일임
        if (this.assetPath.Split('.').Length >= 2 && assetPath.Split('.')[1].Equals("csv"))
        {
            // 헤더 및 내용 검출
            var csvLines = File.ReadAllLines(assetPath);
            if (csvLines.Length == 0)
            {
                Assert.IsFalse(false, "CSV file is empty.");
                return;
            }
            
            var headers = csvLines[0].Split(',');

            // 파일 이름을 통해 해당 클래스 인스턴스 생성
            string fileName = (assetPath.Split('/')[^1]).Split('.')[0];
            Type t = null;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var s in a.GetTypes())
                {
                    if (s.Name == fileName)
                    {
                        t = a.GetType(s.FullName);
                    }
                }
            }
            var inst = Activator.CreateInstance(t);

            // 클래스의 프로퍼티 타입 가져오기
            List<Type> types = new List<Type>();
            foreach (var _ in headers)
            {
                // 해당 프로퍼티 없음
                var propertyInfo = t.GetProperty(_);
                if (propertyInfo == null)
                {
                    types.Add(null);
                    continue;
                }
                types.Add(propertyInfo.PropertyType);
            }

            // 타입 기준으로 세팅
            var listType = typeof(List<>);
            var concreteType = listType.MakeGenericType(t);
            //var jsonList = (IList)Activator.CreateInstance(concreteType);
            List<DataBase> list = new List<DataBase>();

            for (int i = 1; i < csvLines.Length; i++)
            {
                var newInstance = Activator.CreateInstance(t);
                var values = csvLines[i].Split(',');
                for (int j = 0; j < types.Count; j++)
                {
                    if (types[j] == null)
                        continue;

                    var converter = TypeDescriptor.GetConverter(types[j]);
                    var value = converter.ConvertFromString(values[j]);

                    t.GetProperty(headers[j]).SetValue(newInstance, value);
                }

                //jsonList.Add(newInstance);
                list.Add(newInstance as DataBase);
            }

            //var serializableList = typeof(SerializableList<>).MakeGenericType(Type.GetType($"StaticData.{fileName}"));
            //var serializedJsonList = Activator.CreateInstance(serializableList);
            //serializedJsonList.GetType().GetProperty("list")?.SetValue(serializedJsonList, list);

            var byteArray = MemoryPackSerializer.Serialize(list);
            if (fileName == "LDPinMapData")
            {
                List<LDPinMapData> pin = new List<LDPinMapData>();
                foreach (var d in list)
                {
                    pin.Add(d as LDPinMapData);
                }

                byteArray = MemoryPackSerializer.Serialize(pin);
            }
            else if(fileName == "CutSceneInfo")
            {
                List<CutSceneInfo> cut = new List<CutSceneInfo>();
                foreach(var c in list)
                {
                    cut.Add(c as CutSceneInfo);
                }

                byteArray = MemoryPackSerializer.Serialize(cut);
            }
            else if(fileName == "DialogInfo")
            {
                List<DialogInfo> cut = new List<DialogInfo>();
                foreach (var c in list)
                {
                    cut.Add(c as DialogInfo);
                }

                byteArray = MemoryPackSerializer.Serialize(cut);

            }

            SerializeManager.Instance.SaveDataFile(fileName, byteArray);

            var data = JsonUtility.ToJson(list, true);
            File.WriteAllText(basePath + "/Json/" + fileName + ".json", data);

            Debug.Log("CSV File Change Complete");
        }


    }
#endif

    [System.Serializable]
    public partial class SerializableList<T> where T : DataBase, IMemoryPackable<T>
    {
        [MemoryPackInclude]
        [SerializeField]
        private List<T> _list;
        [MemoryPackIgnore] public List<T> list { get => _list; set => _list = value; }
    }
}