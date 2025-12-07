using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicSwitchB : ElectronicSwitch
{
    [SerializeField] private int connectCount;

    [Header("Effect")]
    [SerializeField] private Renderer[] renderList;
    private List<Material> SwitchPipeMaterial = new List<Material>(2);
    [SerializeField] private Material[] InactiveColor;
    [SerializeField] private Material[] ActiveColor;
    [SerializeField] private float fadeTime = 0.2f;

    private int currentConnectedCount;
    private HashSet<ShockableObj> connectedSources = new HashSet<ShockableObj>();

    public GameObject ellectricEffect;

    public void Awake()
    {
        currentConnectedCount = 0;

        for (int i = 0; i < renderList.Length; ++i)
        {
            SwitchPipeMaterial.Add(renderList[i].material);
            SwitchPipeMaterial[i].color = InactiveColor[i].color;
        }
    }

    public override void OnShocked(ShockableObj obj)
    {
        if (currentState != State.Stopped) return;

        PowerSourceObj = obj;

        ++currentConnectedCount;

        if (currentState == State.Generating)
            return;

        if(currentConnectedCount >= connectCount)
        {
            currentConnectedCount = Mathf.Min(currentConnectedCount, connectCount);

            currentState = State.Generating;
            StartCoroutine(CoGenerating());

            ellectricEffect.SetActive(true);
            OnActivated?.Invoke();
        }
    }

    private IEnumerator CoActivate()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;

            for (int i = 0; i < renderList.Length; ++i)
            {
                SwitchPipeMaterial[i].color =
                    Color.Lerp(InactiveColor[i].color, ActiveColor[i].color,
                    elapsedTime / fadeTime);
            }
            yield return null;
        }


        for (int i = 0; i < renderList.Length; ++i)
        {
            SwitchPipeMaterial[i].color = ActiveColor[i].color;
        }
    }

    public override void ShockFailed(ShockableObj obj = null)
    {
        --currentConnectedCount;

        if(currentState == State.Generating && 
            currentConnectedCount < connectCount)
        {
            ellectricEffect.SetActive(false);
            base.ShockFailed();
        }
    }
}
