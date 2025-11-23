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

    [Header("Hp")]
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private RectTransform[] hpStars = new RectTransform[10];
    private int currentShowingHP;
    private int maxHP = 10;
    private PlayerHp hp;

    private void Awake()
    {
        skillImage.gameObject.SetActive(false);
        currentShowingHP = maxHP;

        hp = GameObject.FindAnyObjectByType<PlayerHp>();
        hp.OnDamaged.RemoveListener(SetHP);
        hp.OnDamaged.AddListener(SetHP);
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
        dashFull.gameObject.SetActive(gauge >= 1f);
        dashGauge.gameObject.SetActive(gauge < 1f);
        dashGauge.fillAmount = gauge;
    }

    public void SetHP(int hp)
    {
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
        float startScale = isShow ? 0f : 1f;
        float endScale = isShow ? 1f : 0f;
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
