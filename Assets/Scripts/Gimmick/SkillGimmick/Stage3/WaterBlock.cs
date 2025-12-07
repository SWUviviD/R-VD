using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Defines;
using static Defines.InputDefines;
using UnityEngine.Events;

public class WaterBlock : GimmickBase<WaterBlockData>, IWaterable
{
    [Header("References")]
    [SerializeField] private bool isWallToOpen;
    [SerializeField] public Water_Door door;
    [SerializeField] private WaterBlockData waterBlockData;
    [SerializeField] private bool isUsageDone = false;
    [SerializeField] private Transform waterLevel;

    [Header("Door")]
    [SerializeField] private int toOpenWaterCount = 2;
    [SerializeField] private UnityEvent OnDoorOpen;

    [Header("Well")]
    [SerializeField] private int maxWaterCount = 4;
    [SerializeField] private UnityEvent OnWaterGain;

    [SerializeField] private int currentWaterCount = 0;

    [Header("Source")]
    [SerializeField] private GameObject waterMoveEffect;
    [SerializeField] AudioSource audioSource;
    [SerializeField] private AudioClip addWaterAudio;
    [SerializeField] private AudioClip scoupWaterAudio;

    public bool isClear = false;

    public int remainingUsage;
    private Renderer blockRenderer;

    protected override void Init()
    {
        gameObject.layer = LayerMask.NameToLayer("WaterGimmick");

        blockRenderer = GetComponent<MeshRenderer>();
        remainingUsage = waterBlockData.WaterUsage;
        UpdateBlockMaterial();

        currentWaterCount = isWallToOpen ? 0 : maxWaterCount;
        SetWaterLevel(isWallToOpen ? 0f : 1f);
    }

    public override void SetGimmick()
    {
    }

    protected override string GetAddress()
    {
        return "Data/Prefabs/Gimmick/WaterBlock";
    }

    private void SetWaterLevel(float level)
    {
        level = Mathf.Max(0.01f, level);
        waterLevel.localScale = Vector3.forward + Vector3.up * level + Vector3.right;
    }


    void InteractWithBlock(WaterVaseControll vase)
    {
        if (isUsageDone == true)
            return;

        float waterLevel = 0f;
        if(isWallToOpen == true)
        {
            if (vase.remainingUsage == false)
                return;

            ++currentWaterCount;
            PlaySound(addWaterAudio);
            AddWater();
            vase.watermove();

            if (currentWaterCount >= toOpenWaterCount)
            {
                door.OpenDoor();
                isUsageDone = true;
                OnDoorOpen?.Invoke();
            }

            waterLevel = currentWaterCount / (float)toOpenWaterCount;
        }
        else
        {
            if (currentWaterCount <= 0)
                return;

            --currentWaterCount;
            PlaySound(scoupWaterAudio);
            ScoupWater();
            vase.watermove();
            OnWaterGain?.Invoke();

            if (currentWaterCount <= 0)
            {
                isUsageDone = true;
            }

            waterLevel = currentWaterCount / (float)maxWaterCount;
        }

        SetWaterLevel(waterLevel);

        //if (remainingUsage < waterBlockData.maxWaterCapacity)
        //{
        //    PlaySound(addWaterAudio);
        //    AddWater();
        //    vase.watermove();
        //}
        //else if (remainingUsage > 0)
        //{
        //    PlaySound(scoupWaterAudio);
        //    ScoupWater();
        //    vase.watermove();
        //}
        //else
        //{
        //    LogManager.Log("더 이상 물을 담거나 사용할 수 없습니다.");
        //}

        //if (remainingUsage >= 3)
        //{
        //    isClear = true;
        //    door.OpenDoor();
        //}
    }

    void AddWater()
    {
        if (remainingUsage < waterBlockData.maxWaterCapacity)
        {
            GameObject watermove = Instantiate(waterMoveEffect, transform.position, Quaternion.identity);
            Destroy(watermove, 2f);

            remainingUsage++;
            UpdateBlockMaterial();
        }
        else
        {
            LogManager.Log("블록이 이미 가득 찼습니다.");
        }
    }

    void ScoupWater()
    {
        if (remainingUsage > 0)
        {
            GameObject watermove = Instantiate(waterMoveEffect, transform.position, Quaternion.identity);
            Destroy(watermove, 2f);

            remainingUsage--;
            UpdateBlockMaterial();
        }
        else if (remainingUsage == 0)
        {
            LogManager.Log("블록에 물이 없습니다.");
        }
    }

    void UpdateBlockMaterial()
    {
        return;

        if (waterBlockData.blockMaterials.Length > remainingUsage)
        {
            blockRenderer.material = waterBlockData.blockMaterials[remainingUsage];
        }
        else
        {
            LogManager.LogWarning("블록 Material 배열 부족");
        }
    }

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void OnWater(Transform player)
    {
        var vase = player.GetComponent<WaterVaseControll>();
        if (vase == null)
            return;

        InteractWithBlock(vase);
        LogManager.Log($"{remainingUsage}");
    }
}
