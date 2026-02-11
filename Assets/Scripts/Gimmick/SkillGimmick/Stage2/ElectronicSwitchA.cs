using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicSwitchA : ElectronicSwitch, IFusionable
{
    private ElectronicMap map;
    private bool isConnectToMap = false;
    private int mapObjIndex = -1;

    public GameObject ellectricEffect;

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

    private bool isActivateFromStart = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activatedSound;
    [SerializeField] private AudioClip inactivatedSound;

    private void Awake()
    {
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

    protected override void Start()
    {
        base.Start();

        if (isActivateFromStart)
        {
            OnShocked(null);
        }
    }

    public override void ShockFailed(ShockableObj obj = null)
    {
        base.ShockFailed(obj);
        ellectricEffect.SetActive(false);
    }

    public override void SetForMap(ElectronicMap map, int index)
    {
        this.map = map;
        isConnectToMap = true;
        mapObjIndex = index;
    }

    public void ActivateFromCheckPoint()
    {
        isActivateFromStart = true;
    }

    [ContextMenu("Activate")]
    public bool Activate(Transform player)
    {
        if(currentState == State.Stopped)
            OnShocked(null);
        else if(currentState == State.Generating)
            StopGenerating();

        audioSource.Stop();
        audioSource.clip = activatedSound;
        audioSource.Play();

        ellectricEffect.SetActive(true);
        OnActivated?.Invoke();

        return true;
    }

    protected override IEnumerator CoGenerating()
    {
        audioSource.Stop();
        audioSource.clip = activatedSound;
        audioSource.Play();

        foreach (var r in SwitchPipeMaterial)
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

        if (isConnectToMap)
        {
            map.ShockPinFromOutside(mapObjIndex);
        }

        foreach (var obj in shockObj)
        {
            obj.OnShocked(this);
        }
    }

    protected override IEnumerator CoStopped()
    {
        audioSource.Stop();
        audioSource.clip = inactivatedSound;
        audioSource.Play();


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

        if (isConnectToMap)
        {
            map.ShockFailFromOutside(mapObjIndex);
        }

        foreach (var item in shockObj)
        {
            item.ShockFailed();
        }
    }
}
