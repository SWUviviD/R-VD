using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase
{
    SkillController controller;

    public SkillBase()
    {
        controller = null;
    }

    public void SetController(SkillController _controller)
    {
        controller = _controller;
    }

    public virtual void OnInit() { }

    public virtual void OnAttach() { }

    /// <summary>
    /// 스킬 키가 눌렸을 때
    /// </summary>
    /// <param name="_playerStatus"></param>
    /// <returns></returns>
    public abstract void OnSkillStart(PlayerStatus _playerStatus);
    /// <summary>
    /// 스킬 키가 눌리고 있을 때
    /// </summary>
    /// <param name="_playerStatus"></param>
    /// <returns></returns>
    public abstract void OnSkill(PlayerStatus _playerStatus);
    /// <summary>
    /// 스킬 키가 떼였을 때
    /// </summary>
    /// <param name="_playerStatus"></param>
    /// <returns></returns>
    public abstract void OnSkillStop(PlayerStatus _playerStatus);
    
    public virtual void OnFixedUpdate() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }
    
    public virtual void OnDetach() { }
}
