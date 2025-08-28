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

    [Header("Source")]
    [SerializeField] private GameObject makeIceEffect;
    [SerializeField] private GameObject breakeIceEffect;
    [SerializeField] AudioSource audioSource;
    [SerializeField] private AudioClip makeIceAuido;
    [SerializeField] private AudioClip breakeIceAuido;

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

        // 거리 내에서 R 키 입력으로 상호작용
        if (distanceToPlayer <= interactionDistance && Input.GetKeyDown(KeyCode.R))
        {
            isice = true;

            PlaySound(makeIceAuido);
            GameObject makeice = Instantiate(makeIceEffect, transform.position, Quaternion.identity);
            Destroy(makeice, 2f);

            UpdateBlockMaterial();
        }

        // 거리 내에서 E 키 입력으로 상호작용
        if (distanceToPlayer <= interactionDistance && Input.GetKeyDown(KeyCode.E) && isice)
        {
            isbreak = true;

            PlaySound(breakeIceAuido);
            GameObject breakice = Instantiate(breakeIceEffect, transform.position, Quaternion.identity);
            Destroy(breakice, 2f);

            UpdateBlockMaterial();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isice && other.GetComponentInParent<StarHuntArrow>())
        {
            isbreak = true;

            PlaySound(breakeIceAuido);
            GameObject breakice = Instantiate(breakeIceEffect, transform.position, Quaternion.identity);
            Destroy(breakice, 2f);

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

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
