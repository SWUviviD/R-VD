#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class PlacementToolView : MonoBehaviour
    {
        [SerializeField] private ObjectPlacer objectPlacer;

        /// <summary> 설치된 오브젝트 영역 표시 토글 </summary>
        [SerializeField] private Button showAreaToggle;

        /// <summary> 토글 체크 마크 이미지 </summary>
        [SerializeField] private Image imgCheckMark;

        private bool isShowed;

        private void Start()
        {
            isShowed = false;

            UIHelper.OnClick(showAreaToggle, ShowAreaToggle);
        }

        /// <summary>
        /// 설치된 오브젝트 영역 표시 토글
        /// </summary>
        private void ShowAreaToggle()
        {
            isShowed = !isShowed;
            imgCheckMark.enabled = isShowed;
            objectPlacer.ShowPlacedAreas(isShowed);
        }
    }
}

#endif
