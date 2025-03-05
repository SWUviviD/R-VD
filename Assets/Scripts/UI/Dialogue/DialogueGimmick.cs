using UnityEngine;

public class DialogueGimmick : GimmickBase<DialogueData>
{
    private int dialogueID;
    private Vector3 cameraPosition;
    private Vector3 cameraRotation;

    private bool isPlayerEnter = false;
    private KeyCode dialogueKeyCode;

    void Start()
    {
        dialogueKeyCode = DialogueManager.Instance.DialogueKeyCode;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayerEnter || other.CompareTag("Player"))
        {
            isPlayerEnter = true;
            DialogueManager.Instance.EnterRangeOfNPC();
        }
    }

    private void OnTriggerExit()
    {
        isPlayerEnter = false;
        DialogueManager.Instance.OutOfRange();
    }

    private void Update()
    {
        if (isPlayerEnter)
        {
            if (Input.GetKeyDown(dialogueKeyCode) && !DialogueManager.Instance.IsDialogueActive)
            {
                DialogueManager.Instance.StartDialogue(dialogueID);
                DialogueManager.Instance.SetCameraAngle(cameraPosition, cameraRotation);
            }
        }
    }

    public override void SetGimmick()
    {
        dialogueID = gimmickData.DialogueID;
        cameraPosition = gimmickData.CameraPosition;
        cameraRotation = gimmickData.CameraRotation;
    }

    protected override void Init()
    {
        dialogueID = gimmickData.DialogueID;
        cameraPosition = gimmickData.CameraPosition;
        cameraRotation = gimmickData.CameraRotation;
    }
}