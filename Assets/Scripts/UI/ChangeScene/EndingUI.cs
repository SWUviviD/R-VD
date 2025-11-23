using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private VideoPlayer bgPlayer;
    [SerializeField] private VideoClip endingVideo;
    [SerializeField] private float endingTime = 105f;

    private void Start()
    {
        bgPlayer.clip = endingVideo;
        bgPlayer.isLooping = false;
        bgPlayer.playOnAwake = true;

        StartCoroutine(CoOnVideoEnd());
    }

    private IEnumerator CoOnVideoEnd()
    {
        yield return new WaitForSeconds(endingTime);
        GameManager.Instance.LoadTitle();
    }
}
