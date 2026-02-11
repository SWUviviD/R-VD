using CamAnim;
using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Defines.InputDefines;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    [Header("Components")]
    [SerializeField] private GameObject toChat;               // [C] To Chat
    [SerializeField] private GameObject dialoguePanel;        // Dialogue Panel
    [SerializeField] private Text nameText;                   // 대화자 이름
    [SerializeField] private Text dialogueText;               // 대사 내용
    [SerializeField] private RectTransform nextToggleRT;      // Next Toggle (애니메이션 효과)
    [SerializeField] private Camera dialogueCamera;       // Dialogue Camera

    [Header("Option UI")]
    [SerializeField] private GameObject optionsPanel;         // 선택지 패널
    [SerializeField] private Button option1Button;            // 선택지1 버튼
    [SerializeField] private Text op1Text;                    // 선택지1 텍스트
    [SerializeField] private Image op1BtnBg;
    [SerializeField] private Button option2Button;            // 선택지2 버튼
    [SerializeField] private Text op2Text;                    // 선택지2 텍스트
    [SerializeField] private Image op2BtnBg;

    [Header("Settings")]
    [SerializeField] private float readSpeed = 0.05f;
    [SerializeField] private float nextToggleSpeed = 5f;

    // CSV 파일 경로 (Resources 폴더 기준)
    private string csvPath = "Data/RawData/DialogInfo"; // Resources/Data/RawData/DialogInfo.csv

    // CSV에서 읽어들인 대화 데이터 저장 구조  
    private List<List<DialogueLine>> dialogues = new List<List<DialogueLine>>();

    private Coroutine showTextCoroutine = null;
    private Coroutine nextBtnCoroutine = null;

    // 현재 진행중인 대화의 상태 변수들
    private int currentDialogNumber;
    private int currentLineNumber;
    private Action callback = null;

    private int currentSelectOption = -1;
    private int op1NextTextNum = -1;
    private int op2NextTextNum = -1;

    public bool IsDialogueActive => isDialogueActive;
    private bool isDialogueActive = false;
    private bool isOptionShowing = false;
    private bool isTextShowing = false;
    private bool isCamReady;

    // 기존 변수들 (애니메이션, 효과 관련)
    private Vector3 originPosition;
    private float dialogueTime = 0f;
    private float timer = 0f;


    private void Start()
    {
        CameraController.Instance.SetDialogueCamera(dialogueCamera);
        CameraAnimationConductor.Instance.Init(dialogueCamera);

        // CSV 파일 읽기: Resources.Load는 파일 확장자 없이 사용 (Resources/Data/RawData/DialogInfo.csv)
        List<DialogInfo> dialogueDatas;
        SerializeManager.Instance.LoadDataFile(out dialogueDatas, "DialogInfo");
        if (dialogueDatas.Count <= 0)
        {
            return;
        }

        int currentNumber = -1;
        foreach(var dialogueData in dialogueDatas)
        {
            DialogueLine dialogueLine = new DialogueLine(
                dialogueData.Name,
                dialogueData.Text, 
                dialogueData.NextTextNumber, 
                dialogueData.Op1Txt, 
                dialogueData.Op1Num, 
                dialogueData.Op2Txt, 
                dialogueData.Op2Num);

            if (currentNumber != dialogueData.DialogNumber)
            {
                currentNumber = dialogueData.DialogNumber;
                dialogues.Add(new List<DialogueLine>());
            }
            dialogues[currentNumber].Add(dialogueLine);
        }

        originPosition = nextToggleRT.anchoredPosition;
        dialogueText.text = "";
        optionsPanel.SetActive(false);
    }

    public void EnterRangeOfNPC()
    {
        //toChat.SetActive(true);
    }

    public void OutOfRange()
    {
        toChat.SetActive(false);
    }

    public void StartDialogue(int dialogueID, CameraAnimationData cameraAnimationData,
        Transform baseTR, Action OnDialogStart = null, Action OnDialogEndFuc = null)
    {
        if (dialogues.Count <= dialogueID)
        {
            return;
        }

        isDialogueActive = true;
        callback?.Invoke();
        callback = OnDialogEndFuc;
        StopAllCoroutines();

        // UI 세팅
        GameManager.Instance.HpUI?.SetVisable(false);
        toChat.SetActive(false);
        dialoguePanel.SetActive(true);

        // 카메라 세팅
        CameraController.Instance.SetCameraMode(CameraController.CameraMode.Dialog);
        isCamReady = CameraAnimationConductor.Instance.SetCamAnim(baseTR, cameraAnimationData.Steps);

        // 인풋 세팅
        GameManager.Instance.SetMovementInput(false);
        GameManager.Instance.SetSkillInput(false);
        GameManager.Instance.SetCameraInput(false);
        GameManager.Instance.ShowCursor(true);

        PlayerMove move = GameManager.Instance.Player.GetComponent<PlayerMove>();
        move?.StopMoving();

        SetInput("UINext", true, OnSkip);

        OnDialogStart?.Invoke();

        StartDialog(dialogueID);
    }

    // dialogueID는 CSV에서의 DialogNumber에 해당
    public void StartDialogue(int dialogueID, string camAnimName, Transform baseTR, Action OnDialogStart = null, Action OnDialogEndFuc = null)
    {
        if (dialogues.Count <= dialogueID)
        {
            return;
        }

        callback?.Invoke();
        callback = OnDialogEndFuc;
        StopAllCoroutines();

        // UI 세팅
        GameManager.Instance.HpUI?.SetVisable(false);
        toChat.SetActive(false);
        dialoguePanel.SetActive(true);

        currentSelectOption = -1;
        op1BtnBg.gameObject.SetActive(false);
        op2BtnBg.gameObject.SetActive(false);

        // 카메라 세팅
        CameraController.Instance.SetCameraMode(CameraController.CameraMode.Dialog);
        if(camAnimName.Length > 0)
        {
            isCamReady = CameraAnimationConductor.Instance.LoadAndSetCamAnim(baseTR, camAnimName);
        }

        // 인풋 세팅
        GameManager.Instance.SetMovementInput(false);
        GameManager.Instance.SetSkillInput(false);
        GameManager.Instance.SetCameraInput(false);
        GameManager.Instance.ShowCursor(true);

        PlayerMove move = GameManager.Instance.Player.GetComponent<PlayerMove>();
        move?.StopMoving();

        SetInput("UINext", true, OnSkip);

        OnDialogStart?.Invoke();

        StartDialog(dialogueID);
    }

    private void SetInput(string key, bool isActive, Action<InputAction.CallbackContext> _callback)
    {
        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, key),
            ActionPoint.IsStarted, _callback);

        if(isActive)
        {
            InputManager.Instance.AddInputEventFunction(
                new InputActionName(ActionMapType.PlayerActions, key),
                ActionPoint.IsStarted, _callback);
        }
    }

    private void OnSkip(InputAction.CallbackContext context)
    {
        if (isOptionShowing == true)
            return;

        if(isTextShowing == true)
        {
            SkipText();
            return;
        }

        if(isCamReady == true && CameraAnimationConductor.Instance.StopAnimation() == false)
        {
            return;
        }

        if((currentLineNumber + 1) == dialogues[currentDialogNumber].Count)
        {
            OnDialogEnd();
        }

        int nextNum = dialogues[currentDialogNumber][currentLineNumber].NextTextNumber;
        ShowText(nextNum);
    }

    private void StartDialog(int dialogIndex)
    {
        isDialogueActive = true;

        currentDialogNumber = dialogIndex;
        ShowText(0);
    }

    private void ShowText(int index)
    {
        if(index >= dialogues[currentDialogNumber].Count)
        {
            OnDialogEnd();
            return;
        }

        currentLineNumber = index;

        AchieveData data = null;
        if (isCamReady)
        {
            CameraAnimationConductor.Instance.PlayAnimation(currentLineNumber);
            data = CameraAnimationConductor.Instance.GetAchieveData(currentLineNumber);
        }
        SetUI(dialogues[currentDialogNumber][index]);

        isOptionShowing = false;

        optionsPanel.SetActive(false);
        if (showTextCoroutine != null) StopCoroutine(showTextCoroutine);
        nextToggleRT.gameObject.SetActive(false);

        if (nextBtnCoroutine != null) StopCoroutine(nextBtnCoroutine);
        showTextCoroutine = StartCoroutine(
            CoShowText(dialogues[currentDialogNumber][index].Text,
            () => OnTextAllShown(data)));
    }

    private void SkipText()
    {
        StopCoroutine(showTextCoroutine);
        dialogueText.text = dialogues[currentDialogNumber][currentLineNumber].Text;
        isTextShowing = false;
        OnTextAllShown(isCamReady ? CameraAnimationConductor.Instance.GetAchieveData(currentLineNumber) : null);
    }

    private IEnumerator CoShowText(string text, Action _callback = null)
    {
        isTextShowing = true;

        var builder = new System.Text.StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            builder.Append(text[i]);
            dialogueText.text = builder.ToString();
            yield return CoWaitForUnscaledSeconds(readSpeed);
        }

        _callback?.Invoke();

        isTextShowing = false;
    }

    private void SetUI(DialogueLine info)
    {
        nameText.text = info.Name;

        if(info.NextTextNumber > 0)
        {
            return;
        }

        // 선택지 세팅
        if (info.Op1Txt.Length > 0)
        {
            op1Text.text = info.Op1Txt;
            option1Button.onClick.RemoveAllListeners();
            option1Button.onClick.AddListener(() => ShowText(info.Op1Num));
            op1NextTextNum = info.Op1Num;
        }
        if (info.Op2Txt.Length > 0)
        {
            op2Text.text = info.Op2Txt;
            option2Button.onClick.RemoveAllListeners();
            option2Button.onClick.AddListener(() => ShowText(info.Op2Num));
            op2NextTextNum = info.Op2Num;
        }
    }

    private void OnTextAllShown(AchieveData data)
    {
        if (dialogues[currentDialogNumber][currentLineNumber].NextTextNumber <= -1)
        {
            ShowOptions();
        }
        else
        {
            ShowNextBtn();
        }

        if(data != null)
        {
            SetInput("UINext", false, OnSkip);
            AchieveUI.Instance.ShowUI(data, () => {
                SetInput("UINext", true, OnSkip);

                if ((currentLineNumber + 1) == dialogues[currentDialogNumber].Count)
                {
                    OnDialogEnd();
                }

                int nextNum = dialogues[currentDialogNumber][currentLineNumber].NextTextNumber;
                ShowText(nextNum);
            });
        }
    }

    private void ShowOptions()
    {
        isOptionShowing = true;

        DialogueLine line = dialogues[currentDialogNumber][currentLineNumber];

        if (line.Op1Txt.Length > 0) option1Button.gameObject.SetActive(true);
        else option1Button.gameObject.SetActive(false);

        if (line.Op2Txt.Length > 0) option2Button.gameObject.SetActive(true);
        else option2Button.gameObject.SetActive(false);

        optionsPanel.gameObject.SetActive(true);

        currentSelectOption = -1;
        op1BtnBg.gameObject.SetActive(false);
        op2BtnBg.gameObject.SetActive(false);

        SetInput("UISelectLeft", true, OnSelectOption);
        SetInput("UISelectRight", true, OnSelectOption);

        SetInput("UINext", false, OnConfirmOption);
    }

    public void OnHoverOptionFromMouse(int num)
    {
        if (currentSelectOption == num)
            return;

        currentSelectOption = num;
        op1BtnBg.gameObject.SetActive(currentSelectOption == 0);
        op2BtnBg.gameObject.SetActive(currentSelectOption == 1);
    }

    public void OnExitOptionFromMouse(int num)
    {
        currentSelectOption = -1;
        op1BtnBg.gameObject.SetActive(false);
        op2BtnBg.gameObject.SetActive(false);

        Color temp = op1BtnBg.color; temp.a = 1f;
        op1BtnBg.color = temp;
        op2BtnBg.color = temp;
    }

    private void OnSelectOption(InputAction.CallbackContext context)
    {
        if(currentSelectOption == -1)
        {
            currentSelectOption = 0;
            op1BtnBg.gameObject.SetActive(true);
            op2BtnBg.gameObject.SetActive(false);

            SetInput("UINext", true, OnConfirmOption);
            return;
        }

        currentSelectOption = currentSelectOption == 0 ? 1 : 0;
        op1BtnBg.gameObject.SetActive(currentSelectOption == 0);
        op2BtnBg.gameObject.SetActive(currentSelectOption == 1);
    }

    private void OnConfirmOption(InputAction.CallbackContext context)
    {
        if (currentSelectOption < 0)
            return;

        ShowText(currentSelectOption == 0 ? op1NextTextNum : op2NextTextNum);
        SetInput("UINext", false, OnConfirmOption);
    }

    private void ShowNextBtn()
    {
        optionsPanel.gameObject.SetActive(false);
        if (nextBtnCoroutine != null) StopCoroutine(nextBtnCoroutine);
        nextBtnCoroutine = StartCoroutine(CoNextToggleMove());
    }

    private void OnDialogEnd()
    {
        SetInput("UINext", false, OnSkip);

        isDialogueActive = false;
        GameManager.Instance.HpUI?.SetVisable(true);
        toChat.SetActive(false);
        dialoguePanel.SetActive(false);
        optionsPanel.SetActive(false);

        dialogueCamera.transform.SetParent(gameObject.transform);

        CameraController.Instance.SetCameraMode(CameraController.CameraMode.Orbit);
        GameManager.Instance.SetMovementInput(true);
        GameManager.Instance.SetSkillInput(true);
        GameManager.Instance.SetCameraInput(true);
        GameManager.Instance.ShowCursor(false);

        callback?.Invoke();
    }

    private IEnumerator CoWaitForUnscaledSeconds(float time)
    {
        dialogueTime = 0f;
        while (dialogueTime < time)
        {
            yield return null;
            dialogueTime += Time.unscaledDeltaTime;
        }
    }

    private IEnumerator CoNextToggleMove()
    {
        nextToggleRT.gameObject.SetActive(true);
        timer = 0f;
        while (true)
        {
            timer += Time.unscaledDeltaTime * nextToggleSpeed;
            nextToggleRT.anchoredPosition = originPosition + 10f * Mathf.Cos(timer) * Vector3.up;
            yield return null;
        }
    }

    private void OnDisable()
    {
        SetInput("UINext", false, OnSkip);

        isDialogueActive = false;
        GameManager.Instance.HpUI?.SetVisable(true);
        toChat.SetActive(false);
        dialoguePanel.SetActive(false);
        optionsPanel.SetActive(false);

        dialogueCamera?.transform.SetParent(gameObject.transform);

        CameraController.Instance.SetCameraMode(CameraController.CameraMode.Orbit);
        GameManager.Instance.SetMovementInput(true);
        GameManager.Instance.SetSkillInput(true);
        GameManager.Instance.SetCameraInput(true);
        GameManager.Instance.ShowCursor(false);
    }
}

// CSV 한 줄의 데이터를 저장하는 클래스
public class DialogueLine
{
    public string Name;
    public string Text;
    public int NextTextNumber;
    public string Op1Txt;
    public int Op1Num;
    public string Op2Txt;
    public int Op2Num;

    public DialogueLine(string name, string text, int nextTextNumber,
                        string op1Txt, int op1Num, string op2Txt, int op2Num)
    {
        Name = name;
        Text = text;
        NextTextNumber = nextTextNumber;
        Op1Txt = op1Txt;
        Op1Num = op1Num;
        Op2Txt = op2Txt;
        Op2Num = op2Num;
    }
}
