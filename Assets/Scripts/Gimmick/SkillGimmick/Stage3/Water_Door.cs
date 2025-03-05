using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_Door : MonoBehaviour
{
    // Update is called once per frame
    public void OpenDoor()
    {
        StartCoroutine(MoveOverTime(-2.6f, 5f));
    }

    IEnumerator MoveOverTime(float yDistance, float duration)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, yDistance, 0);

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }
}
