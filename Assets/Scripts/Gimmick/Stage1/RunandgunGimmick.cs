using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunandgunGimmick : GimmickBase<RunandgunGimmickData>
{
    [SerializeField] private LayerMask damageLayerMask; // 데미지 존 레이어 마스크
    [SerializeField] private LayerMask healLayerMask;   // 힐 존 레이어 마스크
    [SerializeField] private PlayerStatus playerStatus; // 플레이어 상태를 관리하는 스크립트 참조


    /// <summary>
    /// 변수 선언
    /// </summary>
    private float elapsedTime;  // 경과 시간 변수

    /// <summary>
    /// 상속
    /// </summary>
    protected override void Init()
    {
        // 추가 작업 없음 
    }

    public override void SetGimmick()
    {
        elapsedTime = 0f;
    }


    /// <summary>
    /// 기믹 작동
    /// </summary>
        private void Update()
    {
        // 플레이어 아래에 Ray를 쏘아 현재 레이어 확인
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            // DamageZone일 경우 체력 감소
            if ((damageLayerMask.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= gimmickData.DamageTickInterval)
                {
                    playerStatus.TakeDamage(gimmickData.DamageAmount); // 체력 감소 함수 호출
                    elapsedTime = 0f;                     // 경과 시간 초기화
                }
            }
            // HealZone일 경우 체력 회복
            else if ((healLayerMask.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                playerStatus.FullHeal(); // 플레이어 체력 회복
            }
            else
            {
                elapsedTime = 0f; // 어떤 존에도 해당하지 않으면 경과 시간 초기화
            }
        }
    }
}
