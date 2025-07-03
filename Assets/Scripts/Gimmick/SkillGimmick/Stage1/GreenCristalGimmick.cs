using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCristalGimmick : GimmickBase<GreenCristalGimmickData>
{
    [SerializeField] private Cristal cristal;
    [SerializeField] private CristalSense sense;

    [SerializeField] private Transform gimmick;
    [SerializeField] private GameObject platePrefab;
    [SerializeField] private AudioSource audioSource;

    private const string GimmickStartPoint = "GimmickStartPoint";
    private const string GimmickEndPoint = "GimmickEndPoint";

    private bool isCristalBroke = false;

    private List<GreenCristalGimmickPlate> plateList = new List<GreenCristalGimmickPlate>();

    private float elapsedTime = 0f;
    private int currentPlateIndex = 0;

    protected override void Init()
    {
        cristal.Init();

        sense.OnCristalBreak.RemoveListener(OnCristalBreak);
        sense.OnCristalBreak.AddListener(OnCristalBreak);

        SetGimmick();
    }

    [ContextMenu("SetMenu")]
    public override void SetGimmick()
    {
        cristal.SetGimmick();

        isCristalBroke = false;

        gimmick.gameObject.SetActive(false);

        elapsedTime = 0f;
        currentPlateIndex = 0;

        Vector3 startPoint = gimmickData.DictPoint[GimmickStartPoint].position;
        Vector3 endPoint = gimmickData.DictPoint[GimmickEndPoint].position;
        int plateCount = gimmickData.PlateCount;

        for (int i = 0; i < plateCount; ++i)
        {
            Vector3 position = Vector3.Lerp(startPoint, endPoint, (float)i / (plateCount - 1));

            if (plateList.Count > i && plateList.Count > 0)
            {
                plateList[i].SetGimmick(this, position, Vector3.one);
            }
            else
            {
                GameObject ob = Instantiate(platePrefab, position, Quaternion.identity);
                ob.transform.SetParent(gimmick);
                plateList.Add(ob.GetComponent<GreenCristalGimmickPlate>());
                plateList[i].SetGimmick(this, position, Vector3.one);
            }
        }

        platePrefab.SetActive(false);
    }

    private void Update()
    {
        if (isCristalBroke == false)
        {
            cristal.cristalUpdate?.Invoke();
        }
    }

    [ContextMenu("CristalBreak")]
    public void OnCristalBreak()
    {
        isCristalBroke = true;
        audioSource.Play();
        cristal.OnCristalBreak();
        ActivateGimmick();
    }

    public void ActivateGimmick()
    {
        gimmick.gameObject.SetActive(true);
        StartCoroutine(CoStartLoop());
    }

    private IEnumerator CoStartLoop()
    {
        currentPlateIndex = 0;
        while(true)
        {
            if (currentPlateIndex >= gimmickData.PlateCount)
                break;

            elapsedTime += Time.deltaTime;
            if (elapsedTime > gimmickData.NextPlateShowTime)
            {
                elapsedTime -= gimmickData.NextPlateShowTime;
                plateList[currentPlateIndex].AppearPlate();
                currentPlateIndex += 1;
            }

            yield return null;
        }
    }
}