#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class PlacementToolView : MonoBehaviour
    {
        [Header("Compnents")]
        [SerializeField] private PlacementSystem placementSystem;
        [SerializeField] private ObjectPlacer objectPlacer;

        [Header("Show Area Toggle")]
        /// <summary> 체크포인트 모드 토글 </summary>
        [SerializeField] private Button checkpointModToggle;
        /// <summary> 체크포인트 모드 체크 마크 이미지 </summary>
        [SerializeField] private Image imgCheckpointModMark;

        [Header("Grid Mod Toggle")]
        /// <summary> 그리드 모드 토글 </summary>
        [SerializeField] private Button gridModToggle;
        /// <summary> 그리드 모드 체크 마크 이미지 </summary>
        [SerializeField] private Image imgGridModCheckMark;
        /// <summary> 그리드 크기 입력란 </summary>
        [SerializeField] private InputField inputGridSize;

        [Header("Show Area Toggle")]
        /// <summary> 설치된 오브젝트 영역 표시 토글 </summary>
        [SerializeField] private Button showAreaToggle;
        /// <summary> 설치된 오브젝트 영역 표시 체크 마크 이미지 </summary>
        [SerializeField] private Image imgshowAreaCheckMark;

        private bool isShowed;
        private bool isGridMod;
        private bool isCheckpointMod;

        private void Start()
        {
            isGridMod = false;
            isShowed = false;
            GridModToggle();

            UIHelper.OnClick(checkpointModToggle, CheckpointModToggle);

            UIHelper.OnClick(gridModToggle, GridModToggle);
            inputGridSize.onEndEdit.AddListener(GridSizeChanged);

            UIHelper.OnClick(showAreaToggle, ShowAreaToggle);
        }

        /// <summary>
        /// 체크포인트 모드 토글
        /// </summary>
        private void CheckpointModToggle()
        {
            isCheckpointMod = !isCheckpointMod;
            imgCheckpointModMark.enabled = isCheckpointMod;
            placementSystem.PlacedCollidersActiveToggle(isCheckpointMod);
        }

        /// <summary>
        /// 그리드 모드 토글
        /// </summary>
        private void GridModToggle()
        {
            isGridMod = !isGridMod;
            imgGridModCheckMark.enabled = isGridMod;
            placementSystem.GridModToggle(float.Parse(inputGridSize.text), isGridMod);
            
        }

        /// <summary>
        /// 그리드 모드 사이즈 설정
        /// </summary>
        /// <param name="text"></param>
        private void GridSizeChanged(string text)
        {
            placementSystem.GridModToggle(float.Parse(text), isGridMod);
        }

        /// <summary>
        /// 설치된 오브젝트 영역 표시 토글
        /// </summary>
        private void ShowAreaToggle()
        {
            isShowed = !isShowed;
            imgshowAreaCheckMark.enabled = isShowed;
            objectPlacer.ShowPlacedAreas(isShowed);
        }
    }
}

#endif
