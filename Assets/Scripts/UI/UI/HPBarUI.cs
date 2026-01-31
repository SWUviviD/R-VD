using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [Header("Skill")]
    [SerializeField] private Image skillImage;
    [SerializeField] private Sprite[] skillPng = new Sprite[(int)InputDefines.SkillType.MAX];


    [Header("Dash")]
    [SerializeField] private Image dashGauge;
    [SerializeField] private Image dashFull;
    [SerializeField] private AudioClip gaugeChargingClip;
    [SerializeField] private AudioClip gaugeFullClip;

    [Header("Hp")]
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private RectTransform[] hpStars = new RectTransform[10];
    [SerializeField] private float starScale = 0.2f;
    [SerializeField] private AudioClip attactedClip;
    private int currentShowingHP;
    private int maxHP = 10;
    private PlayerHp hp;

    private bool ShowHp = false;
    private int NeedShowHp = 0;

    private AudioSource audioSource;

    private void Awake()
    {
        skillImage.gameObject.SetActive(false);
        currentShowingHP = maxHP;

        audioSource = GetComponent<AudioSource>();

        hp = GameObject.FindAnyObjectByType<PlayerHp>();
        hp.OnDamaged.AddListener(OnDamaged);
        hp.OnHealed.AddListener(OnHealed);
        hp.OnSetHP.AddListener(OnSetHp);
    }

    private void Start()
    {
        if(ShowHp)
        {
            SetHP(NeedShowHp);
        }
    }

    private void OnDestroy()
    {
        hp.OnDamaged.RemoveListener(SetHP);
    }

    public void SetVisable(bool visable)
    {
        gameObject.SetActive(visable);
    }

    public void SwichSkill(InputDefines.SkillType skillType)
    {
        skillImage.sprite = skillPng[(int)skillType];
        skillImage.gameObject.SetActive(true);
    }

    public void SetDashGauge(float gauge)
    {
        if (gauge == 0)
        {
            audioSource.loop = true;
            audioSource.clip = gaugeChargingClip;
            audioSource.Play();
        }
        else if (gauge >= 1f)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.PlayOneShot(gaugeFullClip);
        }

        dashFull.gameObject.SetActive(gauge >= 1f);
        dashGauge.gameObject.SetActive(gauge < 1f);
        dashGauge.fillAmount = gauge;
    }

    private void OnDamaged(int hp)
    {
        audioSource.PlayOneShot(attactedClip);
        SetHP(hp);
    }

    private void OnHealed(int hp)
    {
        SetHP(hp);
    }

    private void OnSetHp(int hp)
    {
        SetHP(hp);
    }

    private void SetHP(int hp)
    {
        if(gameObject.activeSelf == false)
        {
            ShowHp = true;
            NeedShowHp = hp;
            return;
        }

        for(int i = 0; i < maxHP; ++i)
        {
            bool shouldShow = i < hp;
            bool isShowing = i < currentShowingHP;

            if(shouldShow && isShowing == false)
            {
                StartCoroutine(CoSetStar(hpStars[i], true));
            }
            else if(shouldShow == false && isShowing)
            {
                StartCoroutine(CoSetStar(hpStars[i], false));
            }
        }

        currentShowingHP = hp;
    }

    private IEnumerator CoSetStar(RectTransform t, bool isShow)
    {
        float startScale = isShow ? 0f : starScale;
        float endScale = isShow ? starScale : 0f;
        float elapsedTime = 0f;

        t.gameObject.SetActive(true);

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            t.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, elapsedTime /  fadeTime);
            yield return null;
        }

        t.localScale = Vector3.one * endScale;
        t.gameObject.SetActive(isShow);
    }
}
