using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_Door : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openDoorAudio;

    [Header("Door Parts")]
    [SerializeField] private List<Transform> doorParts = new List<Transform>();
    [SerializeField] private float moveDistance = 2.6f;
    [SerializeField] private float moveDuration = 5f;

    private BoxCollider doorCollider;

    private void Awake()
    {
        doorCollider = GetComponent<BoxCollider>();
        if (doorCollider == null)
        {
            doorCollider = gameObject.AddComponent<BoxCollider>();
            doorCollider.isTrigger = false; 
        }
    }

    public void OpenDoor()
    {
        PlaySound(openDoorAudio);

        if (doorCollider != null)
            doorCollider.enabled = false;

        if (doorParts.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
                doorParts.Add(transform.GetChild(i));
        }

        if (doorParts.Count > 0)
            StartCoroutine(MoveOverTime(doorParts[0], Vector3.right * moveDistance, moveDuration)); 
        if (doorParts.Count > 1)
            StartCoroutine(MoveOverTime(doorParts[1], Vector3.left * moveDistance, moveDuration)); 
        if (doorParts.Count > 2)
            StartCoroutine(MoveOverTime(doorParts[2], Vector3.up * moveDistance, moveDuration));
    }

    private IEnumerator MoveOverTime(Transform target, Vector3 offset, float duration)
    {
        Vector3 startPosition = target.position;
        Vector3 endPosition = startPosition + offset;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.position = endPosition;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}
