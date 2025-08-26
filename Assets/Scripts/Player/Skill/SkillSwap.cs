using Defines;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Defines.InputDefines;

public class SkillSwap : MonoBehaviour
{
    [Serializable]
    private class SkillInfo
    {
        public SkillBase skillBase;   // 스킬 선택
        public GameObject model;      // 손에 들릴 오브젝트
        public AnimationClip clip;    // 애니메이션
    }

    [Header("SkillObject")]
    [SerializeField] private SkillInfo[] skillInfos = new SkillInfo[(int)SkillType.MAX];

    private SkillType currentSkill = SkillType.MAX;

    private void OnEnable()
    {
        // Q
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.StarHunt.ToString()),
            ActionPoint.IsStarted, OnSkillSwapToStarHunt);

        // E
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.StarFusion.ToString()),
            ActionPoint.IsStarted, OnSkillSwapToStarFusion);

        // R
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.WaterVase.ToString()),
            ActionPoint.IsStarted, OnSkillSwapToWaterVase);

        // Magic Key → 좌클릭으로 스킬 사용
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsStarted, OnSkillStarted);
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsPerformed, OnSkill);
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsCanceled, OnSkillStop);
    }

    private void OnDisable()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.StarHunt.ToString()),
            ActionPoint.IsStarted, OnSkillSwapToStarHunt);
        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.StarFusion.ToString()),
            ActionPoint.IsStarted, OnSkillSwapToStarFusion);
        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.WaterVase.ToString()),
            ActionPoint.IsStarted, OnSkillSwapToWaterVase);

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsStarted, OnSkillStarted);
        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsPerformed, OnSkill);
        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsCanceled, OnSkillStop);
    }

    public void OnSkillSwapToStarHunt(InputAction.CallbackContext _ctx)
    {
        EquipItem(SkillType.StarHunt);
    }

    public void OnSkillSwapToStarFusion(InputAction.CallbackContext _ctx)
    {
        EquipItem(SkillType.StarFusion);
    }

    public void OnSkillSwapToWaterVase(InputAction.CallbackContext _ctx)
    {
        EquipItem(SkillType.WaterVase);
    }

    /// <summary>
    /// 선택 오브젝트만 보이게
    /// </summary>
    private void EquipItem(SkillType _newSkill)
    {
        for (int i = 0; i < skillInfos.Length; i++)
        {
            if (skillInfos[i] != null && skillInfos[i].model != null)
                skillInfos[i].model.SetActive(false);
        }

        currentSkill = _newSkill;
        if (skillInfos[(int)currentSkill].model != null)
            skillInfos[(int)currentSkill].model.SetActive(true);
    }

    private void OnSkillStarted(InputAction.CallbackContext _ctx)
    {
        skillInfos[(int)currentSkill].skillBase?.OnSkillStart(_ctx);
    }

    private void OnSkill(InputAction.CallbackContext _ctx)
    {
        skillInfos[(int)currentSkill].skillBase?.OnSkill(_ctx);
    }

    private void OnSkillStop(InputAction.CallbackContext _ctx)
    {
        skillInfos[(int)currentSkill].skillBase?.OnSkillStop(_ctx);
    }
}