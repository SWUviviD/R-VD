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
        }

        public const string UISampleFull = "Assets/Prefabs/UI/Sample/UISampleFull.prefab";
        public const string UISampleWindow = "Assets/Prefabs/UI/Sample/UISampleWindow.prefab";
    }
}