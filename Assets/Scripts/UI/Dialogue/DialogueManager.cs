using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    [Header("Components")]
    [SerializeField] private GameObject toChat;               // [C] To Chat
    [SerializeField] private GameObject dialoguePanel;        // Dialogue Panel
    [SerializeField] private Text nameText;                   // 대화자 이름
    [SerializeField] private Text dialogueText;               // 대사 내용
    [SerializeField] private RectTransform nextToggleRT;      // Next Toggle (애니메이션 효과)
    [SerializeField] private DialogueContainer container;     // 기존 Dialogue Container (사용하지 않을 수 있음)
    [SerializeField] private GameObject dialogueCamera;       // Dialogue Camera

    [Header("Option UI")]
    [SerializeField] private GameObject optionsPanel;         // 선택지 패널
    [SerializeField] private Button option1Button;            // 선택지1 버튼
    [SerializeField] private Button option2Button;            // 선택지2 버튼

    [Header("Settings")]
    [SerializeField] private float fadePercent = 0.8f;
    [SerializeField] private float fadeSpeed = 1.5f;
    [SerializeField] private float readSpeed = 0.05f;
    [SerializeField] private float readFast = 0.1f;
    [SerializeField] private float nextToggleSpeed = 5f;
    [SerializeField] private KeyCode dialogueKeyCode = KeyCode.C;

    // CSV 파일 경로 (Resources 폴더 기준)
    private string csvPath = "Data/RawData/DialogInfo"; // Resources/Data/RawData/DialogInfo.csv

    // CSV에서 읽어들인 대화 데이터 저장 구조  
    // key: DialogNumber, value: Dictionary< TextNumber, DialogueLine >
    private Dictionary<int, Dictionary<int, DialogueLine>> dialogues = new Dictionary<int, Dictionary<int, DialogueLine>>();

    // 현재 진행중인 대화의 상태 변수들
    private int currentDialogNumber;
    private int currentTextNumber;
    private DialogueLine currentLine;
    private bool isDialogueActive = false;
    private bool inRange = false;

    // 선택지 관련 변수
    private bool optionSelected = false;
    private int selectedNextTextNumber = -1;

    // 기존 변수들 (애니메이션, 효과 관련)
    private Vector3 originPosition;
    private float dialogueTime = 0f;
    private float timer = 0f;

    private void Start()
    {
        // CSV 파일 읽기: Resources.Load는 파일 확장자 없이 사용 (Resources/Data/RawData/DialogInfo.csv)
        TextAsset csvFile = Resources.Load<TextAsset>(csvPath);
        if (csvFile != null)
        {
            ParseCSV(csvFile.text);
        }
        else
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + csvPath);
        }

        originPosition = nextToggleRT.GetComponent<RectTransform>().anchoredPosition;
        dialogueText.text = "";
        optionsPanel.SetActive(false);
    }

    // CSV 파싱: 각 줄을 분리하여 DialogueLine 객체로 생성한 후 dialogues 딕셔너리에 저장
    private void ParseCSV(string csvText)
    {
        // 첫 줄은 헤더이므로 건너뜀
        string[] lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        bool isFirstLine = true;
        foreach (string line in lines)
        {
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }
            string[] tokens = line.Split(',');
            if (tokens.Length < 9)
            {
                Debug.LogWarning("CSV 파싱 오류: " + line);
                continue;
            }
            // CSV의 구조: DialogNumber, TextNumber, Name, Text, NextTextNumber, Op1Txt, Op1Num, Op2Txt, Op2Num
            int dialogNum = int.Parse(tokens[0]);
            int textNum = int.Parse(tokens[1]);
            string speaker = tokens[2];
            string text = tokens[3];
            int nextTextNum = int.Parse(tokens[4]);
            string op1Txt = tokens[5];
            int op1Num = int.Parse(tokens[6]);
            string op2Txt = tokens[7];
            int op2Num = int.Parse(tokens[8]);

            DialogueLine dialogueLine = new DialogueLine(dialogNum, textNum, speaker, text, nextTextNum, op1Txt, op1Num, op2Txt, op2Num);

            if (!dialogues.ContainsKey(dialogNum))
            {
                dialogues[dialogNum] = new Dictionary<int, DialogueLine>();
            }
            dialogues[dialogNum][textNum] = dialogueLine;
        }
    }

    public bool IsDialogueActive => isDialogueActive;
    public KeyCode DialogueKeyCode => dialogueKeyCode;

    public void EnterRangeOfNPC()
    {
        inRange = true;
        toChat.SetActive(true);
        if (isDialogueActive)
        {
            toChat.SetActive(false);
        }
    }

    // dialogueID는 CSV에서의 DialogNumber에 해당
    public void StartDialogue(int dialogueID)
    {
        if (!dialogues.ContainsKey(dialogueID))
        {
            Debug.LogError("대화 번호가 CSV에 없습니다: " + dialogueID);
            return;
        }
        inRange = true;
        toChat.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogueCamera.SetActive(true);
        CameraController.Instance.SetMainCameraActive(false);
        GameManager.Instance.SetMovementInput(false);

        currentDialogNumber = dialogueID;
        currentTextNumber = 0; // 각 대화는 TextNumber 0부터 시작
        isDialogueActive = true;

        StartCoroutine(IFadeInBackground());
        StartCoroutine(IExecuteDialogue());
    }

    // 대화 진행 코루틴: CSV 데이터에 따라 대화를 진행하며 분기 처리
    private IEnumerator IExecuteDialogue()
    {
        // 약간의 딜레이 후 시작
        yield return IWaitForUnscaledSeconds(0.5f);

        while (isDialogueActive)
        {
            if (!dialogues[currentDialogNumber].TryGetValue(currentTextNumber, out currentLine))
            {
                // 다음 DialogNumber가 시작되는 지점이면 대화 종료
                break;
            }

            // 이름 표시
            nameText.text = currentLine.Name;
            // 대사 표시 (글자 단위 효과)
            yield return StartCoroutine(IDisplayDialogueLine(currentLine));

            // 분기 처리: NextTextNumber가 -1이면 선택지를 보여줌
            if (currentLine.NextTextNumber == -1)
            {
                yield return StartCoroutine(IShowOptions(currentLine));
                // 선택 후 selectedNextTextNumber에 값이 저장됨
                currentTextNumber = selectedNextTextNumber;
                optionSelected = false;
            }
            else if (currentLine.NextTextNumber == 0)
            {
                // 대화 끝: DialogNumber가 변경되어야 함.
                break;
            }
            else
            {
                currentTextNumber = currentLine.NextTextNumber;
            }

            yield return null;
        }

        // 대화 종료 처리
        StartCoroutine(IFadeOutBackground());
        yield return IWaitForUnscaledSeconds(0.5f);
        EndDialogue();
    }

    // 글자 단위로 대사를 표시하는 코루틴
    private IEnumerator IDisplayDialogueLine(DialogueLine line)
    {
        dialogueText.text = "";
        int stringLength = line.Text.Length;
        int currentChar = 0;
        bool skipLine = false;

        while (currentChar < stringLength)
        {
            dialogueText.text += line.Text[currentChar++];
            if (currentChar < stringLength)
            {
                if (Input.GetKeyDown(dialogueKeyCode))
                {
                    // 사용자가 버튼을 누르면 해당 대사를 한번에 표시
                    dialogueText.text = line.Text;
                    skipLine = true;
                    break;
                }
                yield return IWaitForUnscaledSeconds(readSpeed);
            }
            else
            {
                break;
            }
        }

        // 글자 표시가 끝난 후, 분기 없이 단순 대사라면 대화 진행키를 기다림
        if (line.NextTextNumber != -1)
        {
            yield return StartCoroutine(INextToggleMove());
            while (!Input.GetKeyDown(dialogueKeyCode))
            {
                yield return null;
            }
        }
        yield return null;
    }

    // 선택지 UI를 표시하고 사용자의 선택을 대기하는 코루틴
    private IEnumerator IShowOptions(DialogueLine line)
    {
        optionsPanel.SetActive(true);
        optionSelected = false;
        selectedNextTextNumber = -1;

        // 버튼 텍스트 세팅 (두 선택지 모두 텍스트가 있을 때)
        if (!string.IsNullOrEmpty(line.Op1Txt))
        {
            option1Button.gameObject.SetActive(true);
            option1Button.GetComponentInChildren<Text>().text = line.Op1Txt;
            option1Button.onClick.RemoveAllListeners();
            option1Button.onClick.AddListener(() => OnOptionSelected(line.Op1Num));
        }
        else
        {
            option1Button.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(line.Op2Txt))
        {
            option2Button.gameObject.SetActive(true);
            option2Button.GetComponentInChildren<Text>().text = line.Op2Txt;
            option2Button.onClick.RemoveAllListeners();
            option2Button.onClick.AddListener(() => OnOptionSelected(line.Op2Num));
        }
        else
        {
            option2Button.gameObject.SetActive(false);
        }

        // 대기: 사용자가 선택할 때까지
        while (!optionSelected)
        {
            yield return null;
        }
        optionsPanel.SetActive(false);
    }

    // 버튼 클릭 시 호출되는 함수: 선택한 다음 대사 번호를 저장
    private void OnOptionSelected(int nextNum)
    {
        selectedNextTextNumber = nextNum;
        optionSelected = true;
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
        optionsPanel.SetActive(false);
        CameraController.Instance.SetMainCameraActive(true);
        GameManager.Instance.SetMovementInput(true);
    }

    public void OutOfRange()
    {
        inRange = false;
        if (inRange == false)
        {
            StopAllCoroutines();
            toChat.SetActive(false);
            dialoguePanel.SetActive(false);
            dialogueCamera.SetActive(false);
            optionsPanel.SetActive(false);
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
        while (!Input.GetKeyDown(dialogueKeyCode))
        {
            timer += Time.unscaledDeltaTime * nextToggleSpeed;
            nextToggleRT.anchoredPosition = originPosition + 10f * Mathf.Cos(timer) * Vector3.up;
            yield return null;
        }
        nextToggleRT.gameObject.SetActive(false);
        yield return null;
    }

    // 대화 종료 후 호출되는 함수
    private void EndDialogue()
    {
        isDialogueActive = false;
        toChat.SetActive(true);
        dialoguePanel.SetActive(false);
        dialogueCamera.SetActive(false);
        optionsPanel.SetActive(false);
        CameraController.Instance.SetMainCameraActive(true);
        GameManager.Instance.SetMovementInput(true);
    }
}

// CSV 한 줄의 데이터를 저장하는 클래스
public class DialogueLine
{
    public int DialogNumber;
    public int TextNumber;
    public string Name;
    public string Text;
    public int NextTextNumber;
    public string Op1Txt;
    public int Op1Num;
    public string Op2Txt;
    public int Op2Num;

    public DialogueLine(int dialogNumber, int textNumber, string name, string text, int nextTextNumber,
                        string op1Txt, int op1Num, string op2Txt, int op2Num)
    {
        DialogNumber = dialogNumber;
        TextNumber = textNumber;
        Name = name;
        Text = text;
        NextTextNumber = nextTextNumber;
        Op1Txt = op1Txt;
        Op1Num = op1Num;
        Op2Txt = op2Txt;
        Op2Num = op2Num;
    }
}
