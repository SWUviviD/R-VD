using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicPipe : ShockableObj
{
    [SerializeField] private float shockSpeed = 0.02f;

    [SerializeField] private ShockableObj[] attached;
    private bool isSending = false;

    public GameObject ellectricEffect;

    [SerializeField] private Renderer[] PipeRender;
    private List<Material> PipeMaterial;
    [SerializeField] private Material[] InactiveColor;
    [SerializeField] private Material[] ActiveColor;

    private readonly string EmissionStr = "_EmissionColor";
    private Color[] inactivateEmission;
    private Color[] activateEmission;

    private void Start()
    {
        inactivateEmission = new Color[InactiveColor.Length];
        activateEmission = new Color[ActiveColor.Length];

        PipeMaterial = new List<Material>();
        for (int i = 0; i < PipeRender.Length; ++i)
        {
            PipeMaterial.Add(PipeRender[i].material);
            PipeMaterial[i].color = InactiveColor[i].color;

            inactivateEmission[i] = InactiveColor[i].GetColor(EmissionStr);
            activateEmission[i] = ActiveColor[i].GetColor(EmissionStr);

            PipeMaterial[i].SetColor(EmissionStr, inactivateEmission[i]);
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
        isSending = true;

        float elapsedTime = 0.0f;
        while(true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= shockSpeed)
            {
                break;
            }

            for(int i = 0; i< PipeMaterial.Count; ++i)
            {
                PipeMaterial[i].color =
                    Color.Lerp(InactiveColor[i].color, ActiveColor[i].color,
                    elapsedTime / shockSpeed);
                PipeMaterial[i].SetColor(EmissionStr,
                    Color.Lerp(inactivateEmission[i], activateEmission[i],
                    elapsedTime / shockSpeed));
            }
        }

        for (int i = 0; i < PipeMaterial.Count; ++i)
        {
            PipeMaterial[i].color = ActiveColor[i].color;
            PipeMaterial[i].SetColor(EmissionStr, activateEmission[i]);
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
        isSending = true;

        float elapsedTime = 0.0f;

        while (true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= shockSpeed)
            {
                break;
            }

            for (int i = 0; i < PipeMaterial.Count; ++i)
            {
                PipeMaterial[i].color =
                    Color.Lerp(ActiveColor[i].color, InactiveColor[i].color,
                    elapsedTime / shockSpeed);
                PipeMaterial[i].SetColor(EmissionStr,
                    Color.Lerp(activateEmission[i], inactivateEmission[i],
                    elapsedTime / shockSpeed));
            }
        }

        for (int i = 0; i < PipeMaterial.Count; ++i)
        {
            PipeMaterial[i].color = InactiveColor[i].color;
            PipeMaterial[i].SetColor(EmissionStr, inactivateEmission[i]);
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
