using Defines;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Defines.InputDefines;
using System.Collections;

public class SkillSwap : MonoBehaviour
{
    [Serializable]
    private class SkillInfo
    {
        public SkillBase skillBase;   // 스킬 선택
        public GameObject model;      // 손에 들릴 오브젝트
        public Animator animator;     // Animator 
    }

    [Header("Skill Unlocked")]
    [field: SerializeField] public bool[] SkillUnlocked = new bool[(int)SkillType.MAX];

    [Header("Skill Key States")]
    public bool isQPressed = false;
    public bool isEPressed = false;
    public bool isRPressed = false;

    [Header("Skill Objects")]
    [SerializeField] private SkillInfo[] skillInfos = new SkillInfo[(int)SkillType.MAX];

    private SkillType currentSkill = SkillType.MAX;

    private void Start()
    {

        foreach (var info in skillInfos)
        {
            if (info != null && info.model != null)
                info.model.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance == null) return;


        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.StarHunt.ToString()),
            ActionPoint.IsStarted, OnSkillSwapToStarHunt);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.StarFusion.ToString()),
            ActionPoint.IsStarted, OnSkillSwapToStarFusion);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.WaterVase.ToString()),
            ActionPoint.IsStarted, OnSkillSwapToWaterVase);


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
        SkillUnlocked[(int)SkillType.WaterVase] = data.IsSkill3_WaterVaseUnlocked;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;


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

    #region Skill Swap Handlers
    public void OnSkillSwapToStarHunt(InputAction.CallbackContext _ctx)
    {
        if (!SkillUnlocked[(int)SkillType.StarHunt]) return;

        isQPressed = true;
        isEPressed = false;
        isRPressed = false;

        EquipItem(SkillType.StarHunt);
    }

    public void OnSkillSwapToStarFusion(InputAction.CallbackContext _ctx)
    {
        if (!SkillUnlocked[(int)SkillType.StarFusion]) return;

        isQPressed = false;
        isEPressed = true;
        isRPressed = false;

        EquipItem(SkillType.StarFusion);
    }

    public void OnSkillSwapToWaterVase(InputAction.CallbackContext _ctx)
    {
        if (!SkillUnlocked[(int)SkillType.WaterVase]) return;

        isQPressed = false;
        isEPressed = false;
        isRPressed = true;

        EquipItem(SkillType.WaterVase);
    }
    #endregion

    private void EquipItem(SkillType _newSkill)
    {
        if (currentSkill == _newSkill) return;

        foreach (var info in skillInfos)
        {
            if (info != null && info.model != null)
                info.model.SetActive(false);
        }

        currentSkill = _newSkill;
        SkillInfo infoSkill = skillInfos[(int)currentSkill];

        if (infoSkill == null) return;

        StartCoroutine(PlayAnimationThenEquip(infoSkill));
    }

    private IEnumerator PlayAnimationThenEquip(SkillInfo info)
    {
        if (info.animator != null)
        {
            info.animator.SetTrigger("IsChangingItem");
        }
        else
        {

        }

        yield return new WaitForSeconds(2f);

        if (info.model != null)
            info.model.SetActive(true);
    }

    #region Skill Use
    private void OnSkillStarted(InputAction.CallbackContext _ctx)
    {
        if (currentSkill == SkillType.MAX) return;
        skillInfos[(int)currentSkill].skillBase?.OnSkillStart(_ctx);
    }

    private void OnSkill(InputAction.CallbackContext _ctx)
    {
        if (currentSkill == SkillType.MAX) return;
        skillInfos[(int)currentSkill].skillBase?.OnSkill(_ctx);
    }

    private void OnSkillStop(InputAction.CallbackContext _ctx)
    {
        if (currentSkill == SkillType.MAX) return;
        skillInfos[(int)currentSkill].skillBase?.OnSkillStop(_ctx);
    }
    #endregion

    public void UnlockSkill(int skillNum)
    {
        SkillUnlocked[skillNum] = true;
    }
}
