using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LocalData;
using MemoryPack;
using UnityEditor;


public class SerializeManager : Singleton<SerializeManager>
{
    private const string folder_path = "Assets/Data/RawData/LocalData/Bytes";
    private static readonly string folder_absoute_path = Path.Combine(Application.dataPath, "Data/RawData/LocalData/Bytes");

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
        string file = string.Format("{0}/{1}.data", folder_path, fileName);

        if (File.Exists(file))
        {
            File.Delete(file);
        }

        FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    // 파일 불러오기 함수
    public bool LoadDataFile<T>(out List<T> list, string fileName)
    {
        try
        {
            FileStream fileStream = new FileStream(string.Format("{0}/{1}.data", folder_path, fileName), FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();
            list = MemoryPackSerializer.Deserialize<List<T>>(data);
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
        DirectoryInfo rootDirectory = new DirectoryInfo(Path.Combine(folder_absoute_path, _fileName));
        return rootDirectory.Exists;
    }
}
