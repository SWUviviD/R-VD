using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornStickMap : MonoBehaviour
{
    [Header("Stick")]
    [SerializeField] private GameObject stickPrefab;
    [SerializeField] private Transform stickParent;
    [SerializeField] private int initialStickCount = 24;
    private Queue<ThornStick> sticks = new Queue<ThornStick>();
    private List<bool> stickPos;

    [Header("Status")]
    [SerializeField] private float stickLifeTime = 17.0f;
    public float StickLifeTime { get { return stickLifeTime; } }
    [SerializeField] private int stickRow = 3;
    [SerializeField] private float dropStickOffset = 3.0f;
    private WaitForSeconds WFdropOffset;

    [Header("StartPos")]
    [SerializeField] private float stickPosOffset = 8f;
    [SerializeField] private Transform stickDropStartPos;

    private PlayerMove playerMove;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        WFdropOffset = new WaitForSeconds(dropStickOffset);
        stickPos = new List<bool>(stickRow);
        for(int i = 0; i < stickRow; ++i)
        {
            stickPos.Add(false);
        }

        for(int i = 0; i < initialStickCount; ++i )
        {
            EnqueueStick(CreateStick());
        }
    }

    private IEnumerator CoStartDropping()
    {
        while(true)
        {
            int stickCount = Random.Range(1, stickRow);
            Debug.Log("SCount: " + stickCount);

            for(int i = 0; i < stickRow; ++i )
            {
                stickPos[i] = false;
            }

            for (int i = 0; i < stickCount; ++i)
            {
                ThornStick s = DequeueStick();
                int pos = 0;
                do
                {
                    pos = Random.Range(0, stickRow);
                } while (stickPos[pos] == true);

                s.transform.position = stickDropStartPos.position + 
                    stickDropStartPos.transform.right * stickPosOffset * pos;

                s.Drop();
            }

            yield return WFdropOffset;
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
            StartCoroutine(CoStartDropping());
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (playerMove == null)
            return;

        Transform parent = other.transform.parent;
        if (parent == null) return;

        if (parent.TryGetComponent<PlayerMove>(out var move) == true)
        {
            if (playerMove == move)
            {
                playerMove = null;
                StopAllCoroutines();
            }
        }
    }


    #region pool
    public void EnqueueStick(ThornStick stick)
    {
        stick.gameObject.SetActive(false);
        sticks.Enqueue(stick);
    }

    private ThornStick CreateStick()
    {
        GameObject s = Instantiate(stickPrefab, stickParent);
        ThornStick sScript = s.GetComponent<ThornStick>();
        sScript.Init(this);
        return sScript;
    }

    private ThornStick DequeueStick()
    {
        if (sticks.Count == 0)
        {
            EnqueueStick(CreateStick());
        }

        sticks.Peek().gameObject.SetActive(true);
        return sticks.Dequeue();
    }
    #endregion
}
