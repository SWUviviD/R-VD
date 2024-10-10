using UnityEngine;

[CreateAssetMenu(fileName = "RunandgunGimmickData", menuName = "ScriptableObjects/RunandgunGimmickData")]
public class RunandgunGimmickData : GimmickDataBase
{
    [Header("Heal Settings")]
    [Tooltip("체력 감소 비율")]
    public float DamagePercentage = 0.05f;

    [Tooltip("체력 회복 여부")]
    public bool IsHealZone = false;

    [Tooltip("체력 회복량")]
    public float HealAmount = 0f;

    [Tooltip("체력 감소 간격(초)")]
    public float DamageInterval = 1f;

}
