using System.Collections.Generic;
using System.Xml; // xml 파싱용
using System.IO;  // 파일 읽기용
using UnityEngine;

public class StringManagerSystem
{
    // 데이터를 저장할 Dictionary
    private Dictionary<string, string> stringData = new Dictionary<string, string>();

    // 싱글톤 패턴
    private static StringManagerSystem _instance;

    public static StringManagerSystem Instance
    {
        get
        {
            // 초기화
            if (_instance == null)
            {
                _instance = new StringManagerSystem();
                // plist 파일 경로 확인, 파일 Load
                _instance.LoadStringsFromPlist("Assets/Scripts/Data/StringData/StringData.xml");
            }
            return _instance;
        }
    }

    // plist 파일에서 데이터 읽어오기
    private void LoadStringsFromPlist(string path)
    {
        // 파일 경로 유효성 검사
        if (!File.Exists(path))
        {
            Debug.LogError("Plist file not found at path: " + path);
            return;
        }

        // xml 파일 로드
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(path);

        // <key> 태그 서치 후 리스트화
        XmlNodeList keyNodes = xmlDocument.GetElementsByTagName("key");

        // 현재 파일에 key-value 저장
        foreach (XmlNode keyNode in keyNodes)
        {
            string key = keyNode.InnerText; // key 저장
            XmlNode valueNode = keyNode.NextSibling; // value 저장

            // key-value 쌍으로 Dictionary에 저장
            if (valueNode != null)
            {
                string value = valueNode.InnerText;
                stringData[key] = value;
            }
        }
    }

    // 특정 키로부터 문자열을 가져오는 메소드
    // 프로퍼티로 교체할 예정입니다...
    public string GetString(string key)
    {
        if (stringData.TryGetValue(key, out string value))
        {
            return value;
        }
        else
        {
            Debug.LogWarning("Key not found: " + key);
            return null;
        }
    }
}