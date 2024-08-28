using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringManageSys : MonoBehaviour
{
    // 관리&저장용 딕셔너리 선언
    private static Dictionary<string, string> UIDictionary;

    // 딕셔너리의 프로퍼티 선언
    // 사용시 StringManageSys.StringData["StringData.txt"]
    public static Dictionary<string, string> UI
    {
        get => UIDictionary;
        set
        {
            UIDictionary = value;
            // local 데이터 연동
            // data.UI = value;
        }
    }

    // 딕셔너리 초기화
    static StringManageSys()
    {
        // UI 딕셔너리
        UI = new Dictionary<string, string>
        {
            { "UI.Title", "게임 제목" },
            { "UI.MainText", "메인 텍스트" }
        };
    }
}
