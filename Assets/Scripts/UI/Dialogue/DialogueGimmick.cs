using CamAnim;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static Defines.InputDefines;

[Serializable]
public class DialogueCallback
{
    public enum Mode
    {
        MovePosAndRotation,
        ConductFunction,
    }

    [field: SerializeField] public Mode mode { get; set; }

    // set pos & rot
    [field: SerializeField] public Transform PosTarget { get; set; }
    [field: SerializeField] public Transform TartgetPos { get; set; }
    [field: SerializeField] public Vector3 WorldPos { get; set; }
    [field: SerializeField] public Vector3 WorldRot { get; set; }

    // set func
    [field: SerializeField] public UnityEvent Event { get; set; }
}

public class DialogueGimmick : GimmickBase<DialogueData>
{
    [SerializeField] private CheckpointGimmick bindCheckPoint;

    [SerializeField] private string CamAnimName = "DialogueCam1";
    [SerializeField] private bool isOnButtonPlay = false;

    [SerializeField] private Collider col = null;
    [field: SerializeField] public List<DialogueCallback> OnDialogStart = new List<DialogueCallback>();
    [field: SerializeField] public List<DialogueCallback> OnDialogEnd = new List<DialogueCallback>();

    private int dialogueID;

    private bool isTherePlayer = false;
    private GameObject player = null;

    [SerializeField] private CameraAnimationData data;

    private void Start()
    {
        data = GetComponent<CameraAnimationData>();
        if (GameManager.Instance.TryTimes > 0)
        {
            if (bindCheckPoint != null &&
                bindCheckPoint.checkPointNumber < GameManager.Instance.LastTryCheckPoint)
            {
                foreach (var d in data.Steps)
                {
                    if (d.AchieveData != null)
                        AchieveUI.Instance.Achieve(d.AchieveData);
                }
                Invoke(OnDialogEnd);

                gameObject.SetActive(false);
            }
        }
    }

    private void OnChatStart(InputAction.CallbackContext context)
    {
        if (isOnButtonPlay)
        {
            col.enabled = false;
            if(data != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueID,
                    data, transform, () => Invoke(OnDialogStart), () => Invoke(OnDialogEnd));
            }
            else
            {
                DialogueManager.Instance.StartDialogue(dialogueID,
                    CamAnimName, transform, () => Invoke(OnDialogStart), () => Invoke(OnDialogEnd));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerHp>(out _) == true)
        {
            isTherePlayer = true;

            col.enabled = false;
            if (data != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueID,
                    data, transform, () => Invoke(OnDialogStart), () => Invoke(OnDialogEnd));
            }
            else
            {
                DialogueManager.Instance.StartDialogue(dialogueID,
                    CamAnimName, transform, () => Invoke(OnDialogStart), () => Invoke(OnDialogEnd));
            }

            DialogueManager.Instance.EnterRangeOfNPC();
            player = other.gameObject;

            InputManager.Instance.RemoveInputEventFunction(
                new InputActionName(ActionMapType.PlayerActions, "UIChat"),
                ActionPoint.IsStarted, OnChatStart);
            InputManager.Instance.AddInputEventFunction(
                new InputActionName(ActionMapType.PlayerActions, "UIChat"),
                ActionPoint.IsStarted, OnChatStart);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isTherePlayer = false;
            if (isOnButtonPlay == true)
            {
                DialogueManager.Instance.OutOfRange();

                InputManager.Instance.RemoveInputEventFunction(
                    new InputActionName(ActionMapType.PlayerActions, "UIChat"),
                    ActionPoint.IsStarted, OnChatStart);
            }
        }
    }

    public void MoveAndRotateTarget(Transform target, Transform posTarget, Vector3 worldPos, Vector3 worldRotate)
    {
        if (target == null)
        {
            return;
        }

        if ((posTarget))
        {
            worldPos = posTarget.position;
            worldRotate = posTarget.rotation.eulerAngles;
        }

        if (target.TryGetComponent<PlayerMove>(out var move) == true)
        {

            move.SetPosition(worldPos);
            move.SetRotation(worldRotate);
            return;
        }

        target.position = worldPos;
        target.rotation = Quaternion.Euler(worldRotate);
    }

    public override void SetGimmick()
    {
        dialogueID = gimmickData.DialogueID;
        //cameraPosition = gimmickData.CameraPosition;
        //cameraRotation = gimmickData.CameraRotation;
    }

    protected override void Init()
    {
        dialogueID = gimmickData.DialogueID;
        //cameraPosition = gimmickData.CameraPosition;
        //cameraRotation = gimmickData.CameraRotation;
    }

    private void Invoke(List<DialogueCallback> list)
    {
        foreach (DialogueCallback callback in list)
        {
            if (callback == null)
                continue;

            switch (callback.mode)
            {
                case DialogueCallback.Mode.MovePosAndRotation:
                    {
                        MoveAndRotateTarget(callback.PosTarget, callback.TartgetPos, callback.WorldPos, callback.WorldRot);
                        break;
                    }

                case DialogueCallback.Mode.ConductFunction:
                    {
                        callback.Event?.Invoke();
                        break;
                    }

            }

        }
    }
}