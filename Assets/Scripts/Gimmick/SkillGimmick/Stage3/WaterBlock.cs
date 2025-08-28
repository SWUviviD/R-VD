using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Defines;
using static Defines.InputDefines;

public class WaterBlock : GimmickBase<WaterBlockData>
{
    [Header("References")]
    [SerializeField] public Transform player;
    [SerializeField] public WaterVaseControll vase;
    [SerializeField] public Water_Door door;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private WaterBlockData waterBlockData;

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

    private void OnEnable()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager.Instance is NULL");
            return;
        }

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsStarted, OnSkillStarted);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsPerformed, OnSkillStarted);
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsStarted, OnSkillStarted);

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsPerformed, OnSkillStarted);
    }


    private void OnSkillStarted(InputAction.CallbackContext context)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= interactionDistance)
        {
            InteractWithBlock();
        }
    }


    void InteractWithBlock()
    {
        if (remainingUsage < waterBlockData.maxWaterCapacity)
        {
            PlaySound(addWaterAudio);
            AddWater();            
            vase.watermove();   
        }
        else if (remainingUsage > 0)
        {
            PlaySound(scoupWaterAudio);
            ScoupWater();       
            vase.watermove();   
        }
        else
        {
            LogManager.Log("더 이상 물을 담거나 사용할 수 없습니다.");
        }

        if (remainingUsage >= 3)
        {
            isClear = true;
            door.OpenDoor();
        }
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
            LogManager.Log("블록 빔");
        }
    }

    void UpdateBlockMaterial()
    {
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
}
