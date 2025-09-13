using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LocalData;
using MemoryPack;
using UnityEditor;


public class SerializeManager : Singleton<SerializeManager>
{
    private static readonly string folder_path = "Data/RawData/LocalData/Bytes";

    public byte[] Serialize<T>(List<T> obj) where T : DataBase
    {
        return MemoryPackSerializer.Serialize(obj);
    }

    public T Deserialize<T>(byte[] data)
    {
        return MemoryPackSerializer.Deserialize<T>(data);
    }

    // 파일 저장 함수
    public void SaveDataFile(string fileName, byte[] data)
    {
        #if UNITY_EDITOR
        string file = string.Empty;
        file = string.Format("Assets/Resources/{0}/{1}.bytes", folder_path, fileName);

        if (File.Exists(file))
        {
            File.Delete(file);
        }

        FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
        #else
        if (!Directory.Exists(folder_path)) Directory.CreateDirectory(folder_path);
        string path = Path.Combine(folder_path, $"{fileName}.bytes");
        File.WriteAllBytes(path, data);
        #endif
    }

    // 파일 불러오기 함수
    public bool LoadDataFile<T>(out List<T> list, string fileName)
    {
        try
        {
            TextAsset ta = AssetLoadManager.Instance.SyncLoadObject<TextAsset>(Path.Combine(folder_path, $"{fileName}"), fileName);
            list = MemoryPackSerializer.Deserialize<List<T>>(ta.bytes);
            return true;
        }
        catch(System.Exception e)
        {
            list = null;
            return false;
        }
    }

    public bool IsFileExist(string _fileName)
    {
        DirectoryInfo rootDirectory = new DirectoryInfo(Path.Combine(folder_path, _fileName));
        return rootDirectory.Exists;
    }
}
