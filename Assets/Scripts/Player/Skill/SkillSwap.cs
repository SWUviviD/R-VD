using System.Collections;
using UnityEngine;

public class SkillSwap : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Transform holdPoint;

    [Header("SkillObject")]
    [SerializeField] private GameObject Bow;
    [SerializeField] private GameObject Sword;
    [SerializeField] private GameObject Vase;

    private GameObject currentItem;          // 현재 들고 있는 아이템
    private GameObject currentPrefab;        // 현재 아이템의 원본 프리팹을 저장

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EquipItem(Bow);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            EquipItem(Sword);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            EquipItem(Vase);
        }
    }

    private void EquipItem(GameObject prefab)
    {
        // 이미 같은 프리팹을 들고 있으면 아무 것도 하지 않음
        if (currentPrefab == prefab)
        {
            return;
        }

        // 다른 아이템을 들고 있다면 제거
        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        // 새 아이템 생성 후 holdPoint에 붙임
        currentItem = Instantiate(prefab, holdPoint);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        // 현재 아이템 프리팹 기록
        currentPrefab = prefab;
    }
}
