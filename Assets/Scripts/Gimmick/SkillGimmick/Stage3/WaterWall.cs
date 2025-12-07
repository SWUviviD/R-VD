using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Defines;
using static Defines.InputDefines;

public class WaterWall : GimmickBase<WaterWallData>, IWaterable
{
    [Header("References")]
    [SerializeField] private WaterWallData waterwallData;
    [SerializeField] private Collider wallCheckCollider;

    [Header("Effects & Audio")]
    [SerializeField] private GameObject makeIceEffect;
    [SerializeField] private GameObject breakIceEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip makeIceAudio;
    [SerializeField] private AudioClip breakIceAudio;

    private bool isIce = false;
    private bool isBreak = false;

    private GameObject currentPrefab;

    protected override void Init()
    {
        gameObject.layer = LayerMask.NameToLayer("WaterGimmick");

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

        UpdateBlockPrefab();
    }

    public override void SetGimmick() { }

    protected override string GetAddress() => "Data/Prefabs/Gimmick/WaterWall";

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
        if (isIce && isBreak == false)
        {
            waterwallData.WaterWallPrefabs[0].SetActive(true);
            waterwallData.WaterWallPrefabs[1].SetActive(false);
            wallCheckCollider.gameObject.SetActive(false);
        }
        else if (!isIce)
        {
            waterwallData.WaterWallPrefabs[0].SetActive(false);
            waterwallData.WaterWallPrefabs[1].SetActive(true);
            wallCheckCollider.gameObject.SetActive(true);
        }
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

        waterwallData.WaterWallPrefabs[0].SetActive(false);
        waterwallData.WaterWallPrefabs[1].SetActive(false);
        wallCheckCollider.gameObject.SetActive(false);

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

    public void OnWater(Transform player)
    {
        WaterVaseControll vase = player.GetComponent<WaterVaseControll>();
        if (vase == null)
            return;

        if (isIce == false && vase.remainingUsage == true)
        {
            isIce = true;
            PlayEffect(makeIceEffect, makeIceAudio);
            UpdateBlockPrefab();
            vase.watermove();
        }
        else if (isIce && !isBreak)
        {
            isBreak = true;
            HandleBreakWall();
        }
    }
}
