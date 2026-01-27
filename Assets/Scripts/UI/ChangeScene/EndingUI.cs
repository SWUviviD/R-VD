using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using static Defines.InputDefines;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private VideoPlayer bgPlayer;
    [SerializeField] private VideoClip endingVideo;
    [SerializeField] private float endingTime = 105f;

    [SerializeField] private Image skipRoll;
    [SerializeField] private Image skipRollbg;
    [SerializeField] private float skipTime = 2f;

    private void Start()
    {
        bgPlayer.clip = endingVideo;
        bgPlayer.isLooping = false;
        bgPlayer.playOnAwake = true;

        StartCoroutine(CoOnVideoEnd());

        skipRollbg.gameObject.SetActive(false);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsStarted, OnKeyPressed);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsCanceled, OnKeyCanceled);
    }

    private float elapsedTime = 0f;
    private bool isPressing = false;
    private void OnKeyPressed(InputAction.CallbackContext context)
    {
        elapsedTime = 0f;
        skipRoll.fillAmount = 0f;
        skipRollbg.gameObject.SetActive(true);
        isPressing = true;
    }

    private void OnKeyCanceled(InputAction.CallbackContext context)
    {
        elapsedTime = 0f;
        skipRollbg.gameObject.SetActive(false);
        isPressing = false;
    }

    private void Update()
    {
        if (isPressing == true)
        {
            elapsedTime += Time.deltaTime;
            skipRoll.fillAmount = elapsedTime / skipTime;
            if (elapsedTime > skipTime)
            {
                StopAllCoroutines();

                InputManager.Instance.RemoveInputEventFunction(
                    new InputActionName(ActionMapType.PlayerActions, "UINext"),
                    ActionPoint.IsStarted, OnKeyPressed);

                InputManager.Instance.RemoveInputEventFunction(
                    new InputActionName(ActionMapType.PlayerActions, "UINext"),
                    ActionPoint.IsCanceled, OnKeyCanceled);

                GameManager.Instance.LoadTitle();
            }
        }
    }

    private IEnumerator CoOnVideoEnd()
    {
        yield return new WaitForSeconds(endingTime);

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsStarted, OnKeyPressed);

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsCanceled, OnKeyCanceled);

        GameManager.Instance.LoadTitle();
    }
}
