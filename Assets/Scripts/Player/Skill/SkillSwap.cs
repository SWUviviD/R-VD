using System.Collections;
using UnityEngine;

public class SkillSwap : MonoBehaviour
{
    private bool ignoreInput = false;
    private KeyCode lastReleasedKey;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q)) HandleKeyRelease(KeyCode.Q, KeyCode.W, KeyCode.E);
        if (Input.GetKeyUp(KeyCode.W)) HandleKeyRelease(KeyCode.W, KeyCode.Q, KeyCode.E);
        if (Input.GetKeyUp(KeyCode.E)) HandleKeyRelease(KeyCode.E, KeyCode.Q, KeyCode.W);

        if (!ignoreInput)
        {
            if (Input.GetKeyDown(KeyCode.Q)) LogManager.Log("Q Pressed");
            if (Input.GetKeyDown(KeyCode.W)) LogManager.Log("W Pressed");
            if (Input.GetKeyDown(KeyCode.E)) LogManager.Log("E Pressed");
        }
    }

    void HandleKeyRelease(KeyCode releasedKey, KeyCode blockKey1, KeyCode blockKey2)
    {
        lastReleasedKey = releasedKey;
        StartCoroutine(BlockInputForDuration(blockKey1, blockKey2, 2.0f));
    }

    IEnumerator BlockInputForDuration(KeyCode blockKey1, KeyCode blockKey2, float duration)
    {
        ignoreInput = true;
        yield return new WaitForSeconds(duration);
        ignoreInput = false;
    }
}
