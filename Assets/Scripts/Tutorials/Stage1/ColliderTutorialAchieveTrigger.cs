using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTutorialAchieveTrigger : TutorialTargetAchieveTrigger
{
    public enum TriggerPoint
    {
        Enter,
        Exit,
    }

    [SerializeField] private Collider col;
    [SerializeField] private TriggerPoint point;

    private void OnTriggerEnter(Collider other)
    {
        if(point == TriggerPoint.Enter)
            Achieved();
    }

    private void OnTriggerExit(Collider other)
    {
        if (point == TriggerPoint.Exit)
            Achieved();
    }

    public override void ActivateTrigger()
    {
        col.enabled = true;
    }

    protected override void DisableTrigger()
    {
        base.DisableTrigger();

        col.enabled = false;
    }
}
