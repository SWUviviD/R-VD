using System.Collections.Generic;
using System.IO;  // 파일 읽기용
using UnityEngine;

public class StringManagerSystem : Singleton<StringManagerSystem>
{
    /// <summary>
    /// 데이터를 저장할 Dictionary
    /// </summary>
    private Dictionary<string, string> stringData = new Dictionary<string, string>();

    public StringManagerSystem()
    {
        // txt 파일 경로 확인, 파일 Load
        LoadStringsFromTxt("Assets/Scripts/Data/StringData/StringData.txt");
    }

    /// <summary>
    ///  txt 파일에서 데이터 읽어오기
    /// </summary>
    private void LoadStringsFromTxt(string path)
    {
        // 파일 경로 유효성 검사
        if (!File.Exists(path))
        {
            LogManager.LogError("Text file not found at path: " + path);
            return;
        }

        // txt 파일 로드
        var lines = File.ReadAllLines(path);

        // 각 줄을 key-value 쌍으로 파싱하여 Dictionary에 저장
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue; // 빈 줄 무시

            // ;로 끝나는 라인을 처리하여 분리
            var tempLine = line.TrimEnd(';'); // 마지막 ; 제거
            var parts = tempLine.Split(new[] { "\":\"" }, System.StringSplitOptions.None); // key와 value를 ":"로 분리

            if (parts.Length != 2)
            {
                LogManager.LogWarning("Invalid line format: " + line);
                continue;
            }

            // key와 value에서 양쪽 " 제거
            string key = parts[0].Trim('\"').Trim();    // key 부분
            string value = parts[1].Trim('\"').Trim();  // value 부분

            // key-value 쌍으로 Dictionary에 저장
            if (!stringData.ContainsKey(key))
            {
                stringData[key] = value;
            }
            else
            {
                LogManager.LogWarning("Duplicate key found: " + key);
            }
        }
    }

    /// <summary>
    ///  프로퍼티 (인덱서)
    /// </summary>
    public string this[string key]
    {
        get
        {
            if (stringData.TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                LogManager.LogWarning("Key not found: " + key);
                return null;
            }
        }
    }
}
