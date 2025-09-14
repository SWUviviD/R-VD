using CamAnim;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimationConductor : MonoSingleton<CameraAnimationConductor>
{
    private List<Step> CamAnimData;

    private bool isThereAnim = false;

    private bool isAnimPlaying = false;

    private int currentStep = 0; 

    private Camera cam;
    private CameraEffector effector;

    public void Init(Camera cam)
    {
        this.cam = cam;

        effector = cam.GetComponent<CameraEffector>();
    }

    public void LoadCameraAnimation(string animaName)
    {
        SerializeManager.Instance.LoadDataFile(out CamAnimData, animaName, "Data/RawData/DialogCamAnimation");
        if (CamAnimData?.Count <= 0)
        {
            isThereAnim = false;
            return;
        }

        isThereAnim = true;
    }

    public bool LoadAndSetCamAnim(Transform baseTR, string animationName)
    {
        if (effector == null)
            return false;

        LoadCameraAnimation(animationName);

        if(isThereAnim == false || CamAnimData == null)
        {
            return false;
        }

        if(isAnimPlaying == true)
        {
            StopAnimation();
        }

        cam.transform.SetParent(baseTR);

        SetCamState(CamAnimData[0].CameraStates[0]);

        return true;
    }

    public bool PlayAnimation(int index, Action OnAnimOver = null)
    {
        if(index < 0 || index >= CamAnimData.Count)
        {
            isAnimPlaying = false;
            return false;
        }

        Step s = CamAnimData[index];
        if (s.CameraStates == null || s.CameraStates.Length == 0)
        {
            isAnimPlaying = false;
            return false;
        }


        SetCamState(s.CameraStates[0]);

        if(s.Shack == true)
        {
            effector?.Shake(s.ShackTime, 1f);
        }

        if (s.CameraStates.Length < 2)
        {
            OnAnimOver?.Invoke();
            isAnimPlaying = false;

            return true;
        }

        isAnimPlaying = true;
        PlayEffect(s, 0, OnAnimOver);
        return true;
    }

    public bool StopAnimation(bool forceStop = false)
    {
        if (isAnimPlaying == false)
            return true;

        if (CamAnimData[currentStep].WaitForFinish == true && forceStop == false)
            return false;

        effector.StopAllEffect();
        isAnimPlaying = false;

        return true;
    }

    private void SetCamState(CameraState state)
    {
        cam.transform.localPosition = state.LocalPosition;
        cam.transform.localRotation = Quaternion.Euler(state.LocalRotation);

        effector?.SetFOV(state.Zoom);
    }

    private void PlayEffect(Step step, int index, Action OnAnimOver = null)
    {
        if(effector == null)
        {
            isAnimPlaying = false;
            return;
        }

        currentStep = index;

        if (step.IsLoop == false && (step.CameraStates.Length - 1) <= index)
        {
            OnAnimOver?.Invoke();
            isAnimPlaying = false;
            return;
        }

        CameraState curState = step.CameraStates[index];
        CameraState nextState = step.CameraStates[index + 1];

        float segments = Mathf.Max(1, (float)(step.CameraStates.Length - 1));
        float moveTime = step.MoveTime / segments;

        // 콜백은 Move에서만 처리(중복 방지)
        effector.Move(curState.LocalPosition,
            nextState.LocalPosition,
            moveTime, () => PlayEffect(step, index + 1, OnAnimOver));

        effector.Rotate(curState.LocalRotation,
            nextState.LocalRotation,
            moveTime);

        effector.Zoom(curState.Zoom,
            nextState.Zoom,
            moveTime);
    }
}
