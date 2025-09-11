using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Defines;

public class CutSceneManager : MonoSingleton<CutSceneManager>
{
    private List<List<CutSceneInfo>> CutSceneData;
    private List<CutSceneInfo> currentCutSecneData;
    private int lineNum = -1;
    private Action onCutFinished = null;

    [Header("UI Element")]
    [SerializeField] private GameObject cutSceneUIPanel;
    [SerializeField] private Image bg1;
    [SerializeField] private Image bg2;
    private Image currentBg;
    private Image nextBg;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;

    [Header("State")]
    [SerializeField] private float imageShowTime = 1f;
    [SerializeField] private float textShowTime = 0.25f;
    private WaitForSeconds wfTextSowTime = null;

    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);

        CutSceneData = new List<List<CutSceneInfo>>();

        List<CutSceneInfo> cutData;
        SerializeManager.Instance.LoadDataFile(out cutData, "CutSceneInfo");
        if (cutData.Count <= 0)
            return;

        int index = -1;
        foreach(var cut in cutData)
        {
            if(index != cut.Number)
            {
                CutSceneData.Add(new List<CutSceneInfo>());
                index = cut.Number;
            }

            CutSceneData[index].Add(cut);
        }

        wfTextSowTime = new WaitForSeconds(textShowTime);

        button.onClick.AddListener(ShowNext);
        cutSceneUIPanel.SetActive(false);
    }

    public void PlayCutScene(CutSceneDefines.CutSceneNumber _number, Action _callBack)
    {
        int num = (int)_number;
        if(num >= CutSceneData.Count)
        {
            _callBack?.Invoke();
            return;
        }

        // 기타 움직임 제한
        GameManager.Instance.SetMovementInput(false);

        // UI 초기화
        bg1.sprite = null;
        bg2.sprite = null;

        currentBg = bg2;
        currentBg.color = Color.black;
        
        nextBg = bg1;
        nextBg.color = Color.black;

        text.text = string.Empty;
        cutSceneUIPanel.SetActive(true);

        button.gameObject.SetActive(false);

        // 데이터 선택
        currentCutSecneData = CutSceneData[num];
        lineNum = 0;

        onCutFinished = _callBack;

        ShowCut(lineNum);
    }

    private void ShowCut(int _lineNumber)
    {
        if(lineNum < 0 ||  lineNum >= currentCutSecneData.Count)
        {
            GameManager.Instance.SetMovementInput(true);
            cutSceneUIPanel.SetActive(false);
            onCutFinished?.Invoke();
            onCutFinished = null;
            return;
        }

        if (currentCutSecneData[lineNum].Image.CompareTo(".") != 0)
        {
            // 이미지 바뀌어야 함
            StartCoroutine(CoImageFadeInOut(currentCutSecneData[lineNum].Image));
        }
        else if (currentCutSecneData[lineNum].Text.CompareTo(".") != 0)
        {
            // 텍스트 출력되어야 함
            StartCoroutine(CoTextInOut(currentCutSecneData[lineNum].Text));
        }
        else
        {
            // 이미지 페이드 아웃 후 컷씬 종료
            StartCoroutine(CoImageFadeInOut(string.Empty));
        }
    }

    private IEnumerator CoImageFadeInOut(string _nextImage)
    {
        // 초기화
        text.text = string.Empty;
        button.gameObject.SetActive(false);

        if (_nextImage.CompareTo(string.Empty) == 0)
        {
            nextBg.sprite = null;
            nextBg.color = Color.black;
        }
        else
        {
            Sprite nextBgSprite = AddressableAssetsManager.Instance.SyncLoadObject<Sprite>(
            AddressableAssetsManager.Instance.GetPrefabPath("UI/CutScene", _nextImage),
            _nextImage);

            nextBg.sprite = nextBgSprite;
            nextBg.color = Color.white;
        }

        float elapsedTime = 0f;
        float alpha = 0f;
        while(true)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > imageShowTime)
            {
                currentBg.color = new Color(currentBg.color.r, currentBg.color.g, currentBg.color.b, 0f);
                nextBg.color = new Color(nextBg.color.r, nextBg.color.g, nextBg.color.b, 1f);
                break;
            }

            alpha = elapsedTime / imageShowTime;
            currentBg.color = new Color(currentBg.color.r, currentBg.color.g, currentBg.color.b, 1 - alpha);
            nextBg.color = new Color(nextBg.color.r, nextBg.color.g, nextBg.color.b, alpha);

            yield return null;
        }

        Image temp = currentBg;
        currentBg = nextBg;
        nextBg = temp;

        ShowCut(++lineNum);
    }

    private IEnumerator CoTextInOut(string _text)
    {
        button.gameObject.SetActive(false);

        int currentPos = 1;
        text.text = string.Empty;

        do
        {
            text.text = _text.Substring(0, currentPos);
            yield return wfTextSowTime;

            currentPos++;
        } while (currentPos <= _text.Length);

        button.gameObject.SetActive(true);
    }

    public void ShowNext()
    {
        ShowCut(++lineNum);
    }
}
