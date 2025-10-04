using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Defines;
using static Defines.InputDefines;

public class WaterWall : GimmickBase<WaterWallData>
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private WaterVaseControll vase;
    [SerializeField] private WaterWallData waterwallData;
    [SerializeField] private float interactionDistance = 2f;

    [Header("Effects & Audio")]
    [SerializeField] private GameObject makeIceEffect;
    [SerializeField] private GameObject breakIceEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip makeIceAudio;
    [SerializeField] private AudioClip breakIceAudio;

    private bool isIce = false;
    private bool isBreak = false;

    private bool rKeyPressed = false;
    private bool eKeyPressed = false;
    private bool qKeyPressed = false;

    private GameObject currentPrefab;

    protected override void Init()
    {
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
            col.isTrigger = false;
        }

        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        UpdateBlockPrefab();
    }

    public override void SetGimmick() { }

    protected override string GetAddress() => "Data/Prefabs/Gimmick/WaterWall";

    private void OnEnable()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "StarHunt"),
            ActionPoint.IsStarted, OnQPressed);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "WaterVase"),
            ActionPoint.IsStarted, OnRPressed);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "StarFusion"),
            ActionPoint.IsStarted, OnEPressed);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsStarted, OnRightClick);
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "StarHunt"),
            ActionPoint.IsStarted, OnQPressed);

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "WaterVase"),
            ActionPoint.IsStarted, OnRPressed);

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "StarFusion"),
            ActionPoint.IsStarted, OnEPressed);

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsStarted, OnRightClick);
    }

    private void OnQPressed(InputAction.CallbackContext ctx)
    {
        qKeyPressed = true; rKeyPressed = false; eKeyPressed = false;
    }

    private void OnRPressed(InputAction.CallbackContext ctx)
    {
        rKeyPressed = true; qKeyPressed = false; eKeyPressed = false;
    }

    private void OnEPressed(InputAction.CallbackContext ctx)
    {
        eKeyPressed = true; qKeyPressed = false; rKeyPressed = false;
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > interactionDistance) return;

        if (rKeyPressed && !isIce)
        {
            isIce = true;
            PlayEffect(makeIceEffect, makeIceAudio);
            UpdateBlockPrefab();
            vase.watermove();
        }
        else if (eKeyPressed && isIce && !isBreak)
        {
            isBreak = true;
            HandleBreakWall();
            vase.watermove();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isIce && !isBreak && other.GetComponentInParent<StarHuntArrow>())
        {
            isBreak = true;
            HandleBreakWall();
        }
    }

    private void UpdateBlockPrefab()
    {
        GameObject prefabToSpawn = null;

        if (isIce && !isBreak)
            prefabToSpawn = waterwallData.WaterWallPrefabs[1];
        else if (!isIce)
            prefabToSpawn = waterwallData.WaterWallPrefabs[0];

        if (prefabToSpawn == null) return;

        Vector3 pos = transform.position;
        Quaternion rot = Quaternion.Euler(0f, 90f, 0f);
        Transform parent = transform.parent;

        if (currentPrefab != null)
        {
            Destroy(currentPrefab);
        }

        currentPrefab = Instantiate(prefabToSpawn, pos, rot, parent);
    }

    private void HandleBreakWall()
    {
        StartCoroutine(BreakWallCoroutine());
    }

    private IEnumerator BreakWallCoroutine()
    {
        PlayEffect(breakIceEffect, breakIceAudio);

        if (currentPrefab != null)
        {
            Destroy(currentPrefab);
        }

        float waitTime = (audioSource != null && breakIceAudio != null) ? breakIceAudio.length : 1f;
        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
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
