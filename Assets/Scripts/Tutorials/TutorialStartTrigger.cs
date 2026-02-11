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

    private int currentAchievedCount;

    public bool isDone { get; set; } = false;

    private void Awake()
    {
        currentAchievedCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentAchievedCount <= info.MaxCount)
            TutorialPlayer.Instance.PlayTutorialTxt(info, this, currentAchievedCount);

        StartTrigger.enabled = false;

        foreach(var trigger in triggers)
        {
            trigger?.Init(this);
        }
    }

    public void TargetAchieved()
    {
        if (isDone == true)
            return;

        ++currentAchievedCount;
        TutorialPlayer.Instance.TargetAchieved(this);
    }
}
