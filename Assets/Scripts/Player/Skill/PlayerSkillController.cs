using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Defines.SkillDefines;

[RequireComponent(typeof(SkillController))]
public class PlayerSkillController : MonoBehaviour
{
    [Serializable]
    private struct PlayerSkillSet
    {
        public SkillIndex Index { get { return index; } }
        [SerializeField] private SkillIndex index;

        public SkillBase Skill { get { return skill; } }
        [SerializeField] private SkillBase skill;
    }


    private SkillController skillController;
    [SerializeField] private List<PlayerSkillSet> playerSkillSets = new List<PlayerSkillSet>();

    private void Awake()
    {
        skillController = GetComponent<SkillController>();
    }

    private void SkillAchieve(SkillIndex _index)
    {
        PlayerSkillSet playerSkillSet = playerSkillSets.Find(_ => _.Index == _index);
        skillController.AddSkillBase(playerSkillSet.Index, playerSkillSet.Skill);
    }
}
