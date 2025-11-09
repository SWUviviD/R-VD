using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTargetAchieveTrigger : MonoBehaviour
{
    private TutorialStartTrigger p;
    private bool isTriggerActivated = false;

    private void Start()
    {
        DisableTrigger();
    }

    public void Init(TutorialStartTrigger parent)
    {
        p = parent;
        ActivateTrigger();
    }

    protected void Achieved()
    {
        isTriggerActivated = true;

        if(p != null)
        {
            p.TargetAchieved();
        }
    }

    public virtual void ActivateTrigger() { }

    protected virtual void DisableTrigger() { }

}
