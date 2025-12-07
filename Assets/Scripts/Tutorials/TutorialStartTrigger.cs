using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TutorialInfo
{
    [Serializable]
    public class TalkInfo
    {
        [SerializeField, TextArea(3, 4)] private string text;
        public string Text => text.Replace("\\n", "\n");
    }

    public TalkInfo[] talkLines;
    [SerializeField, TextArea(3, 4)] private string tutorialTargetTxt;
    public string TutorialTargetTxt => tutorialTargetTxt.Replace("\\n", "\n");
    public int MaxCount;
}

public class TutorialStartTrigger : MonoBehaviour
{
    [SerializeField] private TutorialInfo info;

    [SerializeField] private Collider StartTrigger;

    [SerializeField] private TutorialTargetAchieveTrigger[] triggers;

    private void OnTriggerEnter(Collider other)
    {
        TutorialPlayer.Instance.PlayTutorialTxt(info, this);
        StartTrigger.enabled = false;

        foreach(var trigger in triggers)
        {
            trigger?.Init(this);
        }
    }

    public void TargetAchieved()
    {
        TutorialPlayer.Instance.TargetAchieved(this);
    }
}
