using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionSettor : GimmickBase<PlayerPositionSettorData>
{
    [SerializeField] private GameObject posPointer;


    private const string playerPrefabAddress = "Data/Prefabs/Player";
    private GameObject player;
    public GameObject Player => player;
    private PlayerMove move;

    protected override void Init()
    {
        SetPlayer();
    }

    public override void SetGimmick()
    {
        posPointer.SetActive(false);
        if (player == null)
            SetPlayer();

        move.SetPosition(transform.position);
        move.SetRotation(transform.rotation.eulerAngles);
        player.transform.rotation = transform.rotation;
        player.SetActive(true);
    }

    private void SetPlayer()
    {
        GameObject prefab = AddressableAssetsManager.Instance.SyncLoadObject(playerPrefabAddress, playerPrefabAddress) as GameObject;
        player = Instantiate(prefab);
        player.SetActive(false);
        move = player.GetComponent<PlayerMove>();
    }
}
