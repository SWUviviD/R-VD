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
        Left = 0,
        Forward = 1,
        Right = 2,
        Backward = 3
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

    private Dir dir;
    public Dir CurrentDir { get => dir; private set { dir = value; } }

    private ElectronicMap map;

    [Header("Initialize")]
    [SerializeField] private float activateOffsetTime = 0.2f;

    [Header("Switch")]
    [SerializeField] private Renderer[] SwitchPipeRender;
    private List<Material> SwitchPipeMaterial = new List<Material>(2);
    [SerializeField] private Material[] InactiveColor;
    [SerializeField] private Material[] ActiveColor;

    private readonly string EmissionStr = "_EmissionColor";
    private Color[] inactivateEmission;
    private Color[] activateEmission;

    [Header("Pipe")]
    [SerializeField] private Transform PipePos;
    [SerializeField] private GameObject[] Pipes;
    [SerializeField] private Renderer[] way;

    [SerializeField] private float PipeTurnSpeed = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip trunSound;
    [SerializeField] private AudioClip startSound;
    [SerializeField] private AudioClip StopSound;

    public Vector2Int PinPos { get; set; }

    public GameObject ellectricEffect;


    public void Init(ElectronicMap map, LDPinMapData data)
    {
        Data = data;

        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        for (int i = 0; i < Pipes.Length; i++)
        {
            Pipes[i].SetActive(i == data.Type);
        }

        inactivateEmission = new Color[SwitchPipeRender.Length];
        activateEmission = new Color[SwitchPipeRender.Length];

        for(int i = 0; i < SwitchPipeRender.Length; ++i)
        {
            SwitchPipeMaterial.Add( SwitchPipeRender[i].material);
            SwitchPipeMaterial[i].DisableKeyword("_EMISSION");
            SwitchPipeMaterial[i].color = InactiveColor[i].color;

            inactivateEmission[i] = InactiveColor[i].GetColor(EmissionStr);
            activateEmission[i] = ActiveColor[i].GetColor(EmissionStr);

            SwitchPipeMaterial[i].SetColor(EmissionStr, inactivateEmission[i]);
        }

        PipePos.rotation = Quaternion.Euler(Vector3.up * (int)data.Dir * 90f);

        prevState = State.Inactive;
        CurrentState = State.Inactive;

        CurrentDir = (Dir)data.Dir;

        this.map = map;


        for (int i = 0; i < 4; ++i)
        {
            bool isConnected = map.HasConnection(this, i);
            way[i].material.color = isConnected ? Color.green : Color.red;
            way[i].gameObject.SetActive(isConnected);
        }
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
        audioSource.Stop();
        audioSource.PlayOneShot(trunSound);
        CurrentState = State.Turning;

        Vector3 startRot = PipePos.rotation.eulerAngles;
        Vector3 targetEulerRot = startRot + Vector3.up * 90f ;

        PipePos.Rotate(Vector3.up);

        float elapsedTime = 0.0f;
        while(true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= PipeTurnSpeed)
            {
                break;
            }

            PipePos.rotation = Quaternion.Euler(Vector3.Lerp(
                    startRot, targetEulerRot, elapsedTime / PipeTurnSpeed));
        }

        CurrentDir = (Dir)(((int)CurrentDir + 1) % 4);
        PipePos.rotation = Quaternion.Euler(Vector3.up * (int)CurrentDir * 90f);

        for(int i = 0; i< 4; ++i)
        {
            bool isConnected = map.HasConnection(this, i);
            way[i].material.color = isConnected ? Color.green : Color.red;
            way[i].gameObject.SetActive(isConnected);
        }

        CurrentState = prevState;
        if(CurrentState == State.Active)
        {
            StartCoroutine(CoActivate());
        }
    }

    private IEnumerator CoActivate()
    {
        audioSource.Stop();
        audioSource.clip = startSound;
        audioSource.Play();

        foreach(var r in SwitchPipeMaterial)
        {
            r.EnableKeyword("_EMISSION");
        }

        float elapsedTime = 0.0f;
        while(true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= activateOffsetTime)
            {
                break;
            }

            for(int i = 0; i < SwitchPipeRender.Length; ++i)
            {
                SwitchPipeMaterial[i].color = 
                    Color.Lerp(InactiveColor[i].color, ActiveColor[i].color, 
                    elapsedTime / activateOffsetTime);
                SwitchPipeMaterial[i].SetColor(EmissionStr,
                    Color.Lerp(inactivateEmission[i], activateEmission[i],
                    elapsedTime / activateOffsetTime));
            }
        }

        for (int i = 0; i < SwitchPipeRender.Length; ++i)
        {
            SwitchPipeMaterial[i].color = ActiveColor[i].color;
            SwitchPipeMaterial[i].SetColor(EmissionStr, activateEmission[i]);
        }

        ShockNext();
    }

    private IEnumerator CoInactivate()
    {
        audioSource.Stop();
        audioSource.clip = StopSound;
        audioSource.Play();


        foreach (var r in SwitchPipeMaterial)
        {
            r.DisableKeyword("_EMISSION");
        }

        float elapsedTime = 0.0f;
        while (true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= activateOffsetTime)
            {
                break;
            }

            for (int i = 0; i < SwitchPipeRender.Length; ++i)
            {
                SwitchPipeMaterial[i].color =
                    Color.Lerp(ActiveColor[i].color, InactiveColor[i].color,
                    elapsedTime / activateOffsetTime);
                SwitchPipeMaterial[i].SetColor(EmissionStr,
                    Color.Lerp(activateEmission[i], inactivateEmission[i],
                    elapsedTime / activateOffsetTime));
            }
        }

        for (int i = 0; i < SwitchPipeRender.Length; ++i)
        {
            SwitchPipeMaterial[i].color = InactiveColor[i].color;
            SwitchPipeMaterial[i].SetColor(EmissionStr, inactivateEmission[i]);
        }

        ShockFailToOther();

        prevState = State.Inactive;
        CurrentState = State.Inactive;
    }

    public override void OnShocked(ShockableObj obj)
    {
        if (CurrentState == State.Active)
        {
            return;
        }


        if(CurrentState == State.Turning)
        {
            prevState = State.Active;
        }

        PowerSourceObj = obj;

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

        if (map.CheckIfStillActiveAfterThisShockFail(this, obj))
        {
            return;
        }

        PowerSourceObj = null;
        StartCoroutine(CoInactivate());

        ellectricEffect.SetActive(false);
    }

    private void ShockFailToOther()
    {
        map.ShockFailNextPin(this);
    }
}
