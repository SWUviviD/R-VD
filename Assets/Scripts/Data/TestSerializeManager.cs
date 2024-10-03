using StaticData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSerializeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<sample> samples = new List<sample>();
        SerializeManager.Instance.LoadDataFilie<sample>(out samples, "sample");

        foreach (var item in samples)
        {
            Debug.Log(item);
        }
    }
}
