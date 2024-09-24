using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class StarHunt : SkillBase
{
    [SerializeField] private Transform projectTrs;

    [SerializeField] private float minShotRange = 20f;
    [SerializeField] private float maxShotRange = 105;
    [SerializeField] private float maxKeyPressedTime = 0.8f;

    private float keyPressedTime = 0.0f;
    private bool isKeyPressing = false;

    private void Start()
    {
        try
        {
            var arrow = AddressableAssetsManager.Instance.SyncLoadObject(
                AddressableAssetsManager.Instance.GetPrefabPath("Prefabs", "Arrow.prefab"), 
                Defines.PoolDefines.PoolType.StarHunts.ToString()) as GameObject;
            Poolable arrowPrefab = Instantiate(arrow).GetComponent<Poolable>();
            PoolManager.Instance.CreatePool(Defines.PoolDefines.PoolType.StarHunts, arrowPrefab, 3);
            Destroy(arrowPrefab.gameObject);
        }
        catch(System.Exception e)
        {
            LogManager.LogError($"{e.Message}");
        }
    }

    private void Update()
    {
        if (isKeyPressing == false)
            return;

        keyPressedTime += Time.deltaTime;
        if (keyPressedTime > maxKeyPressedTime)
            keyPressedTime = maxKeyPressedTime;
    }

    public override void OnSkill(InputAction.CallbackContext _playerStatus) { }

    public override void OnSkillStart(InputAction.CallbackContext _playerStatus)
    {
        isKeyPressing = true;
    }

    public override void OnSkillStop(InputAction.CallbackContext _playerStatus)
    {
        isKeyPressing = false;

        float gapRange = maxShotRange - minShotRange;
        float resultRange = 20 + gapRange * keyPressedTime;

        var arrow = PoolManager.Instance.GetPoolObject(Defines.PoolDefines.PoolType.StarHunts);
        StarHuntArrow arrowScript = arrow.GetComponent<StarHuntArrow>();
        if (arrowScript != null)
        {
            arrowScript.Project(projectTrs, resultRange);
        }
    }
}
