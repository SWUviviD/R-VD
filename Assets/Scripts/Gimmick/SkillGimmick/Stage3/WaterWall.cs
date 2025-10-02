using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Defines;
using static Defines.InputDefines;

public class WaterWall : GimmickBase<WaterWallData>
{
    [Header("References")]
    [SerializeField] private Transform player; // 플레이어 Transform
    [SerializeField] private WaterVaseControll vase;
    [SerializeField] private WaterWallData waterwallData;
    [SerializeField] private float interactionDistance = 2f; // 상호작용 거리

    [Header("Effects & Audio")]
    [SerializeField] private GameObject makeIceEffect;
    [SerializeField] private GameObject breakIceEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip makeIceAudio;
    [SerializeField] private AudioClip breakIceAudio;

    private Renderer wallRenderer;
    private bool isIce = false;
    private bool isBreak = false;

    private bool rKeyPressed = false;
    private bool eKeyPressed = false;
    private bool qKeyPressed = false;

    protected override void Init()
    {
        wallRenderer = GetComponent<MeshRenderer>();
        wallRenderer.material = waterwallData.blockMaterials[0];
        isIce = false;
        isBreak = false;

        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        if (GetComponent<Collider>() == null)
        {
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
        }
    }

    public override void SetGimmick() { }

    protected override string GetAddress() => "Data/Prefabs/Gimmick/WaterWall";

    private void OnEnable()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "StarHunt"),
            ActionPoint.IsStarted, ctx => { qKeyPressed = true; rKeyPressed = false; eKeyPressed = false; });

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "WaterVase"),
            ActionPoint.IsStarted, ctx => { rKeyPressed = true; qKeyPressed = false; eKeyPressed = false; });

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "StarFusion"),
            ActionPoint.IsStarted, ctx => { eKeyPressed = true; qKeyPressed = false; rKeyPressed = false; });

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsStarted, OnRightClick);
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "StarHunt"),
            ActionPoint.IsStarted, ctx => { qKeyPressed = true; rKeyPressed = false; eKeyPressed = false; });

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "WaterVase"),
            ActionPoint.IsStarted, ctx => { rKeyPressed = true; qKeyPressed = false; eKeyPressed = false; });

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "StarFusion"),
            ActionPoint.IsStarted, ctx => { eKeyPressed = true; qKeyPressed = false; rKeyPressed = false; });

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsStarted, OnRightClick);
    }



    private void OnQPressed(InputAction.CallbackContext ctx)
    {
        qKeyPressed = true;
        rKeyPressed = false;
        eKeyPressed = false;
        Debug.Log("Q Key Pressed!");
    }

    private void OnRPressed(InputAction.CallbackContext ctx)
    {
        rKeyPressed = true;
        qKeyPressed = false;
        eKeyPressed = false;
        Debug.Log("R Key Pressed!");
    }

    private void OnEPressed(InputAction.CallbackContext ctx)
    {
        eKeyPressed = true;
        qKeyPressed = false;
        rKeyPressed = false;
        Debug.Log("E Key Pressed!");
    }


    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log($"RightClick! Distance:{distanceToPlayer}, R:{rKeyPressed}, E:{eKeyPressed}, Q:{qKeyPressed}");

        if (distanceToPlayer > interactionDistance) return;

        if (rKeyPressed && !isIce)
        {
            isIce = true;
            PlayEffect(makeIceEffect, makeIceAudio);
            UpdateBlockMaterial();
            vase.watermove();
        }
        else if (eKeyPressed && isIce && !isBreak)
        {
            isBreak = true;
            PlayEffect(breakIceEffect, breakIceAudio);
            UpdateBlockMaterial();
            vase.watermove();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isIce && !isBreak && other.GetComponentInParent<StarHuntArrow>())
        {
            isBreak = true;
            PlayEffect(breakIceEffect, breakIceAudio);
            UpdateBlockMaterial();
        }
    }

    private void UpdateBlockMaterial()
    {
        if (isIce) wallRenderer.material = waterwallData.blockMaterials[1];
        if (isBreak)
        {
            wallRenderer.material = waterwallData.blockMaterials[2];
            Destroy(gameObject, 2f);
        }
    }

    private void PlayEffect(GameObject effect, AudioClip audioClip)
    {
        if (effect != null)
        {
            GameObject go = Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(go, 2f);
        }
        if (audioClip != null && audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
