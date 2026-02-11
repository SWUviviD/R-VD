using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocalData;

public class TestSerializeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<sample> samples = new List<sample>();
        SerializeManager.Instance.LoadDataFile<sample>(out samples, "sample");

        foreach (var item in samples)
        {
#if UNITY_EDITOR
            Debug.Log(item);
#endif
        }
    }
}
