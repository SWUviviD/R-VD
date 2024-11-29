using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPUI : MonoBehaviour
{
    [SerializeField] private Transform player;
    private PlayerHp playerHP;
    private PlayerMove playerMove;
    private StarHunt starHunt;

    [Header("Objects")]
    [SerializeField] private Transform line;
    [SerializeField] private Transform bigStar;
    [SerializeField] private Transform[] smallStars;

    [Header("HPOption")]
    [SerializeField] private Vector3 starShowScale;
    [SerializeField] private Vector3 starHideScale;
    [SerializeField] private float hpHideTime;
    [SerializeField] private float hpShowingTime;
    private WaitForSeconds waitFor_Hide;

    private bool isStarShown = false;
    private bool isStarShowing = false;

    private int currentShowStarCount = 0;

    private void Awake()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        playerHP = player.GetComponent<PlayerHp>();
        playerMove = player.GetComponent <PlayerMove>();
        starHunt = player.GetComponent<StarHunt>();

        playerHP.OnDamaged.RemoveListener(OnDamaged);
        playerHP.OnDamaged.AddListener(OnDamaged);

        playerHP.OnDeath.RemoveListener(OnDeath);
        playerHP.OnDeath.AddListener(OnDeath);

        playerMove.OnInteractWithFloorStart.RemoveListener(ShowStar);
        playerMove.OnInteractWithFloorStart.AddListener(ShowStar);

        playerMove.OnInteractWithFloorEnd.AddListener(() =>
        {
            StartCoroutine(CoHpHide());
        });

        starHunt.OnStarHuntKeyDown.RemoveListener(ShowStar);
        starHunt.OnStarHuntKeyDown.AddListener(ShowStar);

        starHunt.OnStarHuntKeyUp.AddListener(() =>
        {
            StartCoroutine(CoHpHide());
        });

        waitFor_Hide = new WaitForSeconds(hpHideTime);


        line.localScale = starHideScale;
        isStarShown = false;
        isStarShowing = false;
    }

    private void Start()
    {
        StartCoroutine (CoHpHide());
    }

    private void OnDamaged(int _currentHP)
    {
        StopAllCoroutines();
        ShowStar();

        if (_currentHP == currentShowStarCount)
        {
            return;
        }    

        currentShowStarCount = _currentHP;
        int smallStarCount = smallStars.Length;
        for(int i = 0; i < smallStarCount; ++i)
        {
            if( i < _currentHP && smallStars[i].gameObject.activeSelf == false)
            {
                RestoreStar(smallStars[i]);
            }
            else if( i >= _currentHP - 1 && smallStars[i].gameObject.activeSelf == true)
            {
                StarBroke(smallStars[i]);
            }
        }

        if (_currentHP > 0 && bigStar.gameObject.activeSelf == false)
        {
            RestoreStar(bigStar);
        }
    }

    private void OnDeath()
    {
        // 여기에 큰별 깨지는 연출
        bigStar.gameObject.SetActive(false);

        // 여기에 죽는 연출

    }

    private void StarBroke(Transform _star)
    {
        // 여기에 별 깨지는 연출
        //가라
        _star.gameObject.SetActive(false);
    }

    private void RestoreStar(Transform _star)
    {
        // 여기에 별 다시 붙는 연출
        _star.gameObject.SetActive(true);
    }

    private void ShowStar()
    {
        if (isStarShown == true)
            return;

        StopAllCoroutines();
        if (isStarShowing == true)
        {
            StartCoroutine(CoShowStar(line.localScale, starShowScale, line.localScale.x / starShowScale.x, () => { isStarShown = true; }));
        }
        else
        {
            StartCoroutine(CoShowStar(starHideScale, starShowScale, 0f, () => { isStarShown = true; }));
        }
    }

    private void HideStar()
    {
        if (isStarShown == false)
            return;

        StopAllCoroutines();
        if(isStarShowing == true)
        {
            StartCoroutine(CoShowStar(line.localScale, starHideScale, line.localScale.x / starHideScale.x, () => { isStarShown = false; }));
        }
        else
        {
            StartCoroutine(CoShowStar(starShowScale, starHideScale, 0f, () => { isStarShown = false; }));
        }
    }

    private IEnumerator CoHpHide()
    {
        yield return waitFor_Hide;
        HideStar();
    }

    private IEnumerator CoShowStar(Vector3 _startScale, Vector3 _endScale, float startRadio, Action _OnCoroutineEnd)
    {
        isStarShowing = true;


        float elpasedTime = startRadio * hpShowingTime;
        Vector3 newScale = _startScale;
        while (true)
        {
            newScale = Vector3.Lerp(newScale, _endScale, elpasedTime / hpShowingTime);
            line.localScale = newScale;

            if (elpasedTime >= hpShowingTime)
                break;

            elpasedTime += Time.deltaTime;
            yield return null;
        }


        isStarShowing = false;
        _OnCoroutineEnd.Invoke();
    }

}
