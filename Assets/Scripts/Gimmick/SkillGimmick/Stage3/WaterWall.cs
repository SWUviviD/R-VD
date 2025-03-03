using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWall : GimmickBase<WaterWallData>
{
    [Header("References")]
    [SerializeField] public Transform player; // 플레이어 Transform
    [SerializeField] public WaterVaseControll vase;
    [SerializeField] private WaterWallData waterwallData;
    [SerializeField] private float interactionDistance = 2f; // 상호작용 거리

    public bool isice;
    public bool isbreak;

    private Renderer wallRenderer;

    protected override void Init()
    {
        // 초기화
        wallRenderer = GetComponent<MeshRenderer>();
        wallRenderer.material = waterwallData.blockMaterials[0];

        isice = false;
        isbreak = false;
    }

    public override void SetGimmick()
    {

    }

    protected override string GetAddress()
    {
        return "Data/Prefabs/Gimmick/WaterWall";
    }

    void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 거리 내에서 E 키 입력으로 상호작용
        if (distanceToPlayer <= interactionDistance && Input.GetKeyDown(KeyCode.E) && vase.waterLevelOne == true)
        {
            isice = true;
            UpdateBlockMaterial();
        }

        // 거리 내에서 Q, W 키 입력으로 상호작용
        if (distanceToPlayer <= interactionDistance && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W)) 
            && isice)
        {
            isbreak = true;
            UpdateBlockMaterial();
        }
    }

    void UpdateBlockMaterial()
    {
        // 상태에 따라 Material 변경
        if (isice)
        {
            wallRenderer.material = waterwallData.blockMaterials[1];
        }
        if (isbreak)
        {
            wallRenderer.material = waterwallData.blockMaterials[2];
            Destroy(gameObject, 2.0f);
        }
    }
}
