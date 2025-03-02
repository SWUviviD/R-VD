using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBlock : GimmickBase<WaterBlockData>
{
    [Header("References")]
    [SerializeField] public Transform player; // 플레이어 Transform
    [SerializeField] public WaterVaseControll vase;
    [SerializeField] public Water_Door door;
    [SerializeField] private float interactionDistance = 2f; // 상호작용 거리
    [SerializeField] private WaterBlockData waterBlockData;

    public bool isClear = false;

    public int remainingUsage; // 남은 물 사용 가능 횟수
    private Renderer blockRenderer;

    private bool ignoreInput = false;
    private float ignoretime = 3.0f;
    private KeyCode lastReleasedKey;

    protected override void Init()
    {
        // 초기화
        blockRenderer = GetComponent<MeshRenderer>();
        remainingUsage = waterBlockData.WaterUsage;
        UpdateBlockMaterial();
    }

    public override void SetGimmick()
    {
        
    }

    protected override string GetAddress()
    {
        return "Data/Prefabs/Gimmick/WaterBlock";
    }

    void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (Input.GetKeyUp(KeyCode.Q)) HandleKeyRelease(KeyCode.Q, KeyCode.W, KeyCode.E);
        if (Input.GetKeyUp(KeyCode.W)) HandleKeyRelease(KeyCode.W, KeyCode.Q, KeyCode.E);
        if (Input.GetKeyUp(KeyCode.E)) HandleKeyRelease(KeyCode.E, KeyCode.Q, KeyCode.W);

        if (!ignoreInput)
        {
            if (Input.GetKeyDown(KeyCode.Q));
            if (Input.GetKeyDown(KeyCode.W));
            if (Input.GetKeyDown(KeyCode.E))
                {
                // 거리 내에서 E 키 입력으로 상호작용
                if (distanceToPlayer <= interactionDistance && Input.GetKeyDown(KeyCode.E))
                {
                    InteractWithBlock();
                }
                };
        }

        if (remainingUsage >= 3)
        {
            isClear = true;
            door.OpenDoor();
        }
    }

    void HandleKeyRelease(KeyCode releasedKey, KeyCode blockKey1, KeyCode blockKey2)
    {
        lastReleasedKey = releasedKey;
        StartCoroutine(BlockInputForDuration(blockKey1, blockKey2, ignoretime));
    }

    IEnumerator BlockInputForDuration(KeyCode blockKey1, KeyCode blockKey2, float duration)
    {
        ignoreInput = true;
        yield return new WaitForSeconds(duration);
        ignoreInput = false;
    }

    void InteractWithBlock()
    {
        // 현재 상태에 따라 동작
        if (remainingUsage < waterBlockData.maxWaterCapacity && vase.waterLevelOne == true)
        {
            AddWater(); // 물 담기
            vase.removeWater();
        }
        else if (remainingUsage > 0 && vase.waterLevelTwo == false)
        {
            ScoupWater(); // 물 뜨기
            vase.addWater();
        }
        else
        {
            LogManager.Log("더 이상 물을 담거나 사용할 수 없습니다.");
        }
    }

    void AddWater()
    {
        if (remainingUsage < waterBlockData.maxWaterCapacity && vase.waterLevelOne == true) 
        {
            remainingUsage++;
            LogManager.Log("현재 물의 양: " + remainingUsage);
            if (vase.waterLevelTwo == true)
            {
                vase.waterLevelTwo = false;
            }
            else
            {
                vase.waterLevelOne = false;
            }
            UpdateBlockMaterial();
        }
        else
        {
            LogManager.Log("블록이 이미 가득 찼습니다.");
        }
    }

    void ScoupWater()
    {
        if (remainingUsage > 0 && vase.waterLevelTwo == false)
        {
            remainingUsage--;
            LogManager.Log("현재 물의 양: " + remainingUsage);
            UpdateBlockMaterial();
        }
        else if (remainingUsage == 0)
        {
            LogManager.Log("블록 빔");
        }
    }

    void UpdateBlockMaterial()
    {
        // 상태에 따라 Material 변경
        if (waterBlockData.blockMaterials.Length > remainingUsage)
        {
            blockRenderer.material = waterBlockData.blockMaterials[remainingUsage];
        }
        else
        {
            LogManager.LogWarning("블록 Material 배열 부족");
        }
    }
}
