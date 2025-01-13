using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    [Header("Components")]
    [SerializeField] private GameObject toChat;             // [C] To Chat
    [SerializeField] private GameObject dialoguePanel;      // Dialogue Panel
    [SerializeField] private Text nameText;                 // NPC Name
    [SerializeField] private Text dialogueText;             // NPC Dialogue
    [SerializeField] private RectTransform nextToggleRT;    // Next Toggle

    [Header("Settings")]
    [SerializeField] private float readSpeed = 0.05f;
    [SerializeField] private float readFast = 0.1f;
    [SerializeField] private float nextToggleSpeed = 5f;
    [SerializeField] private KeyCode dialogueKeyCode = KeyCode.C;
    public KeyCode DialogueKeyCode => dialogueKeyCode;

    private Vector3 originPosition;
    private string[] dialogue;
    private bool letterIsMultiplied = false;
    private bool isDialogueActive = false;
    private bool isDialogueEnded = false;
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
        originPosition = nextToggleRT.GetComponent<RectTransform>().anchoredPosition;
        dialogueText.text = "";
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(dialogueKeyCode) && isLineEnded == false)
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

    public void StartDialogue(in string name, in string[] sentences)
    {
        inRange = true;
        toChat.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogue = sentences;
        nameText.text = name;

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

            isLineEnded = false;
            isDialogueEnded = false;
            isDialogueActive = false;
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
        }
        GameManager.Instance.SetMovementInput(true);
    }

    private IEnumerator IFadeInBackground()
    {
        Image panelImage = dialoguePanel.GetComponent<Image>();
        Color panelColor = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 0f);
        Color nameColor = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 0f);

        panelImage.color = panelColor;
        while (panelColor.a < 0.8f)
        {
            panelColor.a += 0.01f;
            nameColor.a += 0.01f;
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
