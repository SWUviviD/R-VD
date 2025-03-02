using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingGimmick : GimmickBase<ChasingGimmickData>
{
    private enum StarSize
    {
        Small,
        Medium,
        Large
    }

    private enum WFSType
    {
        StartDelay,
        IntervalTime,
        CoolTime,
    }

    private struct ShootingStarData
    {
        public int Count { get; private set; }
        public WaitForSeconds StartDelay { get; private set; }
        public WaitForSeconds IntervalTime { get; private set; }
        public WaitForSeconds CoolTime { get; private set; }

        public ShootingStarData(int _count, float _startDelay, float _intervalTime, float _coolTime)
        {
            Count = _count;
            StartDelay = new WaitForSeconds(_startDelay);
            IntervalTime = new WaitForSeconds(_intervalTime);
            CoolTime = new WaitForSeconds(_coolTime);
        }
    }

    [SerializeField] private Transform coverArea;
    [SerializeField] public GameObject[] starPrefab;

    private List<ShootingStarData> WFSLists;
    private List<List<FallingStar>> starLists;


    private PlayerMove playerMove;
    private Bounds bound;

    protected override void Init()
    {
        SetGimmick();
    }

    [ContextMenu("SetGimmick")]
    public override void SetGimmick()
    {
        StopAllCoroutines();

        //coverArea.localScale = gimmickData.ChaseFloorScale;
        coverArea.GetComponent<Renderer>().enabled = false;
        bound = coverArea.GetComponent<Collider>().bounds;

        PrepareStarList();

        foreach (var list in starLists)
        {
            foreach(var star in list)
            {
                star.SetGimmick();
            }
        }
    }

    private void PrepareStarList()
    {
        ClearStarList();

        int length = starPrefab.Length;

        starLists = new List<List<FallingStar>>(length);
        WFSLists = new List<ShootingStarData>(length);

        StarSize size = StarSize.Small;

        int starCount = 0;
        for (int i = 0; i < length; ++i)
        {
            starLists.Add(new List<FallingStar>());

            switch (size)
            {
                case StarSize.Small:
                    {
                        starCount = gimmickData.SmallStarCount * 3;

                        WFSLists.Add(new ShootingStarData(
                            gimmickData.SmallStarCount,
                            gimmickData.SmallStarStartDelay,
                            gimmickData.SmallStarFallingIntervalTime,
                            gimmickData.SmallStarCoolTime));
                        break;
                    }
                case StarSize.Medium:
                    {
                        starCount = gimmickData.MediumStarCount * 3;

                        WFSLists.Add(new ShootingStarData(
                            gimmickData.MediumStarCount,
                            gimmickData.MediumStarStartDelay,
                            gimmickData.MediumStarFallingIntervalTime,
                            gimmickData.MediumStarCoolTime));
                        break;
                    }
                case StarSize.Large:
                    {
                        starCount = gimmickData.BigStarCount * 3;

                        WFSLists.Add(new ShootingStarData(
                            gimmickData.BigStarCount,
                            gimmickData.BigStarStartDelay,
                            gimmickData.BigStarFallingIntervalTime,
                            gimmickData.BigStarCoolTime));
                        break;
                    }
            }

            for(int j = 0; j < starCount; ++j)
            {
                GameObject newStar = Instantiate(starPrefab[(int)size], Vector3.zero, transform.rotation);
                newStar.transform.parent = transform;
                FallingStar star = newStar.GetComponent<FallingStar>();
                star.Init();
                starLists[i].Add(star);
            }

            ++size;
        }
    }

    private void ClearStarList()
    {
        if (starLists == null)
            return;

        for(int i = 0, length = starPrefab.Length; i< length; ++i)
        {
            foreach(FallingStar newStar in starLists[i])
            {
                Destroy(newStar);
            }
        }
    }

    private IEnumerator CoStartShootingStars(StarSize _starSize)
    {
        List<FallingStar> list = starLists[(int)_starSize];
        ShootingStarData data = WFSLists[(int)_starSize];

        yield return data.StartDelay;

        int currentIndex = 0;
        while(true)
        {
            for(int i = 0; i< data.Count; ++i)
            {
                list[currentIndex].StartFalling(playerMove.GetPosition(), bound);
                currentIndex = (currentIndex + 1) % list.Count;
                yield return data.IntervalTime;
            }

            yield return data.CoolTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform parent = other.transform.parent;
        if (parent == null)
            return;

        if (parent.TryGetComponent<PlayerMove>(out var move) == true)
        {
            playerMove = move;

            for (int i = 0, length = starPrefab.Length; i < length; ++i)
            {
                StartCoroutine(CoStartShootingStars((StarSize)i));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerMove == null)
            return;

        Transform parent = other.transform.parent;
        if (parent == null) return;

        if (parent.TryGetComponent <PlayerMove>(out var move) == true)
        {
            if(playerMove == move)
            {
                playerMove = null;
                StopAllCoroutines();
            }
        }
    }
}
