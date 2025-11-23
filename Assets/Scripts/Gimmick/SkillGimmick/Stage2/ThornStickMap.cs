using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornStickMap : MonoBehaviour
{
    [Header("ThornCastle")]
    [SerializeField] private GameObject thronCastlePrefab;
    [SerializeField] private Renderer plain; 

    [Header("Stick")]
    [SerializeField] private GameObject stickPrefab;
    [SerializeField] private Transform stickParent;
    [SerializeField] private int initialStickCount = 24;
    private Queue<ThornStick> sticks = new Queue<ThornStick>();
    private bool[] stickPos;

    [Header("Status")]
    [SerializeField] private float stickLifeTime = 17.0f;
    public float StickLifeTime { get { return stickLifeTime; } }
    [SerializeField] private int stickRow = 3;
    [SerializeField] private float dropStickOffset = 3.0f;
    private WaitForSeconds WFdropOffset;

    [Header("StartPos")]
    [SerializeField] private float stickPosOffset = 8f;
    [SerializeField] private Transform stickDropStartPos;
    private Transform[] dropPositions;

    private PlayerMove playerMove;
    private bool isThrowing = false;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        plain.enabled = false;
        WFdropOffset = new WaitForSeconds(dropStickOffset);
        stickPos = new bool[stickRow];
        dropPositions = new Transform[stickRow];

        for(int i = 0; i < stickRow; ++i)
        {
            stickPos[i] = false;
            dropPositions[i] = Instantiate(thronCastlePrefab, transform).transform;
            dropPositions[i].position = stickDropStartPos.position +
                    stickDropStartPos.transform.right * stickPosOffset * i;

            
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

                s.transform.position = dropPositions[pos].position;

                s.Drop();
            }

            yield return WFdropOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMove>(out var move) == true
            && isThrowing == false)
        {
            playerMove = move;
            StartCoroutine(CoStartDropping());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<PlayerMove> (out var move) == true)
        {
            isThrowing = false;
            StopAllCoroutines();
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
