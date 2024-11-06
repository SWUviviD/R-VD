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

public class CSVToJson : AssetPostprocessor
{
    const string basePath = "Assets/Data/RawData/LocalData";

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
            var inst = Activator.CreateInstance(Type.GetType($"StaticData.{fileName}"));

            // 클래스의 프로퍼티 타입 가져오기
            List<Type> types = new List<Type>();
            foreach (var _ in headers)
            {
                // 해당 프로퍼티 없음
                var propertyInfo = Type.GetType($"StaticData.{fileName}").GetProperty(_);
                if (propertyInfo == null)
                {
                    types.Add(null);
                    continue;
                }
                types.Add(propertyInfo.PropertyType);
            }

            // 타입 기준으로 세팅
            var listType = typeof(List<>);
            var concreteType = listType.MakeGenericType(Type.GetType($"StaticData.{fileName}"));
            //var jsonList = (IList)Activator.CreateInstance(concreteType);
            List<DataBase> list = new List<DataBase>();

            for (int i = 1; i < csvLines.Length; i++)
            {
                var newInstance = Activator.CreateInstance(Type.GetType($"StaticData.{fileName}"));
                var values = csvLines[i].Split(',');
                for (int j = 0; j < types.Count; j++)
                {
                    if (types[j] == null)
                        continue;

                    var converter = TypeDescriptor.GetConverter(types[j]);
                    var value = converter.ConvertFromString(values[j]);

                    Type.GetType($"StaticData.{fileName}").GetProperty(headers[j]).SetValue(newInstance, value);
                }

                //jsonList.Add(newInstance);
                list.Add(newInstance as DataBase);
            }

            //var serializableList = typeof(SerializableList<>).MakeGenericType(Type.GetType($"StaticData.{fileName}"));
            //var serializedJsonList = Activator.CreateInstance(serializableList);
            //serializedJsonList.GetType().GetProperty("list")?.SetValue(serializedJsonList, list);



            var byteArray = MemoryPackSerializer.Serialize(list);
            SerializeManager.Instance.SaveDataFile(fileName, byteArray);

            var data = JsonUtility.ToJson(list, true);
            File.WriteAllText(basePath + "/Json/" + fileName + ".json", data);
        }


    }

    [System.Serializable]
    public partial class SerializableList<T> where T : DataBase, IMemoryPackable<T>
    {
        [MemoryPackInclude]
        [SerializeField]
        private List<T> _list;
        [MemoryPackIgnore] public List<T> list { get => _list; set => _list = value; }
    }
}
