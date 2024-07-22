using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using Defines.ParsingDefines;
using UnityEditor;
using static UnityEditor.LightingExplorerTableColumn;
using System.Xml.Serialization;
using UnityEngine.Assertions;
using System.ComponentModel;

public class CSVToJson : AssetPostprocessor
{
    const string basePath = "Assets/Resource/Data/";

    void OnPreprocessAsset()
    {
        // 인포트 되는 대상이 csv 파일임
        if (this.assetPath.Split('.')[1].Equals("csv"))
        {
            var csvLines = File.ReadAllLines(assetPath);
            if (csvLines.Length == 0)
            {
                Assert.IsFalse(false, "CSV file is empty.");
                return;
            }
            
            var headers = csvLines[0].Split(',');

            string fileName = (assetPath.Split('/')[^1]).Split('.')[0];
            var inst = Activator.CreateInstance(Type.GetType($"StaticData.{fileName}"));

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

            var listType = typeof(List<>);
            var concreteType = listType.MakeGenericType(Type.GetType($"StaticData.{fileName}"));
            var jsonList = (IList)Activator.CreateInstance(concreteType);

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

                jsonList.Add(newInstance);
                
            }

            var serializableList = typeof(SerializableList<>).MakeGenericType(Type.GetType($"StaticData.{fileName}"));
            var serializedJsonList = Activator.CreateInstance(serializableList);
            serializedJsonList.GetType().GetProperty("list")?.SetValue(serializedJsonList, jsonList);

            var data = JsonUtility.ToJson(serializedJsonList, true);
            File.WriteAllText(basePath + fileName + ".json", data);
        }


    }

    [System.Serializable]
    public class SerializableList<T>
    {
        [SerializeField]
        private List<T> _list;
        public List<T> list { get => _list; set => _list = value; }
    }
}
