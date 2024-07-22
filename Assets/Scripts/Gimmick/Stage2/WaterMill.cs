using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Defines.PoolDefines;
using System.Timers;

public class WaterMill : MonoBehaviour
{
    [Header("Platform")]
    [SerializeField] private int platformCount;

    [Header("WaterMillCenter")]
    [SerializeField] private Transform platformCenter;
    [SerializeField] private float platformRadios;
    [SerializeField] private float platformSpeed;
    [SerializeField] private bool isClockwise = true;
    private float elapsedDegree = 0.0f;

    private List<WaterMillPlatform> waterMillPlatforms = new List<WaterMillPlatform>();

    private void Start()
    {
        GameObject platform = (AddressableAssetsManager.Instance.SyncLoadObject(
            AddressableAssetsManager.Instance.GetPrefabPath("Stage2", "watermillPlatform.prefab"), 
            PoolType.WaterMillPlatform.ToString())) as GameObject;

        if (platform == null)
            return;
        var platformPrefab = Instantiate(platform);

        PoolManager.Instance.CreatePool(PoolType.WaterMillPlatform, platformPrefab.GetComponent<IPoolable>(), platformCount);

        // 처음 위치 정하기
        float delataDegree = 360.0f / platformCount;
        float currentDegree = 0.0f;
        for(int i = 0; i < platformCount; ++i)
        {
            WaterMillPlatform p = PoolManager.Instance.GetPoolObject(PoolType.WaterMillPlatform) as WaterMillPlatform;
            waterMillPlatforms.Add(p);
            p.SetWaterMill(this);

            p.transform.parent = platformCenter;
            p.Position = new Vector3(1f, platformRadios * Mathf.Cos(currentDegree * Mathf.Deg2Rad), platformRadios * Mathf.Sin(currentDegree * Mathf.Deg2Rad));

            currentDegree += delataDegree;
        }
    }

    private void Update()
    {
        // 발판 움직이기
        elapsedDegree += Time.deltaTime * platformSpeed;
        if (elapsedDegree >= 360.0f) elapsedDegree -= 360.0f;

        float delataDegree = 360.0f / platformCount;
        float currentDegree = elapsedDegree * (isClockwise ? 1.0f : -1.0f);
        WaterMillPlatform platform;
        for (int i = 0; i < platformCount; ++i)
        {
            platform = waterMillPlatforms[i];

            platform.Position = new Vector3(1f, platformRadios * Mathf.Cos(currentDegree * Mathf.Deg2Rad), platformRadios * Mathf.Sin(currentDegree * Mathf.Deg2Rad));

            currentDegree += delataDegree;
        }
    }
}
