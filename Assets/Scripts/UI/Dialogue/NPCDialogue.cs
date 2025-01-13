using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    // Component
    private NPCDialogue npcDialogue;

    // NPC ID (Name and Dialogue)
    [SerializeField] private int dialogueID;

    public string npcName;
    [TextArea(5, 10)]
    public string[] sentences;

    private bool isPlayerEnter = false;
    private KeyCode dialogueKeyCode;

    void Start()
    {
        npcDialogue = GetComponent<NPCDialogue>();
        dialogueKeyCode = DialogueManager.Instance.DialogueKeyCode;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayerEnter || other.CompareTag("Player"))
        {
            isPlayerEnter = true;
            npcDialogue.enabled = true;
            DialogueManager.Instance.EnterRangeOfNPC();
        }
    }

    private void OnTriggerExit()
    {
        isPlayerEnter = false;
        npcDialogue.enabled = false;
        DialogueManager.Instance.OutOfRange();
    }

    private void Update()
    {
        if (isPlayerEnter)
        {
            npcDialogue.enabled = true;
            if (Input.GetKeyDown(dialogueKeyCode))
            {
                npcDialogue.enabled = true;
                //DialogueManager.Instance.names = database.datas[dialogueID].name;
                DialogueManager.Instance.StartDialogue(in npcName, in sentences);
            }
        }
    }
}