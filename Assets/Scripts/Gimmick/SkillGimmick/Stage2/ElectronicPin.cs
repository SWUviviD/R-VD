using StaticData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicPin : ShockableObj, IFusionable
{
    public enum State
    {
        Inactive,
        Turning,
        Active,
        Max
    }

    public enum Dir
    {
        Forward = 0,
        Left = 1,
        Backward = 2,
        Right = 3
    }

    public enum Type
    {
        Plus,
        Line,
        Curve,
        LongCurve
    }

    public LDPinMapData Data { get; set; }

    public State CurrentState { get; private set; }
    private State prevState;

    public Dir CurrentDir { get; private set; }

    private ElectronicMap map;

    [Header("Initialize")]
    [SerializeField] private float activateOffsetTime = 0.2f;

    [Header("Switch")]
    [SerializeField] private Renderer[] SwitchPipeRender;
    private List<Material> SwitchPipeMaterial = new List<Material>(2);
    [SerializeField] private Material[] InactiveColor;
    [SerializeField] private Material[] ActiveColor;

    [Header("Pipe")]
    [SerializeField] private Transform PipePos;
    [SerializeField] private GameObject[] Pipes;

    [SerializeField] private float PipeTurnSpeed = 0.5f;

    public Vector2Int PinPos { get; set; }

    public GameObject ellectricEffect;


    public void Init(ElectronicMap map, LDPinMapData data)
    {
        Data = data;

        for (int i = 0; i < Pipes.Length; i++)
        {
            Pipes[i].SetActive(i == data.Type);
        }

        for(int i = 0; i < 2; ++i)
        {
            SwitchPipeMaterial.Add( SwitchPipeRender[i].material);
            SwitchPipeMaterial[i].color = InactiveColor[i].color;
        }

        PipePos.rotation = Quaternion.Euler(Vector3.up * (int)data.Dir * 90f);

        prevState = State.Inactive;
        CurrentState = State.Inactive;

        CurrentDir = (Dir)data.Dir;

        this.map = map;
    }

    public bool Activate(Transform player)
    {
        if(CurrentState == State.Inactive)
        {
            prevState = State.Inactive;
            StartCoroutine(CoTurnPipe());
            return true;
        }

        return false;
    }

    private IEnumerator CoTurnPipe()
    {
        CurrentState = State.Turning;

        Vector3 startRot = PipePos.rotation.eulerAngles;
        Vector3 targetEulerRot = startRot + Vector3.down * 90f ;

        PipePos.Rotate(Vector3.down);

        float elapsedTime = 0.0f;
        while(true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= PipeTurnSpeed)
            {
                break;
            }

            //PipePos.Rotate(Vector3.down * Time.deltaTime * PipeTurnSpeed * 90f);


            PipePos.rotation = Quaternion.Euler(Vector3.Lerp(
                    startRot, targetEulerRot, elapsedTime / PipeTurnSpeed));
        }

        CurrentDir = (Dir)(((int)CurrentDir + 1) % 4);
        PipePos.rotation = Quaternion.Euler(Vector3.down * (int)CurrentDir * 90f);

        CurrentState = prevState;
        if(CurrentState == State.Active)
        {
            StartCoroutine(CoActivate());
        }
    }

    private IEnumerator CoActivate()
    {
        float elapsedTime = 0.0f;
        while(true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= activateOffsetTime)
            {
                break;
            }

            for(int i = 0; i < 2; ++i)
            {
                SwitchPipeMaterial[i].color = 
                    Color.Lerp(InactiveColor[i].color, ActiveColor[i].color, 
                    elapsedTime / activateOffsetTime);
            }
        }

        for (int i = 0; i < 2; ++i)
        {
            SwitchPipeMaterial[i].color = ActiveColor[i].color;
        }

        ShockNext();
    }

    private IEnumerator CoInactivate()
    {
        float elapsedTime = 0.0f;
        while (true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= activateOffsetTime)
            {
                break;
            }

            for (int i = 0; i < 2; ++i)
            {
                SwitchPipeMaterial[i].color =
                    Color.Lerp(ActiveColor[i].color, InactiveColor[i].color,
                    elapsedTime / activateOffsetTime);
            }
        }

        for (int i = 0; i < 2; ++i)
        {
            SwitchPipeMaterial[i].color = InactiveColor[i].color;
        }

        ShockFailToOther();
    }

    public override void OnShocked(ShockableObj obj)
    {
        if(CurrentState == State.Turning)
        {
            prevState = State.Active;
        }

        GiveShockObj = obj;

        ellectricEffect.SetActive(true);
        CurrentState = State.Active;
        StartCoroutine(CoActivate());
    }

    private void ShockNext()
    {
        map.ShockNextPin(this);
    }

    public override void ShockFailed(ShockableObj obj = null)
    {
        if(CurrentState != State.Active)
        {
            return;
        }

        if (map.CheckIfStillActive(this, obj))
        {
            return;
        }

        GiveShockObj = null;
        prevState = State.Inactive;
        CurrentState = State.Inactive;
        StartCoroutine(CoInactivate());

        ellectricEffect.SetActive(false);
    }

    private void ShockFailToOther()
    {
        if (CurrentState != State.Inactive)
            return;

        map.ShockFailNextPin(this);
    }
}
