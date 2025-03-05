using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    [Header("Components")]
    [SerializeField] private GameObject toChat;             // [C] To Chat
    [SerializeField] private GameObject dialoguePanel;      // Dialogue Panel
    [SerializeField] private Text nameText;                 // NPC Name
    [SerializeField] private Text dialogueText;             // NPC Dialogue
    [SerializeField] private RectTransform nextToggleRT;    // Next Toggle
    [SerializeField] private DialogueContainer container;   // Dialogue Container
    [SerializeField] private GameObject dialogueCamera;     // Dialogue Camera

    [Header("Settings")]
    [SerializeField] private float fadePercent = 0.8f;
    [SerializeField] private float fadeSpeed = 1.5f;
    [SerializeField] private float readSpeed = 0.05f;
    [SerializeField] private float readFast = 0.1f;
    [SerializeField] private float nextToggleSpeed = 5f;
    [SerializeField] private KeyCode dialogueKeyCode = KeyCode.C;

    public bool IsDialogueActive => isDialogueActive;
    public KeyCode DialogueKeyCode => dialogueKeyCode;

    private Dictionary<int, DialogueContainer.Chat> chats = new Dictionary<int, DialogueContainer.Chat>();
    private Vector3 originPosition;
    private string[] dialogue;
    private bool letterIsMultiplied = false;
    private bool isDialogueActive = false;
    private bool isDialogueEnded = false;
    private bool isActive = false;
    private bool isLineEnded = false;
    private bool inRange = false;
    private bool isSkip = false;
    private float dialogueTime = 0f;
    private float timer = 0f;
    private int dialogueLine;
    private int currentLine;
    private int stringLength;
    private int currentString;

    private void Start()
    {
        container.Dialogues.ForEach(dialogue => chats.Add(dialogue.DialogueID, dialogue));
        originPosition = nextToggleRT.GetComponent<RectTransform>().anchoredPosition;
        dialogueText.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(dialogueKeyCode) && isDialogueActive && isLineEnded == false)
        {
            isSkip = true;
        }
    }

    public void EnterRangeOfNPC()
    {
        inRange = true;
        toChat.SetActive(true);
        if (isDialogueActive == true)
        {
            toChat.SetActive(false);
        }
    }

    public void StartDialogue(int dialogueID)
    {
        inRange = true;
        toChat.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogueCamera.SetActive(true);
        CameraController.Instance.SetMainCameraActive(false);

        var chat = chats[dialogueID];
        nameText.text = chat.NpcName;
        dialogue = chat.Sentences;

        if (Input.GetKeyDown(dialogueKeyCode) && !isDialogueActive)
        {
            GameManager.Instance.SetMovementInput(false);
            isDialogueActive = true;
            StartCoroutine(IFadeInBackground());
            StartCoroutine(IDialogue());
        }
    }

    private IEnumerator IDialogue()
    {
        // 1초 후 대화 시작
        yield return IWaitForUnscaledSeconds(1f);

        if (inRange == true)
        {
            dialogueLine = dialogue.Length;
            currentLine = 0;

            while (currentLine < dialogueLine || !letterIsMultiplied)
            {
                if (!letterIsMultiplied)
                {
                    letterIsMultiplied = true;
                    StartCoroutine(IDisplayString(dialogue[currentLine++]));

                    if (currentLine >= dialogueLine)
                    {
                        isDialogueEnded = true;
                    }
                }
                yield return null;
            }

            while (true)
            {
                if (Input.GetKeyDown(dialogueKeyCode) && isDialogueEnded == false)
                {
                    break;
                }
                yield return null;
            }

            // 1초 후 대화 종료
            StartCoroutine(IFadeOutBackground());
            yield return IWaitForUnscaledSeconds(1f);

            isLineEnded = false;
            isDialogueEnded = false;
            isDialogueActive = false;
            isActive = false;
            OutOfRange();
        }
    }

    private IEnumerator IDisplayString(string dialogueString)
    {
        yield return null;
        if (inRange == true)
        {
            stringLength = dialogueString.Length;
            currentString = 0;

            dialogueText.text = "";

            while (currentString < stringLength)
            {
                dialogueText.text += dialogueString[currentString++];
                if (currentString < stringLength)
                {
                    if (isSkip)
                    {
                        isSkip = false;
                        isLineEnded = true;
                        isDialogueEnded = false;
                        dialogueText.text = dialogueString;
                        currentString = stringLength;
                    }
                    else
                    {
                        yield return IWaitForUnscaledSeconds(readSpeed);
                    }
                    // if (audioClip) audioSource.PlayOneShot(audioClip, 0.5f);
                }
                else
                {
                    isLineEnded = true;
                    isDialogueEnded = false;
                    break;
                }
            }
            yield return null;

            StartCoroutine(INextToggleMove());
            while (true)
            {
                if (Input.GetKeyDown(dialogueKeyCode) && isLineEnded)
                {
                    break;
                }
                yield return null;
            }

            isLineEnded = false;
            isDialogueEnded = false;
            letterIsMultiplied = false;
            dialogueText.text = "";
        }
    }

    private IEnumerator IWaitForUnscaledSeconds(float time)
    {
        dialogueTime = 0f;
        while (dialogueTime < time)
        {
            yield return null;
            dialogueTime += Time.unscaledDeltaTime;
        }
    }

    public void DropDialogue()
    {
        isDialogueActive = false;
        toChat.SetActive(true);
        dialoguePanel.SetActive(false);
        dialogueCamera.SetActive(false);
        CameraController.Instance.SetMainCameraActive(true);
        GameManager.Instance.SetMovementInput(true);
    }

    public void OutOfRange()
    {
        inRange = false;
        if (inRange == false)
        {
            letterIsMultiplied = false;
            isDialogueActive = false;
            StopAllCoroutines();
            toChat.SetActive(false);
            dialoguePanel.SetActive(false);
            dialogueCamera.SetActive(false);
            CameraController.Instance.SetMainCameraActive(true);
        }
        GameManager.Instance.SetMovementInput(true);
    }

    public void SetCameraAngle(Vector3 position, Vector3 rotation)
    {
        dialogueCamera.transform.position = position;
        dialogueCamera.transform.rotation = Quaternion.Euler(rotation);
    }

    private IEnumerator IFadeInBackground()
    {
        Image panelImage = dialoguePanel.GetComponent<Image>();
        Color panelColor = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 0f);
        Color nameColor = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 0f);
        float speed = 0.01f * fadeSpeed;

        panelImage.color = panelColor;
        while (panelColor.a < fadePercent)
        {
            panelColor.a += speed;
            nameColor.a += speed;
            panelImage.color = panelColor;
            nameText.color = nameColor;
            yield return null;
        }
    }

    private IEnumerator IFadeOutBackground()
    {
        Image panelImage = dialoguePanel.GetComponent<Image>();
        Color panelColor = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, fadePercent);
        Color nameColor = new Color(nameText.color.r, nameText.color.g, nameText.color.b, fadePercent);
        float speed = 0.01f * fadeSpeed;

        panelImage.color = panelColor;
        while (panelColor.a > 0)
        {
            panelColor.a -= speed;
            nameColor.a -= speed;
            panelImage.color = panelColor;
            nameText.color = nameColor;
            yield return null;
        }
    }

    private IEnumerator INextToggleMove()
    {
        nextToggleRT.gameObject.SetActive(true);
        timer = 0f;
        while (isLineEnded)
        {
            timer += Time.unscaledDeltaTime * nextToggleSpeed;
            nextToggleRT.anchoredPosition = originPosition + 10f * Mathf.Cos(timer) * Vector3.up;
            yield return null;
        }
        nextToggleRT.gameObject.SetActive(false);
        yield return null;
    }
}
