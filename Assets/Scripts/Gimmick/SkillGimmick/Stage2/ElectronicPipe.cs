using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicPipe : ShockableObj
{
    [SerializeField] private float shockSpeed = 0.02f;

    [SerializeField] private ShockableObj[] attached;
    private bool isSending = false;

    public GameObject ellectricEffect;

    [Header("Effect")]
    [SerializeField] private float fadeTime = 0.2f;
    [SerializeField] private Renderer[] renderList;
    private List<Material> SwitchPipeMaterial = new List<Material>(2);

    [SerializeField] private Material[] InactiveColor;
    [SerializeField] private Material[] ActiveColor;

    private Color[] inactivateEmission;
    private Color[] activateEmission;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activatedSound;
    [SerializeField] private AudioClip inactivatedSound;

    private readonly string EmissionColorStr = "_EmissionColor";
    private readonly string EmissionStr = "_EMISSION";

    private void Start()
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

    public override void OnShocked(ShockableObj obj)
    {
        if (isSending == true)
            return;

        PowerSourceObj = obj;
        StartCoroutine(CoSendShock());
    }

    private IEnumerator CoSendShock()
    {
        //audioSource.Stop();
        //.clip = activatedSound;
        //audioSource.Play();

        isSending = true;

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

        foreach (var shock in attached)
        {
            if (shock == PowerSourceObj)
                continue;
            shock.OnShocked(this);
        }

        isSending = false;
    }

    public override void ShockFailed(ShockableObj obj = null)
    {
        if(isSending == true)
        {
            StopAllCoroutines();
        }

        //GameObject ellectric = Instantiate(ellectricEffect, transform.position, Quaternion.identity);
        //Destroy(ellectric, 1f);

        PowerSourceObj = null;
        StartCoroutine(CoSendFail(obj));
    }

    private IEnumerator CoSendFail(ShockableObj shockFailObj)
    {
        //audioSource.Stop();
        //audioSource.clip = inactivatedSound;
        //audioSource.Play();

        isSending = true;

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

        foreach (var shock in attached)
        {
            if (shock == shockFailObj)
                continue;
            shock.ShockFailed(this);
        }

        isSending = false;
    }
}
