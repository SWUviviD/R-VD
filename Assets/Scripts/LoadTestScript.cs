using LevelEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTestScript : MonoBehaviour
{
    // 실험을 위한 임시 추가
    [SerializeField] PlacementSystem placementSystem;

    private void Start()
    {
        var mapData = StageManager.Instance.LoadStage("asdf");
        
        foreach (var blinkBoard in mapData.BlinkBoardDataList)
        {
            GameObject prefab = (GameObject)AddressableAssetsManager.Instance.SyncLoadObject(blinkBoard.Address, blinkBoard.Address);
            var instance = Instantiate(prefab).GetComponent<BlinkBoardGimmick>();
            instance.GimmickData.Set(blinkBoard);

            // CreateGimmick으로 생성되는 기믹 확인을 위해 instance는 잠시 꺼두기
            instance.gameObject.SetActive(false);

            // 오브젝트 생성 (하지만, 기믹 데이터가 열리고 수정되는건 CreateGimmick으로 생성된 오브젝트가 아닌, 위에 생성된 instance에 연결되어 있음)
            placementSystem.CreateGimmick(blinkBoard.Address, blinkBoard.Position, blinkBoard.Rotation, blinkBoard.Scale, instance.GimmickData);


            // Test-1 : GimmickDataBase가 Monobehaviour를 상속받아 data가 생성이 안됨
            //var data = new GimmickDataBase();
            //data.Set(blinkBoard);
            //placementSystem.CreateGimmick(blinkBoard.Address, blinkBoard.Position, blinkBoard.Rotation, blinkBoard.Scale, data);


            // Test-2 : 검색 결과, Test-1이 안될 경우 AddComponent 하는 방법이 있음
            // 하지만, GimmickBase가 Awake() 함수로 GimmickDataBase의 Init()를 호출하고, Init() 함수에는 SerializeField를 참조하여 DictPoint를 초기화 함
            //var data2 = gameObject.AddComponent<BlinkBoardData>();
            //data2.Set(blinkBoard);
            //placementSystem.CreateGimmick(blinkBoard.Address, blinkBoard.Position, blinkBoard.Rotation, blinkBoard.Scale, data2);


            // Test-3 : Test-1과 마찬가지로 GimmickDataBase가 Monobehaviour를 상속받아 data가 생성이 안됨
            //var data3 = new BlinkBoardGimmick();
            //data3.GimmickData.Set(blinkBoard);
            //placementSystem.CreateGimmick(blinkBoard.Address, blinkBoard.Position, blinkBoard.Rotation, blinkBoard.Scale, data.GimmickData);


            // 만약 placementSystem.CreateGimmick() 함수 내부에서 Set() 함수를 사용하지 않으려면 CreateGimmick()이 GameObject를 반환해서
            // GetComponent를 통해 Set() 함수로 접근하는 방법이 있는 것으로 판단됨
            // 예시)
            //var go = placementSystem.CreateGimmick(blinkBoard.Address, blinkBoard.Position, blinkBoard.Rotation, blinkBoard.Scale, data);
            //go.GetComponent<BlinkBoardGimmick>().GimmickData.Set(blinkBoard);
        }
    }
}
