using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defines
{
    public static class UIDefines 
    {
        public enum UIType
        {
            FullScreen, // 화면을 덮는 UI
            Window, // 화면에 창으로 띄워지는 UI
            ToastPopup, // 이벤트 발생시 나타나는 알림 UI
        }

        public const string UISampleFull = "Assets/Data/UI/Sample/UISampleFull.prefab";
        public const string UISampleWindow = "Assets/Data/UI/Sample/UISampleWindow.prefab";
        public const string UISampleToastPopup = "Assets/Data/UI/Sample/UISampleToastPopup.prefab";
    }
}