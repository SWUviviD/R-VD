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
    public abstract bool UseSkill(Status status);
    public virtual void OnFixedUpdate() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnDetach() { }
}
