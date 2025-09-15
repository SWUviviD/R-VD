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
    [Header("Skill Unlocked")]
    [field: SerializeField] public bool[] SkillUnlocked = new bool[(int)SkillType.MAX];

    [Header("Skill Key States")]
    public bool isQPressed = false;
    public bool isEPressed = false;
    public bool isRPressed = false;

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

        // 좌클릭으로 스킬 사용
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsStarted, OnSkillStarted);
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsPerformed, OnSkill);
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "Magic"),
            ActionPoint.IsCanceled, OnSkillStop);

        GameData data = GameManager.Instance.GameDataManager.GameData;
        SkillUnlocked[(int)SkillType.StarHunt] = data.IsSkill1_StarHuntUnlocked;
        SkillUnlocked[(int)SkillType.StarFusion] = data.IsSkill2_StarFusionUnlocked;
        SkillUnlocked[(int)SkillType.WaterVase] = data.IsSkill1_StarHuntUnlocked;
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
        if (SkillUnlocked[(int)SkillType.StarHunt] == false)
            return;

        isQPressed = true;
        isEPressed = false;
        isRPressed = false;

        EquipItem(SkillType.StarHunt);
    }

    public void OnSkillSwapToStarFusion(InputAction.CallbackContext _ctx)
    {
        if (SkillUnlocked[(int)SkillType.StarFusion] == false)
            return;

        isQPressed = false;
        isEPressed = true;
        isRPressed = false;

        EquipItem(SkillType.StarFusion);
    }

    public void OnSkillSwapToWaterVase(InputAction.CallbackContext _ctx)
    {
        if (SkillUnlocked[(int)SkillType.WaterVase] == false)
            return;

        isQPressed = false;
        isEPressed = false;
        isRPressed = true;

        EquipItem(SkillType.WaterVase);
    }

    /// <summary>
    /// 스킬 선택
    /// </summary>
    private void EquipItem(SkillType _newSkill)
    {
        if (currentSkill == _newSkill)
            return; 

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
        if (currentSkill == SkillType.MAX)
        {
            return;
        }

        skillInfos[(int)currentSkill].skillBase?.OnSkillStart(_ctx);
    }

    private void OnSkill(InputAction.CallbackContext _ctx)
    {
        if (currentSkill == SkillType.MAX)
        {
            return;
        }

        skillInfos[(int)currentSkill].skillBase?.OnSkill(_ctx);
    }

    private void OnSkillStop(InputAction.CallbackContext _ctx)
    {
        if (currentSkill == SkillType.MAX)
        {
            return;
        }

        skillInfos[(int)currentSkill].skillBase?.OnSkillStop(_ctx);
    }

    public void UnlockSkill(int skillNum)
    {
        Debug.Log($"{skillNum} is unlocked");
        SkillUnlocked[skillNum] = true;
    }
}