using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkBoardGimmick : GimmickBase<BlinkBoardData>
{
    /// <summary>
    /// 빙판 프리팹
    /// </summary>
    [SerializeField] private BlinkBoardPanelProp prefabPanelProp;

    private const string StartPointName = "StartPoint";
    private const string EndPointName = "EndPoint";

    private List<BlinkBoardPanelProp> panelList;
    
    private Vector3 StartPosition => gimmickData.DictPoint[StartPointName].position;
    private Vector3 EndPosition => gimmickData.DictPoint[EndPointName].position;

    /// <summary> 깜빡임이 시작되었는지 여부 </summary>
    private bool isStartBlink;

    /// <summary> 다음의 빙판의 깜빡임 대기의 누적시간 </summary>
    private float elapsedTime;

    /// <summary> 깜빡임이 시작되어야하는 빙판의 인덱스 </summary>
    private int currentPanelIndex;

    protected override void Init()
    {
        panelList = new List<BlinkBoardPanelProp>();
    }

    public override void SetGimmick()
    {
        if (panelList.Count < gimmickData.BoardCount)
        {
            while (panelList.Count < gimmickData.BoardCount)
            {
                panelList.Add(Instantiate(prefabPanelProp, transform));
            }
        }
        
        panelList.ForEach(_ => _.gameObject.SetActive(false));

        var distance = (StartPosition - EndPosition).magnitude;
        var dir = (EndPosition - StartPosition).normalized;
        var gap = distance / (gimmickData.BoardCount - 1); 
        
        for (int i = 0; i < gimmickData.BoardCount; ++i)
        {
            var board = panelList[i];
            var scale = board.transform.localScale;
            scale.x = gimmickData.BoardSize;
            scale.z = gimmickData.BoardSize;
            board.transform.localScale = scale;
            board.transform.position = StartPosition + gap * i * dir;
            board.gameObject.SetActive(true);
            board.Init(gimmickData.DurationTime, gimmickData.BlinkTime);
        }

        elapsedTime = 0f;
        currentPanelIndex = 0;
        isStartBlink = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetGimmick();
        }
        
        if (isStartBlink == false) return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= gimmickData.NextBlinkTime)
        {
            elapsedTime -= gimmickData.NextBlinkTime;
            panelList[currentPanelIndex].StartBlink();
            ++currentPanelIndex;
            // 모든 빙판의 깜빡임이 끝나면 종료한다.
            if(currentPanelIndex >= gimmickData.BoardCount)
            {
                isStartBlink = false;
            }
        }
    }
}
