using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicSwitchB : ElectronicSwitch
{
    [SerializeField] private int connectCount;

    [Header("Effect")]
    [SerializeField] private float fadeTime = 0.2f;
    [SerializeField] private Renderer[] renderList;
    private List<Material> SwitchPipeMaterial = new List<Material>(2);
    
    [SerializeField] private Material[] InactiveColor;
    [SerializeField] private Material[] ActiveColor;

    private Color[] inactivateEmission;
    private Color[] activateEmission;
    

    private readonly string EmissionColorStr = "_EmissionColor";
    private readonly string EmissionStr = "_EMISSION";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activatedSound;
    [SerializeField] private AudioClip inactivatedSound;

    private int currentConnectedCount;
    private HashSet<ShockableObj> connectedSources = new HashSet<ShockableObj>();

    public GameObject ellectricEffect;

    public void Awake()
    {
        currentConnectedCount = 0;

        audioSource = GetComponent<AudioSource>();

        inactivateEmission = new Color[renderList.Length];
        activateEmission = new Color[renderList.Length];

        for (int i = 0; i < renderList.Length; ++i)
        {
            SwitchPipeMaterial.Add(renderList[i].material);
            SwitchPipeMaterial[i].color = InactiveColor[i].color;
            SwitchPipeMaterial[i].DisableKeyword(EmissionStr);

            inactivateEmission[i] = Color.black;
            activateEmission[i] = ActiveColor[i].GetColor(EmissionColorStr);
        }
    }

    public override void OnShocked(ShockableObj obj)
    {
        if (currentState != State.Stopped) return;
        if (currentState == State.Generating) return;

        PowerSourceObj = obj;

        ++currentConnectedCount;

        if (currentState == State.Generating)
            return;

        if(currentConnectedCount >= connectCount)
        {
            currentConnectedCount = Mathf.Min(currentConnectedCount, connectCount);

            currentState = State.Generating;
            StartCoroutine(CoGenerating());

            audioSource.Stop();
            audioSource.clip = activatedSound;
            audioSource.Play();

            ellectricEffect.SetActive(true);
            OnActivated?.Invoke();
        }
    }

    protected override IEnumerator CoGenerating()
    {
        foreach(var r in SwitchPipeMaterial)
        {
            r.EnableKeyword(EmissionStr);
        }

        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;

            for (int i = 0; i < renderList.Length; ++i)
            {
                SwitchPipeMaterial[i].color =
                    Color.Lerp(InactiveColor[i].color, ActiveColor[i].color,
                    elapsedTime / fadeTime);
                SwitchPipeMaterial[i].SetColor(EmissionColorStr,
                    Color.Lerp(inactivateEmission[i], activateEmission[i],
                    elapsedTime / fadeTime));
            }
            yield return null;
        }


        for (int i = 0; i < renderList.Length; ++i)
        {
            SwitchPipeMaterial[i].color = ActiveColor[i].color;
            SwitchPipeMaterial[i].SetColor(EmissionColorStr, activateEmission[i]);
        }

        foreach (var shockObj in shockObj)
        {
            shockObj?.OnShocked(this);
        }
    }

    protected override IEnumerator CoStopped()
    {
        foreach (var r in SwitchPipeMaterial)
        {
            r.DisableKeyword(EmissionStr);
        }

        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;

            for (int i = 0; i < renderList.Length; ++i)
            {
                SwitchPipeMaterial[i].color =
                    Color.Lerp(ActiveColor[i].color, InactiveColor[i].color,
                    elapsedTime / fadeTime);
            }
            yield return null;
        }


        for (int i = 0; i < renderList.Length; ++i)
        {
            SwitchPipeMaterial[i].color = InactiveColor[i].color;
        }

        foreach (var shockObj in shockObj)
        {
            shockObj?.ShockFailed(this);
        }
    }

    public override void ShockFailed(ShockableObj obj = null)
    {
        --currentConnectedCount;

        if (currentState == State.Generating && 
            currentConnectedCount < connectCount)
        {
            audioSource.Stop();
            audioSource.clip = inactivatedSound;
            audioSource.Play();

            ellectricEffect.SetActive(false);
            base.ShockFailed();
        }
    }
}
